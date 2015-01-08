////////////////////////////////////////////////////////////////////////////
// Generated code, do not modify; instead, modify the source .dyn file.
// Source file: C:\Projects\DynObjectsPresentation\DynObjectsPresentation\Components\Components.dyn
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Dyn;

namespace DynObjectsPresentation
{
	[Dyn.DefaultMessageImplementer]
	public sealed partial class DefaultMessageImplementations
		: ComponentMsgs.AddedToWorld
		, ComponentMsgs.GetComposer
		, ComponentMsgs.GetPosition
		, ComponentMsgs.GetSize
		, ComponentMsgs.GetWorld
		, ComponentMsgs.Hit
		, ComponentMsgs.OnDead
		, ComponentMsgs.PositionChanged
		, ComponentMsgs.SetPosition
	{
		public void AddedToWorld(World world)
		{
			// multicast message
		}

		public Canvas GetComposer()
		{
			throw new Dyn.DynException("Unimplemented message: ComponentMsgs.GetComposer");
		}

		public Point GetPosition()
		{
			throw new Dyn.DynException("Unimplemented message: ComponentMsgs.GetPosition");
		}

		public Size GetSize()
		{
			throw new Dyn.DynException("Unimplemented message: ComponentMsgs.GetSize");
		}

		public World GetWorld()
		{
			throw new Dyn.DynException("Unimplemented message: ComponentMsgs.GetWorld");
		}

		//public bool Hit(DynObject byWhom)
		//{
		//}

		public void OnDead()
		{
			// multicast message
		}

		public void PositionChanged(Point newPosition)
		{
			// multicast message
		}

		public void SetPosition(Point newPosition)
		{
			throw new Dyn.DynException("Unimplemented message: ComponentMsgs.SetPosition");
		}
	}

	public static class ComponentMsgs
	{
		[Dyn.Message(Index = 0, Multicast = true, Delegate = typeof(ComponentMsgsDelegates.AddedToWorld))]
		public interface AddedToWorld
		{
			void AddedToWorld(World world);
		}
		[Dyn.Message(Index = 1, Delegate = typeof(ComponentMsgsDelegates.GetComposer))]
		public interface GetComposer
		{
			Canvas GetComposer();
		}
		[Dyn.Message(Index = 2, Delegate = typeof(ComponentMsgsDelegates.GetPosition))]
		public interface GetPosition
		{
			Point GetPosition();
		}
		[Dyn.Message(Index = 3, Delegate = typeof(ComponentMsgsDelegates.GetSize))]
		public interface GetSize
		{
			Size GetSize();
		}
		[Dyn.Message(Index = 4, Delegate = typeof(ComponentMsgsDelegates.GetWorld))]
		public interface GetWorld
		{
			World GetWorld();
		}
		[Dyn.Message(Index = 5, Delegate = typeof(ComponentMsgsDelegates.Hit))]
		public interface Hit
		{
			bool Hit(DynObject byWhom);
		}
		[Dyn.Message(Index = 6, Multicast = true, Delegate = typeof(ComponentMsgsDelegates.OnDead))]
		public interface OnDead
		{
			void OnDead();
		}
		[Dyn.Message(Index = 7, Multicast = true, Delegate = typeof(ComponentMsgsDelegates.PositionChanged))]
		public interface PositionChanged
		{
			void PositionChanged(Point newPosition);
		}
		[Dyn.Message(Index = 8, Delegate = typeof(ComponentMsgsDelegates.SetPosition))]
		public interface SetPosition
		{
			void SetPosition(Point newPosition);
		}

		public static int _Offset;
	}

	public static class ComponentMsgsDelegates
	{
		public delegate void AddedToWorld(Dyn.DynObject _obj, World world);
		public delegate Canvas GetComposer(Dyn.DynObject _obj);
		public delegate Point GetPosition(Dyn.DynObject _obj);
		public delegate Size GetSize(Dyn.DynObject _obj);
		public delegate World GetWorld(Dyn.DynObject _obj);
		public delegate bool Hit(Dyn.DynObject _obj, DynObject byWhom);
		public delegate void OnDead(Dyn.DynObject _obj);
		public delegate void PositionChanged(Dyn.DynObject _obj, Point newPosition);
		public delegate void SetPosition(Dyn.DynObject _obj, Point newPosition);
	}

