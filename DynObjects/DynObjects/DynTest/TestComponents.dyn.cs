////////////////////////////////////////////////////////////////////////////
// This file is part of the DynObjects library's test suite. Copyright Stefan Dragnev 2010
////////////////////////////////////////////////////////////////////////////
// Generated code, do not modify; instead, modify the source .dyn file.
// Source file: C:\Users\aeris\Documents\ProjectDungeons\Source\Libraries\DynObjects\DynObjects\DynTest\TestComponents.dyn
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dyn;

namespace DynTest
{
	[Dyn.DefaultMessageImplementer]
	public sealed partial class DefaultMessageImplementations
		: GameMsgs.Bar
		, GameMsgs.Baz
		, GameMsgs.Convert
		, GameMsgs.ElaborateSignature
		, GameMsgs.Foo
		, GameMsgs.Overloaded
		, GameMsgs.OverloadedFloat
		, GameMsgs.OverloadedInt
		, GameMsgs.SimpleGeneric
		, GameMsgs.TestMessage0
		, GameMsgs.TestMessage1
		, GameMsgs.TestMessageDefaultImpl
		, GameMsgs.TestMessageMulticast
		, GameMsgs.TestMessageMulticastDefaultImpl
		, GameMsgs.WithAttribute
	{
		public int Bar(int a, char b, string c)
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.Bar");
		}

		//public void Baz()
		//{
		//}

