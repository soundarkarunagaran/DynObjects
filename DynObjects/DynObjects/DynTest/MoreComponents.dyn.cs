////////////////////////////////////////////////////////////////////////////
// This file is part of the DynObjects library's test suite. Copyright Stefan Dragnev 2010
////////////////////////////////////////////////////////////////////////////
// Generated code, do not modify; instead, modify the source .dyn file.
// Source file: C:\Users\aeris\Documents\ProjectDungeons\Source\Libraries\DynObjects\DynObjects\DynTest\MoreComponents.dyn
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace DynTest.More
{
	[Dyn.DefaultMessageImplementer]
	public sealed partial class DefaultMessageImplementations
		: MoreMsgs.Browse
		, MoreMsgs.DoSomething
		, MoreMsgs.Show
	{
		public void Browse()
		{
			throw new Dyn.DynException("Unimplemented message: MoreMsgs.Browse");
		}

		//public void DoSomething()
		//{
		//}

		public void Show()
		{
			throw new Dyn.DynException("Unimplemented message: MoreMsgs.Show");
		}
	}

	public static class MoreMsgs
	{
		[Dyn.Message(Index = 0, Delegate = typeof(MoreMsgsDelegates.Browse))]
		public interface Browse
		{
			void Browse();
		}
		[Dyn.Message(Index = 1, Delegate = typeof(MoreMsgsDelegates.DoSomething))]
		public interface DoSomething
		{
			void DoSomething();
		}
		[Dyn.Message(Index = 2, Delegate = typeof(MoreMsgsDelegates.Show))]
		public interface Show
		{
			void Show();
		}

		public static int _Offset;
	}

	public static class MoreMsgsDelegates
	{
		public delegate void Browse(Dyn.DynObject _obj);
		public delegate void DoSomething(Dyn.DynObject _obj);
		public delegate void Show(Dyn.DynObject _obj);
	}

	public static class MoreComponentsMessages
	{
		[System.Diagnostics.DebuggerNonUserCode]
		public static void Browse(this Dyn.DynObject _obj)
		{
			((MoreMsgsDelegates.Browse)_obj._MessageImpl[MoreMsgs._Offset + 0])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static void BrowseForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			((MoreMsgsDelegates.Browse)_obj.TypeInfo.GetNextMessageBidderDelegate(MoreMsgs._Offset + 0, currentMessageImplementor))(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void DoSomething(this Dyn.DynObject _obj)
		{
			((MoreMsgsDelegates.DoSomething)_obj._MessageImpl[MoreMsgs._Offset + 1])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static void DoSomethingForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			((MoreMsgsDelegates.DoSomething)_obj.TypeInfo.GetNextMessageBidderDelegate(MoreMsgs._Offset + 1, currentMessageImplementor))(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void Show(this Dyn.DynObject _obj)
		{
			((MoreMsgsDelegates.Show)_obj._MessageImpl[MoreMsgs._Offset + 2])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static void ShowForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			((MoreMsgsDelegates.Show)_obj.TypeInfo.GetNextMessageBidderDelegate(MoreMsgs._Offset + 2, currentMessageImplementor))(_obj);
		}
	}

	public sealed class MoreComponents : Dyn.ComponentProvider
	{
		public MoreComponents()
		{
			MessageCount = 3;
			ComponentAccessors = typeof(MoreComponentsAccessors);
			MessageExtensions = typeof(MoreMsgs);
			MessageDelegates = typeof(MoreMsgsDelegates);
			DefaultMsgImpls = new[] { new DefaultMessageImplementations() };

			Types = new System.Type[]
				{
					typeof(Something),
					typeof(View),
					typeof(Web),
				};
		}
	}

	public static class MoreComponentsAccessors
	{
		public static int _SomethingOffset = 0;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Something GetSomething(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_SomethingOffset) as Something;
		}

		public static int _ViewOffset = 1;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static View GetView(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_ViewOffset) as View;
		}

		public static int _WebOffset = 2;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Web GetWeb(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_WebOffset) as Web;
		}
	}
}