	public static class ComponentsExtensions
	{
		[System.Diagnostics.DebuggerNonUserCode]
		public static void AddedToWorld(this Dyn.DynObject _obj, World world)
		{
			((ComponentMsgsDelegates.AddedToWorld)_obj._MessageImpl[ComponentMsgs._Offset + 0])(_obj, world);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static Canvas GetComposer(this Dyn.DynObject _obj)
		{
			return ((ComponentMsgsDelegates.GetComposer)_obj._MessageImpl[ComponentMsgs._Offset + 1])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static Canvas GetComposerForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			return ((ComponentMsgsDelegates.GetComposer)_obj.TypeInfo.GetNextMessageBidderDelegate(ComponentMsgs._Offset + 1, currentMessageImplementor))(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static Point GetPosition(this Dyn.DynObject _obj)
		{
			return ((ComponentMsgsDelegates.GetPosition)_obj._MessageImpl[ComponentMsgs._Offset + 2])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static Point GetPositionForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			return ((ComponentMsgsDelegates.GetPosition)_obj.TypeInfo.GetNextMessageBidderDelegate(ComponentMsgs._Offset + 2, currentMessageImplementor))(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static Size GetSize(this Dyn.DynObject _obj)
		{
			return ((ComponentMsgsDelegates.GetSize)_obj._MessageImpl[ComponentMsgs._Offset + 3])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static Size GetSizeForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			return ((ComponentMsgsDelegates.GetSize)_obj.TypeInfo.GetNextMessageBidderDelegate(ComponentMsgs._Offset + 3, currentMessageImplementor))(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static World GetWorld(this Dyn.DynObject _obj)
		{
			return ((ComponentMsgsDelegates.GetWorld)_obj._MessageImpl[ComponentMsgs._Offset + 4])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static World GetWorldForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			return ((ComponentMsgsDelegates.GetWorld)_obj.TypeInfo.GetNextMessageBidderDelegate(ComponentMsgs._Offset + 4, currentMessageImplementor))(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static bool Hit(this Dyn.DynObject _obj, DynObject byWhom)
		{
			return ((ComponentMsgsDelegates.Hit)_obj._MessageImpl[ComponentMsgs._Offset + 5])(_obj, byWhom);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static bool HitForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor, DynObject byWhom)
		{
			return ((ComponentMsgsDelegates.Hit)_obj.TypeInfo.GetNextMessageBidderDelegate(ComponentMsgs._Offset + 5, currentMessageImplementor))(_obj, byWhom);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void OnDead(this Dyn.DynObject _obj)
		{
			((ComponentMsgsDelegates.OnDead)_obj._MessageImpl[ComponentMsgs._Offset + 6])(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void PositionChanged(this Dyn.DynObject _obj, Point newPosition)
		{
			((ComponentMsgsDelegates.PositionChanged)_obj._MessageImpl[ComponentMsgs._Offset + 7])(_obj, newPosition);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void SetPosition(this Dyn.DynObject _obj, Point newPosition)
		{
			((ComponentMsgsDelegates.SetPosition)_obj._MessageImpl[ComponentMsgs._Offset + 8])(_obj, newPosition);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static void SetPositionForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor, Point newPosition)
		{
			((ComponentMsgsDelegates.SetPosition)_obj.TypeInfo.GetNextMessageBidderDelegate(ComponentMsgs._Offset + 8, currentMessageImplementor))(_obj, newPosition);
		}
	}

	public class Beacon : Dyn.Component
		, ComponentMsgs.OnDead
	{
		public event ComponentMsgsDelegates.OnDead OnDeadBeacon;

		public void OnDead()
		{
			var evt = OnDeadBeacon;
			if (evt != null)
				evt(DynThis);
		}
	}

	public sealed class Components : Dyn.ComponentProvider
	{
		public Components()
		{
			MessageCount = 9;
			ComponentAccessors = typeof(ComponentsAccessors);
			MessageExtensions = typeof(ComponentMsgs);
			MessageDelegates = typeof(ComponentMsgsDelegates);
			DefaultMsgImpls = new[] { new DefaultMessageImplementations() };

			Types = new System.Type[]
				{
					typeof(Beacon),
					typeof(HealthFeature),
					typeof(Missile),
					typeof(MonsterFeature),
					typeof(MotionFeature),
					typeof(Player),
					typeof(VisualFeature),
					typeof(WorldObject),
				};
		}
	}

	public static class ComponentsAccessors
	{
		public static int _BeaconOffset = 0;
		[System.Diagnostics.DebuggerNonUserCode]
		public static Beacon GetBeacon(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_BeaconOffset) as Beacon;
		}

		public static int _HealthFeatureOffset = 1;
		[System.Diagnostics.DebuggerNonUserCode]
		public static HealthFeature GetHealthFeature(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_HealthFeatureOffset) as HealthFeature;
		}

		public static int _MissileOffset = 2;
		[System.Diagnostics.DebuggerNonUserCode]
		public static Missile GetMissile(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_MissileOffset) as Missile;
		}

		public static int _MonsterFeatureOffset = 3;
		[System.Diagnostics.DebuggerNonUserCode]
		public static MonsterFeature GetMonsterFeature(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_MonsterFeatureOffset) as MonsterFeature;
		}

		public static int _MotionFeatureOffset = 4;
		[System.Diagnostics.DebuggerNonUserCode]
		public static MotionFeature GetMotionFeature(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_MotionFeatureOffset) as MotionFeature;
		}

		public static int _PlayerOffset = 5;
		[System.Diagnostics.DebuggerNonUserCode]
		public static Player GetPlayer(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_PlayerOffset) as Player;
		}

		public static int _VisualFeatureOffset = 6;
		[System.Diagnostics.DebuggerNonUserCode]
		public static VisualFeature GetVisualFeature(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_VisualFeatureOffset) as VisualFeature;
		}

		public static int _WorldObjectOffset = 7;
		[System.Diagnostics.DebuggerNonUserCode]
		public static WorldObject GetWorldObject(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_WorldObjectOffset) as WorldObject;
		}
	}
}