using System;
using System.Runtime.Serialization;

namespace Dyn.Utilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class CustomSerializationAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DelegatesSerializationAttribute : Attribute
    {
        public DelegatesSerializationAttribute(Type mementoType)
        {
            Memento = mementoType;

            var mementoAttr = Memento.GetAttribute<MementoAttribute>();
            if (mementoAttr == null)
                throw new SerializationException(String.Format("The associated memento type {0} must have the MementoAttribute", mementoType));
        }
        
        public Type Memento;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MementoAttribute : Attribute
    {
        public MementoAttribute(Type of)
        {
            Of = of;
            if (!Of.ImplementsInterface(typeof(ICustomSerializable)))
                throw new SerializationException(String.Format("The associated type {0} for the memento must implement ICustomSerializable", of));
        }
        
        public Type Of;
    }

    public interface ICustomSerializable
    {
        object ToSerializable();
        void FromSerializable(object memento, object context);
    }
}
