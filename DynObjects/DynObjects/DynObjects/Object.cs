// This file is part of the DynObjects library.
// Copyright 2010 Stefan Dragnev.
// The library and all its files are distributed under the MIT license.
// See the full text of the license in the accompanying LICENSE.txt file or at http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Dyn.Utilities;

namespace Dyn
{
#if !SILVERLIGHT
    [Serializable]
#endif
	public sealed class DynException : Exception
	{
        public DynException()
        {}

		public DynException(string message)
            : base(message)
		{}

        public DynException(string message, Exception innerException)
            : base(message, innerException)
        {}

#if !SILVERLIGHT
        private DynException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif
    }

    /// <summary>
    /// Decorate components with this attribute if they have to be serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SerializedComponentAttribute : Attribute
    {}

    /// <summary>
    /// This is the main figure of the library. Create DynObjects from a DynFactory.
    /// </summary>
	public sealed class DynObject : IDisposable
	{
        /// <summary>
        /// The TypeInfo of this object. Can be used to create new objects of the same type.
        /// </summary>
		public TypeInfo TypeInfo { get; private set; }

        #region Private fields
        // all fields beginning with _ are not meant for public use, only for the extension methods for the messages!
        [EditorBrowsable(EditorBrowsableState.Never), CLSCompliant(false)]
        public object[] _Components;
        [EditorBrowsable(EditorBrowsableState.Never), CLSCompliant(false)]
        public int[] _ComponentOffsets;
        [EditorBrowsable(EditorBrowsableState.Never), CLSCompliant(false)]
        public int[] _UnicastImpl;
        [EditorBrowsable(EditorBrowsableState.Never), CLSCompliant(false)]
        public Delegate[] _MessageImpl;
        #endregion

        private void Construct(object[] components, TypeInfo ti)
		{
			_Components = components;
			TypeInfo = ti;
            _UnicastImpl = ti.myUnicastImpl;
            _MessageImpl = ti.myMessageDelegates;
            _ComponentOffsets = ti.myComponentOffsets;
		}

        internal DynObject(object[] components, TypeInfo ti)
        {
            Construct(components, ti);
            FinishComponentConstruction(_Components);
        }

        /// <summary>
        /// Gets a component instance by its type. Preferrably use the auto-generated component accessors.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The component instance, or null if this DynObject doesn't contain the component</returns>
        [DebuggerNonUserCode]
        public T GetComponent<T>() where T : class
		{
		    return (T)GetComponent(typeof(T));
		}

        /// <summary>
        /// Gets a component instance by its type. Preferrably use the auto-generated component accessors.
        /// </summary>
        /// <returns>The component instance, or null if this DynObject doesn't contain the component</returns>
        [DebuggerNonUserCode]
        public object GetComponent(Type componentType)
        {
            var offset = TypeInfo.GetComponentOffset(componentType);
            return offset.HasValue ? _Components[offset.Value] : null;
        }

        /// <summary>
        /// Integration point, for internal use.
        /// </summary>
        [DebuggerNonUserCode]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetComponent(int componentId)
		{
			var offset = TypeInfo.GetComponentOffset(componentId);
            return offset != TypeInfo.INVALID_COMPONENT_OFFSET ? _Components[offset] : null;
		}

        /// <summary>
        /// Check if this DynObject implements a specific message
        /// </summary>
        /// <param name="messageInterface">The type of the message interface. It's the same type as the ones implemented by components supporting the message.</param>
        /// <returns>true if this DynObject specifically implements the message (default implementations don't count), false otherwise</returns>
		public bool ImplementsMessage(Type messageInterface)
		{
			return TypeInfo.ImplementsMessage(messageInterface);
		}

        /// <summary>
        /// Changes this DynObject's component set.
        /// </summary>
        /// <param name="componentOperations">For every component to be removed, supply its type, e.g. typeof(MyComponent), or component.GetType().
        /// For every component to be added, supply an instance, e.g. new MyComponent()</param>
        public void Mutate(params object[] componentOperations)
        {
            Mutate(from newComponent in componentOperations
                   where !(newComponent is Type)
                   select newComponent,
                   from typeToRemove in componentOperations
                   where typeToRemove is Type
                   select typeToRemove as Type
                       );
        }

