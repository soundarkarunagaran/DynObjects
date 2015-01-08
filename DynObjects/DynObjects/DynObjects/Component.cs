// This file is part of the DynObjects library.
// Copyright 2010 Stefan Dragnev.
// The library and all its files are distributed under the MIT license.
// See the full text of the license in the accompanying LICENSE.txt file or at http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dyn
{
    /// <summary>
    /// Components should implement this interface to for full integration with their containing DynObject.
    /// It is usually easier though to just use the <see cref="Component"/> class.
    /// </summary>
    public interface IHasObjectHook
    {
        IObjectHook ObjectHook { get; }
    }

    /// <summary>
    /// Components should implement this interface to for full integration with their containing DynObject.
    /// The library calls the interface methods to notify the component of events occurring on the DynObject level.
    /// It is usually easier though to just use the <see cref="Component"/> class.
    /// </summary>
    public interface IObjectHook
    {
        /// <summary>
        /// Integration point. For internal use.
        /// </summary>
        void SetDynObject(DynObject obj);

        /// <summary>
        /// Called when all components in a DynObject have been created. Also called for newly added components after a Mutate() operation.
        /// </summary>
        void OnDynObjectCreated();

        /// <summary>
        /// Called when the components in a DynObject have changed after a <see cref="DynObject.Mutate(object[])"/>. This is only called
        /// for the remaining components in the DynObject, i.e. newly added and unchanged. It's not called for removed components - they
        /// get Dispose()'d instead.
        /// </summary>
        /// <param name="componentsAdded">A list of the new components</param>
        /// <param name="componentsRemoved"></param>
        void OnDynObjectMutated(IEnumerable<object> componentsAdded, IEnumerable<Type> componentsRemoved);
    }

    /// <summary>
    /// <see cref="IObjectHook.OnDynObjectMutated"/>
    /// </summary>
    public class DynObjectMutatedEventArgs : EventArgs
    {
        public IEnumerable<object> ComponentsAdded { get; internal set; }
        public IEnumerable<Type> ComponentsRemoved { get; internal set; }
    }

    /// <summary>
    /// A common way to integrate components is to derive your component classes from Component.
    /// This way you get direct access to the containing DynObject through the DynThis property
    /// and can also override OnDynObjectCreated and OnDynObjectMutated. If for any reason you
    /// can't inherit from Component, then you can store an instance of component as a field and
    /// provide access to the library by implementing <see cref="IHasObjectHook"/>. You will then
    /// have access to the containing DynObject through your Component field. In order to process
    /// OnDynObjectCreated and OnDynObjectMutated calls, you will have to subscribe to the
    /// DynObjectCreated and DynObjectMutated events in it.
    /// </summary>
    public class Component : IObjectHook, IHasObjectHook
    {
        IObjectHook IHasObjectHook.ObjectHook
        {
            get { return this; }
        }

        /// <summary>
        /// The DynObject instance containing this component.
        /// </summary>
        [XmlIgnore]
        public DynObject DynThis { get; private set; }

        /// <summary>
        /// Integration point. For internal use.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        void IObjectHook.SetDynObject(DynObject obj)
        {
            DynThis = obj;
        }

        /// <summary>
        /// Emitted when OnDynObjectCreated is called by the library, unless overridden. See <see cref="IObjectHook.OnDynObjectCreated"/>
        /// </summary>
        public event EventHandler DynObjectCreated;

        /// <summary>
        /// Emitted when OnDynObjectMutated is called by the library, unless overridden. See <see cref="IObjectHook.OnDynObjectMutated"/>
        /// </summary>
        public event EventHandler<DynObjectMutatedEventArgs> DynObjectMutated;

        /// <summary>
        /// Override to intercept this event. The base implementation emits the DynObjectCreated event.
        /// See <see cref="IObjectHook.OnDynObjectCreated"/>
        /// </summary>
        public virtual void OnDynObjectCreated()
        {
            var evt = DynObjectCreated;
            if (evt != null)
                evt(this, EventArgs.Empty);
        }

        /// <summary>
        /// Override to intercept this event. The base implementation emits the DynObjectMutated event.
        /// See <see cref="IObjectHook.OnDynObjectMutated"/>
        /// </summary>
        public virtual void OnDynObjectMutated(IEnumerable<object> componentsAdded, IEnumerable<Type> componentsRemoved)
        {
            var evt = DynObjectMutated;
            if (evt != null)
            {
                var mutatedEventArgs = new DynObjectMutatedEventArgs { ComponentsAdded = componentsAdded, ComponentsRemoved = componentsRemoved};
                evt(this, mutatedEventArgs);
            }
        }
    }
}
