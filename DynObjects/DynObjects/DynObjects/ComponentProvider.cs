// This file is part of the DynObjects library.
// Copyright 2010 Stefan Dragnev.
// The library and all its files are distributed under the MIT license.
// See the full text of the license in the accompanying LICENSE.txt file or at http://www.opensource.org/licenses/mit-license.php

using System;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using Dyn.Utilities;

namespace Dyn
{
    // Dictionary from Pair<Type of implementing component, Type of message delegate> to the trampoline Delegate instance itself
    using DispatcherKey = Pair<Type, Type>;
    using DispatchersDict = Dictionary<Pair<Type, Type>, Delegate>;

    /// <summary>
    /// Integration point. For internal use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DefaultMessageImplementerAttribute : Attribute {}
    
    /// <summary>
    /// This class is not meant to be used directly. When you write a .dyn file with the BuildDyn custom build tool, this class
    /// will be inherited by the integration code generated for you. Use <see cref="Merge"/> to merge several component providers.
    /// </summary>
    /// <remarks>
    /// Component providers store static data, which means that they can be used with different factories with certain restrictions.
    /// It's generally recommended to have a single factory instance for the entire application. See <see cref="DynFactory"/>.
    /// </remarks>
    public class ComponentProvider
    {
        static private AssemblyBuilder theAssemblyBuilder;
        static private ModuleBuilder theModuleBuilder;

        private const string MessagePrefix = "++";
        private const string OffsetFieldName = "_Offset";
        private string myDispatcherSource; // init from descendant data or merge
        internal bool IsSealed { get; private set; }

        internal DispatchersDict DispatcherDelegates = new DispatchersDict(); // init from descendant data or merge
        protected internal object[] DefaultMsgImpls { get; protected set; } // init from descendant data or merge
        protected internal Type[] Types { get; protected set; } // init from descendant data or merge
        protected internal int MessageCount { get; protected set; } // init from descendant data or merge
        protected Type ComponentAccessors { get; set; } // init from descendant data
        protected Type MessageExtensions { get; set; } // init from descendant data
        protected Type MessageDelegates { get; set; } // init from descendant data

        internal Type[] KnownDataContracts { get; private set; }

        internal XmlSerializer Serializer;

        private struct MessageDelegateInfo
        {
            public Type Delegate;
            public MethodInfo DelegateInfo;
            public Type Component;
            public Type MessageInterface;
        }

        protected ComponentProvider()
        {}

        private string GenerateSymbolsFileName()
        {
            return GetType().Name + "Dispatchers.src";
        }

        internal void Seal(Dictionary<Type, int> componentOffsets)
        {
            if (IsSealed)
                throw new DynException("Tried to seal an already sealed component provider.");

            // fix up the component type list
            TypeInfo.NormalizeComponentTypeList(Types);

            // We'll bootstrap the offset field of the component accessor class, but only if we're not going to run as part of a merged provider.
            // If we are part of a merged provider, then ComponentProvider.Merge will do the bootstrap. componentOffsets is null only when
            // called from Merge().
            if (componentOffsets == null)
            {
                foreach (var typeEnum in Types.Enumerate())
                {
                    var offsetFld = GetComponentOffsetField(typeEnum.Value);
                    offsetFld.SetValue(null, typeEnum.Index);
                }
            }

            // prepare the assembly and module builder
            CreateDelegatesThruReflectionEmit(componentOffsets, false);

            KnownDataContracts = (from type in Types
                                  let isContract = type.GetAttribute<SerializedComponentAttribute>() != null
                                  let delegates = type.GetAttribute<DelegatesSerializationAttribute>()
                                  where isContract || delegates != null
                                  select delegates == null ? type : delegates.Memento
                                 ).ToArray();

            Serializer = new XmlSerializer(typeof(DynFactory.ObjectMemento), KnownDataContracts);

            IsSealed = true;
        }

        private void CreateDelegatesThruReflectionEmit(IDictionary<Type, int> componentOffsets, bool emitSymbolInfo)
        {
            var name = GetType().Name;

            if (theAssemblyBuilder == null)
            {
                theAssemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName("DynDispatchers"), AssemblyBuilderAccess.Run);

                theModuleBuilder = theAssemblyBuilder.DefineDynamicModule("DynDisp", emitSymbolInfo);

                var daType = typeof(DebuggableAttribute);
                var daCtor = daType.GetConstructor(new[] { typeof(DebuggableAttribute.DebuggingModes) });
                var daBuilder = new CustomAttributeBuilder(daCtor, new object[] { DebuggableAttribute.DebuggingModes.Default });
                theAssemblyBuilder.SetCustomAttribute(daBuilder);
            }