        /// <summary>
        /// Changes this DynObject's component set.
        /// </summary>
        /// <param name="componentsToAdd">The new components to add</param>
        /// <param name="componentsToRemove">The component types to remove. Each removed component is Dispose()'d</param>
		public void Mutate(IEnumerable<object> componentsToAdd, IEnumerable<Type> componentsToRemove)
		{
			if (componentsToRemove == null)
				componentsToRemove = new Type[0];
            if (componentsToAdd == null)
                componentsToAdd = new object[0];

			var newComponentSet =
				(from c in _Components.Concat(componentsToAdd)
				where c.GetType().GetAttribute<DefaultMessageImplementerAttribute>() == null && !componentsToRemove.Contains(c.GetType())
				select c)
				.ToArray();
			TypeInfo.NormalizeComponentTypeList(newComponentSet);

            var ti = TypeInfo.Factory.CreateObjectType(TypeInfo.TypesFromComponents(newComponentSet));
            var oldComponents = _Components;
            Construct(ti.CreateAllComponents(newComponentSet), ti);

            var removedComponents =
                from c in oldComponents
                where componentsToRemove.Contains(c.GetType())
                select c;
            foreach (var c in removedComponents)
            {
                var disposable = c as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }

		    var addedComponents = (from c in _Components
		                           where !oldComponents.Contains(c)
		                           select c).ToArray();

            FinishComponentConstruction(addedComponents);

            ForEachObjectHook(hook => hook.OnDynObjectMutated(addedComponents, componentsToRemove));
		}

        private void FinishComponentConstruction(IEnumerable<object> components)
        {
            ForEachObjectHook(hook=>hook.SetDynObject(this), components);
            ForEachObjectHook(hook=>hook.OnDynObjectCreated(), components);
        }

        internal void ForEachObjectHook(Action<IObjectHook> action)
        {
            ForEachObjectHook(action, _Components);
        }

        private static void ForEachObjectHook(Action<IObjectHook> action, IEnumerable<object> components)
        {
            foreach (var c in components)
            {
                var hook = c as IHasObjectHook;
                if (hook != null)
                    action(hook.ObjectHook);
            }
        }

		public string ToString(string separator)
		{
		    var userComponents = _Components.TakeWhile(c => c.GetType().GetAttribute<DefaultMessageImplementerAttribute>() == null);
            return String.Join(separator, (from c in userComponents select c.ToString()).ToArray());
		}

		public override string ToString()
		{
			return ToString("\n---\n");
		}

        /// <summary>
        /// Checks if this DynObjects contains the specified component, and adds a new instance if it doesn't.
        /// </summary>
        /// <param name="componentType"></param>
        public void EnsureComponent(Type componentType)
        {
            if (GetComponent(componentType) == null)
                Mutate(Activator.CreateInstance(componentType));

            Debug.Assert(GetComponent(componentType) != null);
        }

        /// <summary>
        /// Checks if this DynObjects contains the specified component, and adds the supplied instance if it doesn't.
        /// </summary>
        /// <param name="component"></param>
        public void EnsureComponent(object component)
        {
            if (component == null) throw new ArgumentNullException("component");
            if (GetComponent(component.GetType()) == null)
                Mutate(component);

            Debug.Assert(GetComponent(component.GetType()) != null);
        }

        /// <summary>
        /// Integration point, for internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
		public struct MulticastSlotCollection : IEnumerable<object>
		{
			public object[] myComponents;
			public int[] myImplementers;

			public MulticastSlotIterator GetEnumerator()
			{
				return new MulticastSlotIterator { myComponents = myComponents, myImplementers = myImplementers, myPosition = -1 };
			}

            IEnumerator<object> IEnumerable<object>.GetEnumerator()
			{
				return GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

        /// <summary>
        /// Integration point, for internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct MulticastSlotIterator : IEnumerator<object>
		{
            public object[] myComponents;
			public int[] myImplementers;
			public int myPosition;

            public object Current
			{
				get { return myComponents[myImplementers[myPosition]]; }
			}

			public void Dispose()
			{
			}

			object System.Collections.IEnumerator.Current
			{
				get { return Current; }
			}

			public bool MoveNext()
			{
				myPosition++;
				return myPosition < myImplementers.Length;
			}

			public void Reset()
			{
				myPosition = -1;
			}
		}

        /// <summary>
        /// Integration point, for internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
		public MulticastSlotCollection GetMulticastMessageImplementers(int msgId)
		{
			return new MulticastSlotCollection { myComponents = _Components, myImplementers = TypeInfo.GetMulticastMessageImplementers(msgId) };
		}

        /// <summary>
        /// Disposes all components that implement IDisposable
        /// </summary>
        public void Dispose()
        {
            foreach (var c in _Components)
            {
                var disposable = c as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
	}
}