		public T Convert<T,U>(U from)
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.Convert");
		}

		public Func<int, List<int>, System.String> ElaborateSignature<T, U, SomeOtherParam>(ref Func<List<int>, Dictionary<int, U>> param1, out Func< Func<T> > param2, Func< List < SomeOtherParam >,Func< int >> param3) where T : class, IEnumerable<U> where U : new() where SomeOtherParam : struct, IEquatable<T>, IComparable<T>
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.ElaborateSignature");
		}

		public void Foo()
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.Foo");
		}

		public string Overloaded(string x)
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.Overloaded");
		}

		public float Overloaded(float x)
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.OverloadedFloat");
		}

		public int Overloaded(int x)
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.OverloadedInt");
		}

		public T SimpleGeneric<T, U>(T t, U u)
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.SimpleGeneric");
		}

		public void TestMessage0()
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.TestMessage0");
		}

		public int TestMessage1(out bool result)
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.TestMessage1");
		}

		//public int TestMessageDefaultImpl()
		//{
		//}

		public void TestMessageMulticast(ref List<string> result)
		{
			// multicast message
		}

		//public void TestMessageMulticastDefaultImpl(ref int value)
		//{
		//}

		public void WithAttribute()
		{
			throw new Dyn.DynException("Unimplemented message: GameMsgs.WithAttribute");
		}
	}

	public static class GameMsgs
	{
		[Dyn.Message(Index = 0, Delegate = typeof(GameMsgsDelegates.Bar))]
		public interface Bar
		{
			int Bar(int a, char b, string c);
		}
		[Dyn.Message(Index = 1, Multicast = true, Delegate = typeof(GameMsgsDelegates.Baz))]
		public interface Baz
		{
			void Baz();
		}
		[Dyn.Message(Index = 2, ThruInterface = true)]
		public interface Convert
		{
			T Convert<T,U>(U from);
		}
		[Dyn.Message(Index = 3, ThruInterface = true)]
		public interface ElaborateSignature
		{
			Func<int, List<int>, System.String> ElaborateSignature<T, U, SomeOtherParam>(ref Func<List<int>, Dictionary<int, U>> param1, out Func< Func<T> > param2, Func< List < SomeOtherParam >,Func< int >> param3) where T : class, IEnumerable<U> where U : new() where SomeOtherParam : struct, IEquatable<T>, IComparable<T>;
		}
		[Dyn.Message(Index = 4, Delegate = typeof(GameMsgsDelegates.Foo))]
		public interface Foo
		{
			void Foo();
		}
		[Dyn.Message(Index = 5, Delegate = typeof(GameMsgsDelegates.Overloaded))]
		public interface Overloaded
		{
			string Overloaded(string x);
		}
		[Dyn.Message(Index = 6, Delegate = typeof(GameMsgsDelegates.OverloadedFloat))]
		public interface OverloadedFloat
		{
			float Overloaded(float x);
		}
		[Dyn.Message(Index = 7, Delegate = typeof(GameMsgsDelegates.OverloadedInt))]
		public interface OverloadedInt
		{
			int Overloaded(int x);
		}
		[Dyn.Message(Index = 8, ThruInterface = true)]
		public interface SimpleGeneric
		{
			T SimpleGeneric<T, U>(T t, U u);
		}
		[Dyn.Message(Index = 9, Delegate = typeof(GameMsgsDelegates.TestMessage0))]
		public interface TestMessage0
		{
			void TestMessage0();
		}
		[Dyn.Message(Index = 10, Delegate = typeof(GameMsgsDelegates.TestMessage1))]
		public interface TestMessage1
		{
			int TestMessage1(out bool result);
		}
		[Dyn.Message(Index = 11, Delegate = typeof(GameMsgsDelegates.TestMessageDefaultImpl))]
		public interface TestMessageDefaultImpl
		{
			int TestMessageDefaultImpl();
		}
		[Dyn.Message(Index = 12, Multicast = true, Delegate = typeof(GameMsgsDelegates.TestMessageMulticast))]
		public interface TestMessageMulticast
		{
			void TestMessageMulticast(ref List<string> result);
		}
		[Dyn.Message(Index = 13, Multicast = true, Delegate = typeof(GameMsgsDelegates.TestMessageMulticastDefaultImpl))]
		public interface TestMessageMulticastDefaultImpl
		{
			void TestMessageMulticastDefaultImpl(ref int value);
		}
		[Dyn.Message(Index = 14, Delegate = typeof(GameMsgsDelegates.WithAttribute))]
		public interface WithAttribute
		{
			void WithAttribute();
		}

		public static int _Offset;
	}

	public static class GameMsgsDelegates
	{
		public delegate int Bar(Dyn.DynObject _obj, int a, char b, string c);
		public delegate void Baz(Dyn.DynObject _obj);
		public delegate void Foo(Dyn.DynObject _obj);
		public delegate string Overloaded(Dyn.DynObject _obj, string x);
		public delegate float OverloadedFloat(Dyn.DynObject _obj, float x);
		public delegate int OverloadedInt(Dyn.DynObject _obj, int x);
		public delegate void TestMessage0(Dyn.DynObject _obj);
		public delegate int TestMessage1(Dyn.DynObject _obj, out bool result);
		public delegate int TestMessageDefaultImpl(Dyn.DynObject _obj);
		public delegate void TestMessageMulticast(Dyn.DynObject _obj, ref List<string> result);
		public delegate void TestMessageMulticastDefaultImpl(Dyn.DynObject _obj, ref int value);
		public delegate void WithAttribute(Dyn.DynObject _obj);
	}

	public static class GameComponentsMessages
	{
		[System.Diagnostics.DebuggerNonUserCode]
		public static int Bar(this Dyn.DynObject _obj, int a, char b, string c)
		{
			return ((GameMsgsDelegates.Bar)_obj._MessageImpl[GameMsgs._Offset + 0])(_obj, a, b, c);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static int BarForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor, int a, char b, string c)
		{
			return ((GameMsgsDelegates.Bar)_obj.TypeInfo.GetNextMessageBidderDelegate(GameMsgs._Offset + 0, currentMessageImplementor))(_obj, a, b, c);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void Baz(this Dyn.DynObject _obj)
		{
			((GameMsgsDelegates.Baz)_obj._MessageImpl[GameMsgs._Offset + 1])(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static T Convert<T,U>(this Dyn.DynObject _obj, U from)
		{
			return ((GameMsgs.Convert)_obj._Components[_obj._UnicastImpl[GameMsgs._Offset + 2]]).Convert<T,U>(from);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static T ConvertForNextBidder<T,U>(this Dyn.DynObject _obj, Type currentMessageImplementor, U from)
		{
			return ((GameMsgs.Convert)_obj._Components[_obj.TypeInfo.GetNextMessageBidderInterface(GameMsgs._Offset + 2, currentMessageImplementor)]).Convert<T,U>(from);
		}

		[EditorBrowsable(), TestAttribute(Values = new[] { 3,5,7 }, SomeOtherValues = "Don't put brackets here...")]
		[System.Diagnostics.Contracts.Pure]
		[System.Diagnostics.DebuggerNonUserCode]
		public static Func<int, List<int>, System.String> ElaborateSignature<T, U, SomeOtherParam>(this Dyn.DynObject _obj, ref Func<List<int>, Dictionary<int, U>> param1, out Func< Func<T> > param2, Func< List < SomeOtherParam >,Func< int >> param3) where T : class, IEnumerable<U> where U : new() where SomeOtherParam : struct, IEquatable<T>, IComparable<T>
		{
			return ((GameMsgs.ElaborateSignature)_obj._Components[_obj._UnicastImpl[GameMsgs._Offset + 3]]).ElaborateSignature<T, U, SomeOtherParam>(ref param1, out param2, param3);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static Func<int, List<int>, System.String> ElaborateSignatureForNextBidder<T, U, SomeOtherParam>(this Dyn.DynObject _obj, Type currentMessageImplementor, ref Func<List<int>, Dictionary<int, U>> param1, out Func< Func<T> > param2, Func< List < SomeOtherParam >,Func< int >> param3) where T : class, IEnumerable<U> where U : new() where SomeOtherParam : struct, IEquatable<T>, IComparable<T>
		{
			return ((GameMsgs.ElaborateSignature)_obj._Components[_obj.TypeInfo.GetNextMessageBidderInterface(GameMsgs._Offset + 3, currentMessageImplementor)]).ElaborateSignature<T, U, SomeOtherParam>(ref param1, out param2, param3);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void Foo(this Dyn.DynObject _obj)
		{
			((GameMsgsDelegates.Foo)_obj._MessageImpl[GameMsgs._Offset + 4])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static void FooForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			((GameMsgsDelegates.Foo)_obj.TypeInfo.GetNextMessageBidderDelegate(GameMsgs._Offset + 4, currentMessageImplementor))(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static string Overloaded(this Dyn.DynObject _obj, string x)
		{
			return ((GameMsgsDelegates.Overloaded)_obj._MessageImpl[GameMsgs._Offset + 5])(_obj, x);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static string OverloadedForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor, string x)
		{
			return ((GameMsgsDelegates.Overloaded)_obj.TypeInfo.GetNextMessageBidderDelegate(GameMsgs._Offset + 5, currentMessageImplementor))(_obj, x);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static float Overloaded(this Dyn.DynObject _obj, float x)
		{
			return ((GameMsgsDelegates.OverloadedFloat)_obj._MessageImpl[GameMsgs._Offset + 6])(_obj, x);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static float OverloadedForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor, float x)
		{
			return ((GameMsgsDelegates.OverloadedFloat)_obj.TypeInfo.GetNextMessageBidderDelegate(GameMsgs._Offset + 6, currentMessageImplementor))(_obj, x);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static int Overloaded(this Dyn.DynObject _obj, int x)
		{
			return ((GameMsgsDelegates.OverloadedInt)_obj._MessageImpl[GameMsgs._Offset + 7])(_obj, x);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static int OverloadedForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor, int x)
		{
			return ((GameMsgsDelegates.OverloadedInt)_obj.TypeInfo.GetNextMessageBidderDelegate(GameMsgs._Offset + 7, currentMessageImplementor))(_obj, x);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static T SimpleGeneric<T, U>(this Dyn.DynObject _obj, T t, U u)
		{
			return ((GameMsgs.SimpleGeneric)_obj._Components[_obj._UnicastImpl[GameMsgs._Offset + 8]]).SimpleGeneric<T, U>(t, u);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static T SimpleGenericForNextBidder<T, U>(this Dyn.DynObject _obj, Type currentMessageImplementor, T t, U u)
		{
			return ((GameMsgs.SimpleGeneric)_obj._Components[_obj.TypeInfo.GetNextMessageBidderInterface(GameMsgs._Offset + 8, currentMessageImplementor)]).SimpleGeneric<T, U>(t, u);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void TestMessage0(this Dyn.DynObject _obj)
		{
			((GameMsgsDelegates.TestMessage0)_obj._MessageImpl[GameMsgs._Offset + 9])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static void TestMessage0ForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			((GameMsgsDelegates.TestMessage0)_obj.TypeInfo.GetNextMessageBidderDelegate(GameMsgs._Offset + 9, currentMessageImplementor))(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static int TestMessage1(this Dyn.DynObject _obj, out bool result)
		{
			return ((GameMsgsDelegates.TestMessage1)_obj._MessageImpl[GameMsgs._Offset + 10])(_obj, out result);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static int TestMessage1ForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor, out bool result)
		{
			return ((GameMsgsDelegates.TestMessage1)_obj.TypeInfo.GetNextMessageBidderDelegate(GameMsgs._Offset + 10, currentMessageImplementor))(_obj, out result);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static int TestMessageDefaultImpl(this Dyn.DynObject _obj)
		{
			return ((GameMsgsDelegates.TestMessageDefaultImpl)_obj._MessageImpl[GameMsgs._Offset + 11])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static int TestMessageDefaultImplForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			return ((GameMsgsDelegates.TestMessageDefaultImpl)_obj.TypeInfo.GetNextMessageBidderDelegate(GameMsgs._Offset + 11, currentMessageImplementor))(_obj);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void TestMessageMulticast(this Dyn.DynObject _obj, ref List<string> result)
		{
			((GameMsgsDelegates.TestMessageMulticast)_obj._MessageImpl[GameMsgs._Offset + 12])(_obj, ref result);
		}

		[System.Diagnostics.DebuggerNonUserCode]
		public static void TestMessageMulticastDefaultImpl(this Dyn.DynObject _obj, ref int value)
		{
			((GameMsgsDelegates.TestMessageMulticastDefaultImpl)_obj._MessageImpl[GameMsgs._Offset + 13])(_obj, ref value);
		}

		[  CompilerGenerated()  ]
		[TestAttribute(Values = new[] { 3,5,7 })]
		[System.Diagnostics.DebuggerNonUserCode]
		public static void WithAttribute(this Dyn.DynObject _obj)
		{
			((GameMsgsDelegates.WithAttribute)_obj._MessageImpl[GameMsgs._Offset + 14])(_obj);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public static void WithAttributeForNextBidder(this Dyn.DynObject _obj, Type currentMessageImplementor)
		{
			((GameMsgsDelegates.WithAttribute)_obj.TypeInfo.GetNextMessageBidderDelegate(GameMsgs._Offset + 14, currentMessageImplementor))(_obj);
		}
	}

	public sealed class GameComponents : Dyn.ComponentProvider
	{
		public GameComponents()
		{
			MessageCount = 15;
			ComponentAccessors = typeof(GameComponentsAccessors);
			MessageExtensions = typeof(GameMsgs);
			MessageDelegates = typeof(GameMsgsDelegates);
			DefaultMsgImpls = new[] { new DefaultMessageImplementations() };

			Types = new System.Type[]
				{
					typeof(AnotherNamespace.NestedClass),
					typeof(Brain),
					typeof(Depended),
					typeof(Dependent1),
					typeof(Dependent2),
					typeof(DependentOn1),
					typeof(EqualBid1),
					typeof(EqualBid2),
					typeof(External.AutomaticGetter),
					typeof(External.ExternalComponent),
					typeof(Mandatory),
					typeof(Motor),
					typeof(NoAccessorComponent),
					typeof(Rendering),
					typeof(Stats),
					typeof(StatsRendering),
					typeof(TestClass1),
					typeof(TestClass2),
					typeof(TestClassMulticast),
					typeof(TestClassMulticastHigherBid1),
					typeof(TestClassMulticastHigherBid2),
					typeof(TestClassMulticastHighPriority),
					typeof(TestClassMulticastLowPriority),
				};
		}
	}

	public static class GameComponentsAccessors
	{
		public static int _AnotherNamespace_NestedClassOffset = 0;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static AnotherNamespace.NestedClass GetAnothersNestedClass(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_AnotherNamespace_NestedClassOffset) as AnotherNamespace.NestedClass;
		}

		public static int _BrainOffset = 1;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Brain GetBrain(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_BrainOffset) as Brain;
		}

		public static int _DependedOffset = 2;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Depended GetDepended(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_DependedOffset) as Depended;
		}

		public static int _Dependent1Offset = 3;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Dependent1 GetDependent1(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_Dependent1Offset) as Dependent1;
		}

		public static int _Dependent2Offset = 4;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Dependent2 GetDependent2(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_Dependent2Offset) as Dependent2;
		}

		public static int _DependentOn1Offset = 5;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static DependentOn1 GetDependentOn1(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_DependentOn1Offset) as DependentOn1;
		}

		public static int _EqualBid1Offset = 6;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static EqualBid1 GetEqualBid1(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_EqualBid1Offset) as EqualBid1;
		}

		public static int _EqualBid2Offset = 7;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static EqualBid2 GetEqualBid2(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_EqualBid2Offset) as EqualBid2;
		}

		public static int _External_AutomaticGetterOffset = 8;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static External.AutomaticGetter GetAutomaticGetter(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_External_AutomaticGetterOffset) as External.AutomaticGetter;
		}

		public static int _External_ExternalComponentOffset = 9;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static External.ExternalComponent GetExternalComponent(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_External_ExternalComponentOffset) as External.ExternalComponent;
		}

		public static int _MandatoryOffset = 10;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Mandatory GetMandatory(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_MandatoryOffset) as Mandatory;
		}

		public static int _MotorOffset = 11;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Motor GetMotor(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_MotorOffset) as Motor;
		}

		public static int _NoAccessorComponentOffset = 12;

		public static int _RenderingOffset = 13;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Rendering GetRendering(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_RenderingOffset) as Rendering;
		}

		public static int _StatsOffset = 14;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static Stats GetStats(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_StatsOffset) as Stats;
		}

		public static int _StatsRenderingOffset = 15;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static StatsRendering GetStatsRendering(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_StatsRenderingOffset) as StatsRendering;
		}

		public static int _TestClass1Offset = 16;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static TestClass1 GetTestClass1(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_TestClass1Offset) as TestClass1;
		}

		public static int _TestClass2Offset = 17;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static TestClass2 GetTestClass2(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_TestClass2Offset) as TestClass2;
		}

		public static int _TestClassMulticastOffset = 18;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static TestClassMulticast GetTestClassMulticast(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_TestClassMulticastOffset) as TestClassMulticast;
		}

		public static int _TestClassMulticastHigherBid1Offset = 19;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static TestClassMulticastHigherBid1 GetTestClassMulticastHigherBid1(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_TestClassMulticastHigherBid1Offset) as TestClassMulticastHigherBid1;
		}

		public static int _TestClassMulticastHigherBid2Offset = 20;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static TestClassMulticastHigherBid2 GetTestClassMulticastHigherBid2(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_TestClassMulticastHigherBid2Offset) as TestClassMulticastHigherBid2;
		}

		public static int _TestClassMulticastHighPriorityOffset = 21;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static TestClassMulticastHighPriority GetTestClassMulticastHighPriority(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_TestClassMulticastHighPriorityOffset) as TestClassMulticastHighPriority;
		}

		public static int _TestClassMulticastLowPriorityOffset = 22;
		[System.Diagnostics.DebuggerNonUserCode]
		[System.Diagnostics.Contracts.Pure]
		public static TestClassMulticastLowPriority GetTestClassMulticastLowPriority(this Dyn.DynObject obj)
		{
			return obj.GetComponent(_TestClassMulticastLowPriorityOffset) as TestClassMulticastLowPriority;
		}
	}
}