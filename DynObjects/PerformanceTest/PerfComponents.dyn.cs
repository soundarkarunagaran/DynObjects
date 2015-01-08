////////////////////////////////////////////////////////////////////////////
// Generated code, do not modify; instead, modify the source .dyn file.
// Source file: D:\Personal\Projects\DynObjectsPresentation\DynObjects\PerformanceTest\PerfComponents.dyn
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace PerformanceTest
{
	[Dyn.DefaultMessageImplementer]
	public sealed partial class DefaultMessageImplementations
		: PerfComponentsMsgs.Multicast
		, PerfComponentsMsgs.Unicast
	{
		public void Multicast()
		{
			// multicast message
		}

		public void Unicast()
		{
			throw new Dyn.DynException("Unimplemented message: PerfComponentsMsgs.Unicast");
		}
	}

	public static class PerfComponentsMsgs
	{
		[Dyn.Message(Index = 0, Multicast = true, Delegate = typeof(PerfComponentsMsgsDelegates.Multicast))]
		public interface Multicast
		{
			void Multicast();
		}
		[Dyn.Message(Index = 1, Delegate = typeof(PerfComponentsMsgsDelegates.Unicast))]
		public interface Unicast
		{
			void Unicast();
		}

		public static int _Offset;
	}

	public static class PerfComponentsMsgsDelegates
	{
		public delegate void Multicast(Dyn.DynObject _obj);
		public delegate void Unicast(Dyn.DynObject _obj);
	}

	public static class PerfComponentsExtensions
	{
		[System.Diagnostics.DebuggerNonUserCode]
		public static void Multicast(this Dyn.DynObject _obj)
		{
			((PerfComponentsMsgsDelegates.Multicast)_obj._MessageImpl[PerfComponentsMsgs._Offset + 0])(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void Unicast(this Dyn.DynObject _obj)
		{
			((PerfComponentsMsgsDelegates.Unicast)_obj._MessageImpl[PerfComponentsMsgs._Offset + 1])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static void UnicastForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			((PerfComponentsMsgsDelegates.Unicast)_obj.TypeInfo.GetNextMessageBidderDelegate(PerfComponentsMsgs._Offset + 1, currentMessageImplementor))(_obj);
		}
	}

	public class Beacon : Dyn.Component
		, PerfComponentsMsgs.Multicast
	{
		public event PerfComponentsMsgsDelegates.Multicast MulticastBeacon;

		public void Multicast()
		{
			var evt = MulticastBeacon;
			if (evt != null)
				evt(DynThis);
		}
	}

	public sealed class PerfComponents : Dyn.ComponentProvider
	{
		public PerfComponents()
		{
			MessageCount = 2;
			ComponentAccessors = typeof(PerfComponentsAccessors);
			MessageExtensions = typeof(PerfComponentsMsgs);
			MessageDelegates = typeof(PerfComponentsMsgsDelegates);
			DefaultMsgImpls = new[] { new DefaultMessageImplementations() };

			Types = new System.Type[]
				{
					typeof(Beacon),
					typeof(Perf),
					typeof(Perf2),
					typeof(Perf3),
				};
		}
	}

	public static class PerfComponentsAccessors
	{
		public static int _BeaconOffset = 0;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Beacon GetBeacon(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_BeaconOffset) as Beacon;
		}

		public static int _PerfOffset = 1;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Perf GetPerf(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_PerfOffset) as Perf;
		}

		public static int _Perf2Offset = 2;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Perf2 GetPerf2(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_Perf2Offset) as Perf2;
		}

		public static int _Perf3Offset = 3;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Perf3 GetPerf3(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_Perf3Offset) as Perf3;
		}
	}
}