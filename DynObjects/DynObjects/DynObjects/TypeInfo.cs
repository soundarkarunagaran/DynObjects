// This file is part of the DynObjects library.
// Copyright 2010 Stefan Dragnev.
// The library and all its files are distributed under the MIT license.
// See the full text of the license in the accompanying LICENSE.txt file or at http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using Dyn.Utilities;

namespace Dyn
{
    /// <summary>
    /// Integration point. For internal use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class MessageAttribute : Attribute
    {
        public int Index { get; set; }
        public bool Multicast { get; set; }
        public bool ThruInterface { get; set; }
        public Type Delegate { get; set; }
    }

    /// <summary>
    /// Decorate message implementations with this attribute. Only components with the highest bid
    /// will have their implementation called. Unicast messages must have only one highest bidder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class BidAttribute : Attribute
    {
        public const int DefaultImplBid = int.MinValue;

        /// <summary>
        /// The bid. The highest bid among all components in a given DynObject wins.
        /// </summary>
        public int Value { get; private set; }

        public BidAttribute(int value)
        {
            Value = value;
            if (Value == DefaultImplBid)
                throw new DynException("int.MinValue cannot be used as a message bid.");
        }
    }

    /// <summary>
    /// Decorate multicast message implementations with this attribute. The higher the priority
    /// of a message, the earlier it will be called. Useful when certain multicast message implementations
    /// must be called first or last.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class PriorityAttribute : Attribute
    {
        public int Value { get; private set; }

        public PriorityAttribute(int value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Represents a template from which DynObjects are made. Created from <see cref="DynFactory.CreateObjectType"/>.
    /// <see cref="DynObject"/> instances contain a reference to the template from which they were made.
    /// </summary>
    public sealed class TypeInfo
    {
        internal readonly int[] myComponentOffsets;
        internal readonly Type[] myComponentTypes;
        private readonly Dictionary<Type, int> myTypeLookup = new Dictionary<Type, int>();

        internal readonly int[] myUnicastImpl;
        internal readonly Dictionary<Type, int>[] myUnicastRemainingChain;
        private readonly int[][] myMulticastImpl;

        internal readonly int[] myDataContractComponentTypes;

        internal readonly Delegate[] myMessageDelegates;
        private readonly Dictionary<Type, Delegate>[] myRemainingChainDelegates;

        private const int INVALID_MESSAGE_IMPL = -1;
        internal const int INVALID_COMPONENT_OFFSET = -1;

        private int ComponentsCount { get { return myTypeLookup.Count; } }
        private int DefaultImplComponentIndexStart { get { return ComponentsCount; } }

        /// <summary>
        /// The factory from which this template was made.
        /// </summary>
        public DynFactory Factory { get; private set; }

        /// <summary>
        /// Integration point. For internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerNonUserCode]
        public int GetComponentOffset(int componentId)
        {
            return myComponentOffsets[componentId];
        }

        internal int? GetComponentOffset(Type type)
        {
            int result;
            var found = myTypeLookup.TryGetValue(type, out result);
            return found ? result : default(int?);
        }

        internal static object[] NormalizeComponentTypeList(object[] components)
        {
            Array.Sort(components, (a, b) => StringComparer.Ordinal.Compare(a.GetType().FullName, b.GetType().FullName));
            return components;
        }

        internal static Type[] NormalizeComponentTypeList(Type[] components)
        {
            Array.Sort(components, (a, b) => StringComparer.Ordinal.Compare(a.FullName, b.FullName));
            return components;
        }

        internal static Type[] TypesFromComponents(object[] components)
        {
            return (from c in components select c.GetType()).ToArray();
        }

        internal static Type[] TypesOfParameters(MethodInfo method)
        {
            return (from p in method.GetParameters() select p.ParameterType).ToArray();
        }

        private struct CallSlot
        {
            public int ComponentIndex;
            public int Bid;
            public int Priority;
            public Delegate Dispatcher;
            public Type ComponentType;
        }

        internal static int CalculateMessageIndex(Type msgIntf)
        {
            var baseIndex = msgIntf.GetAttribute<MessageAttribute>().Index;
            var offset = msgIntf.ReflectedType.GetField("_Offset").GetValue(null);
            return baseIndex + (int)offset;
        }

        internal TypeInfo(DynFactory factory, Type[] components)
        {
            // sanity checks
            for (var i = 1; i < components.Length; ++i)
                if (components[i - 1] == components[i])
                    throw new DynException("Duplicate component types used in object creation.");

            Factory = factory;

            // initialize component lookup structures
            myComponentTypes = new Type[components.Length];
            for (var i=0; i<components.Length; ++i)
            {
                var type = components[i];
                myTypeLookup[type] = i;
                myComponentTypes[i] = type;
            }

            var providedTypes = factory.myComponentProvider.Types;
            var compIdx = 0;
            var defaultMsgImpls = factory.myComponentProvider.DefaultMsgImpls;
            myComponentOffsets = new int[providedTypes.Length + defaultMsgImpls.Length];
            for (int i=0; i<providedTypes.Length; ++i)
            {
                var offset = INVALID_COMPONENT_OFFSET;
                if (compIdx < components.Length && components[compIdx] == providedTypes[i])
                {
                    offset = compIdx;
                    compIdx++;
                }
                myComponentOffsets[i] = offset;
            }
            if (compIdx != components.Length)
            {
                throw new DynException("Invalid components submitted to factory. Check that the used components are all present in the factory's component provider.");
            }
            for (int i=0; i<defaultMsgImpls.Length; ++i)
            {
                myComponentOffsets[providedTypes.Length + i] = components.Length + i;
            }

            // initialize message slots
            int messageCount = factory.myComponentProvider.MessageCount;
            var messageDispatchers = factory.myComponentProvider.DispatcherDelegates;
            var unicastImpl = new List<CallSlot>[messageCount];
            var multicastImpl = new List<CallSlot>[messageCount];
            for (int i = 0; i < messageCount; ++i)
            {
                unicastImpl[i] = new List<CallSlot>(1);
                multicastImpl[i] = new List<CallSlot>(1);
            }

            foreach (var implEnum in defaultMsgImpls.Enumerate())
            {
                var defaultMsgImpl = implEnum.Value;
                var defaultImplComponentIdx = components.Length + implEnum.Index;
                foreach (var intf in defaultMsgImpl.GetType().GetInterfaces())
                {
                    var attr = intf.GetAttribute<MessageAttribute>();
                    if (attr == null)
                        continue;

                    Delegate dispatcher = null;
                    if (!attr.ThruInterface)
                        dispatcher = messageDispatchers[Util.MakePair(defaultMsgImpl.GetType(), intf)];
                    var messageIdx = CalculateMessageIndex(intf);

                    var callSlot = new CallSlot
                    {
                        ComponentIndex = defaultImplComponentIdx,
                        Dispatcher = dispatcher,
                        Bid = BidAttribute.DefaultImplBid,
                        ComponentType = defaultMsgImpl.GetType(),
                    };
                    if (!attr.Multicast)
                    {
                        Debug.Assert(unicastImpl[messageIdx].Count == 0);
                        unicastImpl[messageIdx].Add(callSlot);
                    }
                    else
                    {
                        multicastImpl[messageIdx].Add(callSlot);
                    }
                }
            }
            
            foreach (var type in components)
            {
                var implementedMessages =
                    from intf in type.GetInterfaces()
                    let msgAttr = intf.GetAttribute<MessageAttribute>()
                    where msgAttr != null
                    select new { Interface = intf, MessageAttr = msgAttr };

                foreach (var msg in implementedMessages)
                {
                    var idx = CalculateMessageIndex(msg.Interface);

                    var implMethod = type.GetInterfaceMap(msg.Interface).TargetMethods[0];
                    var bidAttr = implMethod.GetAttribute<BidAttribute>();
                    var priorityAttr = implMethod.GetAttribute<PriorityAttribute>();

                    var newSlot = new CallSlot {
                        ComponentIndex = GetComponentOffset(type).Value,
                        Bid = bidAttr != null ? bidAttr.Value : 0,
                        Priority = priorityAttr != null ? priorityAttr.Value : 0,
                        Dispatcher = msg.MessageAttr.ThruInterface ? null : messageDispatchers[Util.MakePair(type, msg.Interface)],
                        ComponentType = type,
                    };

                    if (!msg.MessageAttr.Multicast)
                    {
                        if (priorityAttr != null)
                            throw new DynException("Unicast messages cannot have a priority attribute.");
                        
                        if (unicastImpl[idx].Count >= 1)
                        {
                            var bids = from cs in unicastImpl[idx]
                                       where cs.Bid == newSlot.Bid
                                       select cs;
                            if (bids.Count() != 0)
                            {
                                var oldSlot = bids.Single();
                                throw new DynException(String.Format(CultureInfo.InvariantCulture, "Unicast messages can't have the same bid: {0} in components {1} and {2}.",
                                    msg.Interface.FullName, myComponentTypes[oldSlot.ComponentIndex], myComponentTypes[newSlot.ComponentIndex]));
                            }
                        }

                        unicastImpl[idx].Add(newSlot);
                    }
                    else
                    {
                        if (multicastImpl[idx].Count > 0)
                        {
                            if (multicastImpl[idx][0].ComponentIndex >= DefaultImplComponentIndexStart)
                            {
                                multicastImpl[idx].Clear();
                            }
                            else
                            {
                                var curBid = multicastImpl[idx][0].Bid;
                                if (curBid > newSlot.Bid)
                                    continue;
                                if (curBid < newSlot.Bid)
                                    multicastImpl[idx].Clear();
                            }
                        }

                        multicastImpl[idx].Add(newSlot);
                    }
                }
            }

            myUnicastImpl = Enumerable.Repeat(INVALID_MESSAGE_IMPL, messageCount).ToArray();
            myUnicastRemainingChain = new Dictionary<Type, int>[messageCount];
            myMulticastImpl = new int[messageCount][];
            myMessageDelegates = new Delegate[messageCount];
            myRemainingChainDelegates = new Dictionary<Type, Delegate>[messageCount];
            for (int i = 0; i < messageCount; ++i)
            {
                myMulticastImpl[i] =
                    (from slot in multicastImpl[i]
                     orderby slot.Priority descending
                     select slot.ComponentIndex).ToArray();

                var unicasters = from slot in unicastImpl[i]
                                 orderby slot.Bid descending
                                 select new {Index = slot.ComponentIndex, slot.Dispatcher, slot.ComponentType};

                if (myMulticastImpl[i].Length == 0)
                {
                    myUnicastImpl[i] = unicasters.First().Index;
                    myUnicastRemainingChain[i] = 
                        (from u in unicasters.Skip(1)
                         select new {u.ComponentType, u.Index})
                         .ToDictionary(p=>p.ComponentType, p=>p.Index);
                }

                if (unicastImpl[i].Count > 0)
                {
                    myMessageDelegates[i] = unicasters.First().Dispatcher;
                    myRemainingChainDelegates[i] =
                        (from u in unicasters.Skip(1)
                         select new {u.ComponentType, u.Dispatcher})
                         .ToDictionary(p => p.ComponentType, p => p.Dispatcher);
                }
                else
                {
                    myMessageDelegates[i] = Util.CombineDelegates(
                        (from slot in multicastImpl[i]
                         orderby slot.Priority descending
                         select slot.Dispatcher
                         ).ToArray());
                }
            }

            // initialize serialization structures
            var dataContractComponents =
                from pair in components.Enumerate()
                let compType = pair.Value
                where compType.GetAttribute<SerializedComponentAttribute>() != null
                    || compType.ImplementsInterface(typeof(ICustomSerializable))
                select pair.Index;
            myDataContractComponentTypes = dataContractComponents.ToArray();
        }

        internal object[] CreateAllComponents(object[] existingComponents)
        {
            var defaultImpls = Factory.myComponentProvider.DefaultMsgImpls;
            var internalComponents = new object[ComponentsCount + defaultImpls.Length];
            Array.Copy(defaultImpls, 0, internalComponents, ComponentsCount, defaultImpls.Length);
            for (int i = 0; i < ComponentsCount; ++i)
            {
                var compType = myComponentTypes[i];
                var result = (from comp in existingComponents
                              where comp.GetType() == compType
                              select comp).SingleOrDefault();

                internalComponents[i] = result ?? Activator.CreateInstance(compType);
            }
            return internalComponents;
        }

        /// <summary>
        /// Creates an object with this typeinfo and uses the given components. If any component types are missing, they're then default-constructed.
        /// </summary>
        /// <param name="components">Components with which to populate the object</param>
        /// <returns>The new object</returns>
        public DynObject CreateObject(params object[] components)
        {
            return new DynObject(CreateAllComponents(components), this);
        }

        /// <summary>
        /// Integration point. For internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int[] GetMulticastMessageImplementers(int messageIndex)
        {
            return myMulticastImpl[messageIndex];
        }

        /// <summary>
        /// Integration point. For internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Delegate GetNextMessageBidderDelegate(int messageIndex, Type currentMessageImplementer)
        {
            Delegate messageDispatcher;
            myRemainingChainDelegates[messageIndex].TryGetValue(currentMessageImplementer, out messageDispatcher);
            if (messageDispatcher == null)
                throw new DynException("There is no next message bidder.");
            return messageDispatcher;
        }

        /// <summary>
        /// Integration point. For internal use.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int GetNextMessageBidderInterface(int messageIndex, Type currentMessageImplementer)
        {
            int implComponent;
            var hasNext = myUnicastRemainingChain[messageIndex].TryGetValue(currentMessageImplementer, out implComponent);
            if (!hasNext)
                throw new DynException("There is no next message bidder.");
            return implComponent;
        }

        internal bool ImplementsMessage(Type messageIntf)
        {
            var msg = messageIntf.GetAttribute<MessageAttribute>();
            if (msg == null)
                throw new DynException("Supplied type isn't a message.");

            var idx = CalculateMessageIndex(messageIntf);
            if (msg.Multicast)
            {
                var impls = myMulticastImpl[idx];
                return (impls.Length > 0 && impls[0] < DefaultImplComponentIndexStart);
            }
            else
            {
                return (myUnicastImpl[idx] != INVALID_MESSAGE_IMPL && myUnicastImpl[idx] < DefaultImplComponentIndexStart);
            }
        }
    }
}