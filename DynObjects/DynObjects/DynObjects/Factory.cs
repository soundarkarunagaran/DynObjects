// This file is part of the DynObjects library.
// Copyright 2010 Stefan Dragnev.
// The library and all its files are distributed under the MIT license.
// See the full text of the license in the accompanying LICENSE.txt file or at http://www.opensource.org/licenses/mit-license.php

using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Dyn.Utilities;

namespace Dyn
{
    /// <summary>
    /// Type analysis rules allow you to plug in custom code that analyzes DynObject types
    /// and add/remove component types to/from it based on custom logic.
    /// </summary>
    public interface ITypeAnalysisRule
    {
        /// <summary>
        /// Analysis entry point.
        /// </summary>
        /// <param name="components">The currently analyzed component types for a DynObject</param>
        /// <returns>true if modifications to the list of types were made, false otherwise</returns>
        bool Analyze(List<Type> components);
    }

    /// <summary>
    /// The factory produces new DynObjects given a ComponentProvider. It's generally recommended to
    /// use a single factory instance for the entire application. If you have several libraries that have
    /// component providers, it's a good idea to merge all component providers using <see cref="ComponentProvider.Merge"/>,
    /// create one factory from it and give access to it to all libraries. It's not a good idea to have
    /// libraries indiscriminately creating factories and passing around DynObjects created from different factorie.
    /// 
    /// Methods marked as 'Thread-safe' are thread-safe. All other methods are not guaranteed to be thread-safe.
    /// </summary>
    public sealed class DynFactory
    {
        private static readonly XmlWriterSettings DefaultXmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false };

        private static readonly XmlSerializerNamespaces XsiNamespace
            = new XmlSerializerNamespaces(new[]{ new XmlQualifiedName("i", "http://www.w3.org/2001/XMLSchema-instance") });

        private struct ComponentList : IEquatable<ComponentList>
        {
            public Type[] ComponentTypes;

            public bool Equals(ComponentList other)
            {
                ComponentTypes.SequenceEqual(other.ComponentTypes);
                return true;
            }

            public override int GetHashCode()
            {
                return ComponentTypes.Aggregate(0, (current, t) => current ^ t.GetHashCode());
            }
        }

        private class InjectionRules
        {
            private readonly HashSet<Type> myMandatoryComponents = new HashSet<Type>();
            private readonly Dictionary<Type, List<Type>> myInjectionRules = new Dictionary<Type, List<Type>>();

            public void AddMandatoryComponent(Type type)
            {
                myMandatoryComponents.Add(type);
            }

            public void AddInjectionRule(Type ifExists, Type add)
            {
                if (!myInjectionRules.ContainsKey(ifExists))
                    myInjectionRules.Add(ifExists, new List<Type> { add });
                else if (!myInjectionRules[ifExists].Contains(add))
                    myInjectionRules[ifExists].Add(add);
            }

            public void AnalyzeInjections(List<Type> components)
            {
                var existingComponents = new HashSet<Type>();
                foreach (var compType in components)
                    existingComponents.Add(compType);

                var added = (from mand in myMandatoryComponents
                            where !existingComponents.Contains(mand)
                            select mand).ToArray();
                //var result = added.Count() != 0;
                components.AddRange(added);
                foreach (var comp in added)
                    existingComponents.Add(comp);

                var sequenceBegin = 0;
                var sequenceEnd = components.Count;

                while (true)
                {
                    for (var i = sequenceBegin; i < sequenceEnd; ++i)
                    {
                        List<Type> dependencies;
                        if (!myInjectionRules.TryGetValue(components[i], out dependencies))
                            continue;

                        foreach (var dep in dependencies)
                        {
                            if (!existingComponents.Contains(dep))
                            {
                                components.Add(dep);
                                existingComponents.Add(dep);
                            }
                        }
                    }

                    var addedAny = sequenceEnd != components.Count;
                    //result |= addedAny;
                    if (!addedAny)
                        break;

                    sequenceBegin = sequenceEnd;
                    sequenceEnd = components.Count;
                }
            }
        }

        internal readonly ComponentProvider myComponentProvider;
        private readonly Dictionary<ComponentList, WeakReference> myTypeCache = new Dictionary<ComponentList, WeakReference>();
        private readonly List<ITypeAnalysisRule> myTypeAnalysisRules = new List<ITypeAnalysisRule>();
        private readonly InjectionRules myInjectionRules = new InjectionRules();

        private readonly Dictionary<object, TypeInfo> myRegisteredTypes = new Dictionary<object, TypeInfo>();