            var dispatcherTypeNameBase = name + "Disp";
            var dispatcherTypeName = dispatcherTypeNameBase;
            for (int i = 1; theModuleBuilder.GetType(dispatcherTypeName) != null; ++i)
                dispatcherTypeName = dispatcherTypeNameBase + i;

            ISymbolDocumentWriter doc = null;
            if (emitSymbolInfo)
                doc = theModuleBuilder.DefineDocument(GenerateSymbolsFileName(), Guid.Empty, Guid.Empty, Guid.Empty);
            var dispatchersMethods = new List<MessageDelegateInfo>();

            // define the static class containing all dispatcher methods
            var dispatchersDef = theModuleBuilder.DefineType(dispatcherTypeName, TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.Public);
            var componentIdx = 0;
            foreach (var compType in Types.Concat(TypeInfo.TypesFromComponents(DefaultMsgImpls)))
            {
                if (componentOffsets != null)
                    componentIdx = componentOffsets[compType];

                foreach (var intf in compType.GetInterfaces())
                {
                    var msgAttr = intf.GetAttribute<MessageAttribute>();
                    if (msgAttr == null || msgAttr.ThruInterface)
                        continue;
                    if (msgAttr.Delegate == null)
                        throw new DynException("Delegate not found for message {0}. ");


                    var messageSig = intf.GetMethods()[0];
                    var messageParams = TypeInfo.TypesOfParameters(messageSig);
                    var methodImpl = compType.GetInterfaceMap(intf).TargetMethods[0];
                    if (methodImpl.IsPrivate)
                    {
                        //TODO(O): with enough security permission we'd be able to call those methods through delegates...
                        throw new DynException(String.Format(CultureInfo.InvariantCulture,
                                                "Component {0} implements message interface {1} explicitly." +
                                                "You should either implement the interface implicitly, or declare the message to be called thru interface.",
                                                compType.FullName, intf.FullName));
                    }

                    var dispatcher = dispatchersDef.DefineMethod(GenerateMessageDispatcherName(compType, intf), MethodAttributes.Static | MethodAttributes.Public);
                    dispatcher.SetParameters(new[] { typeof(DynObject) }.Concat(messageParams).ToArray());
                    dispatcher.SetReturnType(messageSig.ReturnType);
                    dispatcher.DefineParameter(1, ParameterAttributes.In, "_obj");
                    foreach (var param in messageSig.GetParameters())
                    {
                        var attrs = ParameterAttributes.None;
                    #if !SILVERLIGHT
                        if (param.IsIn) attrs |= ParameterAttributes.In;
                    #endif
                        if (param.IsOut) attrs |= ParameterAttributes.Out;
                        dispatcher.DefineParameter(param.Position + 1, attrs, param.Name);
                    }

                    var il = dispatcher.GetILGenerator();
                    if (doc != null)
                        il.MarkSequencePoint(doc, dispatchersMethods.Count + 1, 1, dispatchersMethods.Count + 1, 100);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, typeof(DynObject).GetField("_Components"));
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, typeof(DynObject).GetField("_ComponentOffsets"));
                    il.Emit(OpCodes.Ldc_I4, componentIdx);
                    il.Emit(OpCodes.Ldelem_I4);
                    il.Emit(OpCodes.Ldelem_Ref);
                    il.Emit(OpCodes.Castclass, compType);
                    for (int i = 0; i < messageSig.GetParameters().Length; ++i)
                    {
                        il.Emit(OpCodes.Ldarg, i + 1);
                    }
                    il.Emit(OpCodes.Callvirt, methodImpl);
                    il.Emit(OpCodes.Ret);

                    dispatchersMethods.Add(new MessageDelegateInfo { Delegate = msgAttr.Delegate, DelegateInfo = dispatcher, Component = compType, MessageInterface = intf });
                }
                componentIdx++;
            }

            // after creating the type, prepare delegates for each dispatcher
            var dispatchersType = dispatchersDef.CreateType();
            foreach (var disp in dispatchersMethods)
            {
                var method = dispatchersType.GetMethod(disp.DelegateInfo.Name, TypeInfo.TypesOfParameters(disp.DelegateInfo));
                var delg = Delegate.CreateDelegate(disp.Delegate, method);
                DispatcherDelegates.Add(new DispatcherKey { First = disp.Component, Second = disp.MessageInterface }, delg);
            }

            var sourceLines = from disp in dispatchersMethods
                              select string.Format(CultureInfo.InvariantCulture, "(obj as {0}).{1}()\n", disp.Component.FullName, disp.MessageInterface.Name);
            myDispatcherSource = string.Join("", sourceLines.ToArray());
        }

        public void WriteDispatcherSource(string filePath)
        {
            using (var sourceFile = new StreamWriter(Path.Combine(filePath, GenerateSymbolsFileName()), false))
            {
                sourceFile.Write(myDispatcherSource);
                myDispatcherSource = null;
            }
        }

        private static string GenerateMessageDispatcherName(Type componentType, Type message)
        {
            var componentNamePart = componentType.FullName.Replace('.', '_');
            var messageNamePart = message.FullName.Replace('.', '_');
            return componentNamePart + MessagePrefix + messageNamePart;
        }

        /// <summary>
        /// Merges several component providers into one. This is generally needed when you have several assemblies,
        /// each providing its own components and messages. Instantiate each provider once, and merge them into one
        /// super-provider that can then be supplied to the factory.
        /// </summary>
        /// <param name="providers"></param>
        /// <returns></returns>
        public static ComponentProvider Merge(params ComponentProvider[] providers)
        {
            if (providers == null) throw new ArgumentNullException("providers");
            if (providers.Length == 0)
                return null;

            var result = new ComponentProvider();

            var combinedTypes = from cp in providers
                          from type in cp.Types
                          orderby type.FullName
                          select new {Provider = cp, Type = type};

            result.Types = (from t in combinedTypes select t.Type).ToArray();

            var componentOffsets = new Dictionary<Type, int>();
            foreach (var typesEnum in combinedTypes.Enumerate())
            {
                var compType = typesEnum.Value.Type;
                var provider = typesEnum.Value.Provider;
                var index = typesEnum.Index;

                var offsetFld = provider.GetComponentOffsetField(compType);

                offsetFld.SetValue(null, index);
                componentOffsets.Add(compType, index);
            }
            
            result.myDispatcherSource = String.Join("", (from cp in providers select cp.myDispatcherSource).ToArray());
            result.MessageCount = (from cp in providers select cp.MessageCount).Sum();

            result.DefaultMsgImpls = (from cp in providers select cp.DefaultMsgImpls).FlattenOnce().ToArray();

            var defaultMsgImplIndex = componentOffsets.Count;
            foreach (var impl in result.DefaultMsgImpls)
            {
                componentOffsets.Add(impl.GetType(), defaultMsgImplIndex);
                defaultMsgImplIndex++;
            }

            var nextMessageStart = 0;
            foreach (var cp in providers)
            {
                var messageExt = cp.MessageExtensions;
                var offsetFld = messageExt.GetField(OffsetFieldName);
                offsetFld.SetValue(null, nextMessageStart);
                nextMessageStart += cp.MessageCount;
            }

            result.Seal(componentOffsets);
            return result;
        }

        private FieldInfo GetComponentOffsetField(Type compType)
        {
            var compNamespace = compType.Namespace;
            var accessorsNamespace = ComponentAccessors.Namespace;
            string relativePath;

            if (compNamespace.StartsWith(accessorsNamespace, StringComparison.Ordinal))
            {
                relativePath = compType.FullName.Remove(0, accessorsNamespace.Length + 1);
            }
            else if (compNamespace != accessorsNamespace)
            {
                relativePath = compType.FullName;
            }
            else
            {
                relativePath = compType.Name;
            }

            relativePath = relativePath.Replace('.', '_');

            var offsetFld = ComponentAccessors.GetField("_" + relativePath + "Offset");
            if (offsetFld == null)
                throw new DynException(String.Format(CultureInfo.InvariantCulture, "Couldn't get offset field for component {0}. Ensure that the component and its enclosing namespaces are properly declared in the <Components> section of the .dyn", compType.FullName));
            return offsetFld;
        }
    }
}