        /// <summary>
        /// Creates a new DynObject factory that works with the component types in the supplied ComponentProvider.
        /// </summary>
        /// <param name="componentProvider">Either a custom component provider created from a .dyn file, or the result of ComponentProvider.Merge()</param>
        public DynFactory(ComponentProvider componentProvider)
        {
            if (componentProvider == null) throw new ArgumentNullException("componentProvider");

            myComponentProvider = componentProvider;
            if (!myComponentProvider.IsSealed)
                myComponentProvider.Seal(null);
        }


        /// <summary>
        /// Retrieves the set of component types used in the input array of component types, DynObject's, arrays of component types and TypeInfo's
        /// </summary>
        public static Type[] GetTypes(params object[] objects)
        {
            var fromTypeInfos = objects.OfType<TypeInfo>().Select(ti => ti.myComponentTypes);
            var fromObjects = objects.OfType<DynObject>().Select(o => o.TypeInfo.myComponentTypes);
            var fromTypes = objects.OfType<Type>();
            var fromTypeArrays = objects.OfType<Type[]>();

            var fromComponents = from obj in objects
                                 where obj as DynObject == null
                                        && obj as TypeInfo == null
                                        && obj as Type[] == null
                                        && obj as Type == null
                                 select obj.GetType();


            return fromTypeInfos.FlattenOnce()
                .Concat(fromObjects.FlattenOnce())
                .Concat(fromTypeArrays.FlattenOnce())
                .Concat(fromTypes)
                .Concat(fromComponents)
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Creates an object type containing the specified component types. Thread-safe
        /// </summary>
        /// <param name="componentTypes"></param>
        /// <returns>The result can be used as a template for quickly creating new DynObjects with the specified component types.</returns>
        public TypeInfo CreateObjectType(params Type[] componentTypes)
        {
            if (!componentTypes.SequenceEqual(componentTypes.Distinct()))
                throw new DynException("Duplicate component types in object type info.");
            var fullComponentTypes = componentTypes.ToList();
            while (true)
            {
                myInjectionRules.AnalyzeInjections(fullComponentTypes);

                var modified = false;
                foreach (var rule in myTypeAnalysisRules)
                {
                    modified |= rule.Analyze(fullComponentTypes);
                }
                if (!modified)
                    break;
            }
            componentTypes = fullComponentTypes.ToArray();

            var componentList = new ComponentList { ComponentTypes = componentTypes };
            TypeInfo.NormalizeComponentTypeList(componentTypes);

            lock (myTypeCache)
            {
                WeakReference typeInfoRef;
                if (!myTypeCache.TryGetValue(componentList, out typeInfoRef))
                    return CreateTypeInfo(componentTypes, componentList);
                
                var typeInfo = (TypeInfo) typeInfoRef.Target;
                if (typeInfo != null)
                    return typeInfo;
                
                myTypeCache.Remove(componentList);
                return CreateTypeInfo(componentTypes, componentList);
            }
        }

        private TypeInfo CreateTypeInfo(Type[] componentTypes, ComponentList componentList)
        {
            var typeInfo = new TypeInfo(this, componentTypes);
            myTypeCache.Add(componentList, new WeakReference(typeInfo));
            return typeInfo;
        }

        /// <summary>
        /// Creates an object containing the specified component instances. Thread-safe
        /// </summary>
        public DynObject CreateObject(params object[] components)
        {
            var typeInfo = CreateObjectType(TypeInfo.TypesFromComponents(components));
            return typeInfo.CreateObject(components);
        }

        /// <summary>
        /// Creates an object containing the specified component instances. Thread-safe.
        /// Uses a user-provided key that uniquely identifies the object's type. This method
        /// is faster in general as it does type analysis on the components only for the first
        /// created object. Using this method also guarantees that the generated type info
        /// will not be garbage collected when there are no object instances using it.
        /// 
        /// Use this method if you create many objects that have the same set of components.
        /// Alternatively, use CreateObjectType and cache the result TypeInfo, then call
        /// TypeInfo.CreateObject() instead.
        /// 
        /// Use it like so:
        ///    private static object MyTypeKey = new object();
        ///    ...
        ///    Factory.CreateObjectFast(MyTypeKey, ...);
        /// </summary>
        public DynObject CreateObjectFast(object key, params object[] components)
        {
            lock (myRegisteredTypes)
            {
                var typeInfo = myRegisteredTypes.GetOrDefault(key, null);
                if (typeInfo == null)
                {
                    typeInfo = CreateObjectType(TypeInfo.TypesFromComponents(components));
                    myRegisteredTypes[key] = typeInfo;
                }

                return typeInfo.CreateObject(components);
            }
        }

        /// <summary>
        /// Adds a new type analysis rule. Can only be called before the first object is created.
        /// </summary>
        /// <param name="rule"></param>
        public void AddTypeAnalysisRule(ITypeAnalysisRule rule)
        {
            CheckCanAddTypeAnalysisRules();
            myTypeAnalysisRules.Add(rule);
        }

        /// <summary>
        /// Adds a mandatory component type. This component will be present in all DynObjects created from
        /// this factory. Can only be called before the first object is created.
        /// </summary>
        /// <param name="mandatoryComp"></param>
        public void AddMandatoryComponent(Type mandatoryComp)
        {
            CheckCanAddTypeAnalysisRules();
            myInjectionRules.AddMandatoryComponent(mandatoryComp);
        }

        /// <summary>
        /// Adds an injection rule. Useful whenever some component always relies on the existence
        /// of another component in a given DynObject type.
        /// </summary>
        /// <param name="ifExists"></param>
        /// <param name="thenAdd">This component will be added whenever the ifExists component is present.</param>
        public void AddInjectionRule(Type ifExists, Type thenAdd)
        {
            CheckCanAddTypeAnalysisRules();
            myInjectionRules.AddInjectionRule(ifExists, thenAdd);
        }

        /// <summary>
        /// Associates a fast object creation key with a given TypeInfo. See <see cref="CreateObjectFast"/>
        /// </summary>
        /// <param name="key">Fast object creation key</param>
        /// <param name="type"></param>
        public void RegisterType(object key, TypeInfo type)
        {
            lock (myRegisteredTypes)
                myRegisteredTypes.Add(key, type);
        }

        /// <summary>
        /// Thread-safe.
        /// </summary>
        public XElement SerializeXml(DynObject obj)
        {
            var doc = new XDocument();
            var dcs = myComponentProvider.Serializer;
            var memento = new ObjectMemento(obj);
            var writer = doc.CreateWriter();
            dcs.Serialize(writer, memento, XsiNamespace);
            writer.Close();
            var root = doc.Root;
            root.Remove();
            return root;
        }

        /// <summary>
        /// Thread-safe.
        /// </summary>
        public DynObject Deserialize(XNode objectXml, object context)
        {
            if (objectXml == null) throw new ArgumentNullException("objectXml");

            var serializer = myComponentProvider.Serializer;
            using (var reader = objectXml.CreateReader())
            {
                var memento = serializer.Deserialize(reader);
                return FinishDeserialization(memento, context);
            }
        }

        /// <summary>
        /// Thread-safe.
        /// </summary>
        public string Serialize(DynObject obj)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(stringBuilder, DefaultXmlWriterSettings))
            {
                var serializer = myComponentProvider.Serializer;
                var memento = new ObjectMemento(obj);
                serializer.Serialize(writer, memento, XsiNamespace);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Thread-safe.
        /// </summary>
        public DynObject Deserialize(string data, object context)
        {
            var serializer = myComponentProvider.Serializer;
            using (var reader = new StringReader(data))
            {
                var memento = serializer.Deserialize(reader);
                return FinishDeserialization(memento, context);
            }
        }

        private DynObject FinishDeserialization(object mementoObj, object context)
        {
            var memento = (ObjectMemento) mementoObj;
            memento.FinishDeserialization(context);
            var obj = CreateObject(memento.Components);
            return obj;
        }

        private void CheckCanAddTypeAnalysisRules()
        {
            if (myTypeCache.Count > 0)
                throw new DynException("Cannot add type analysis rules after types have already been created.");
        }

        // Not public for use. However it must remain public so that serialization works in Silverlight (otherwise a SecurityException is thrown).
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct ObjectMemento
        {
            public object[] Components;

            internal ObjectMemento(DynObject obj) : this()
            {
                Components = (from ci in obj.TypeInfo.myDataContractComponentTypes
                              let component = obj._Components[ci]
                              let customSer = component as ICustomSerializable
                              select customSer != null ? customSer.ToSerializable() : component)
                              .ToArray();
            }

            internal void FinishDeserialization(object context)
            {
                for (var i = 0; i < Components.Length; i++)
                {
                    var mementoAttr = Components[i].GetType().GetAttribute<MementoAttribute>();
                    if (mementoAttr == null)
                        continue;

                    var memento = Components[i];
                    var component = Activator.CreateInstance(mementoAttr.Of);
                    ((ICustomSerializable)component).FromSerializable(memento, context);
                    Components[i] = component;
                }
            }
        }
    }
}