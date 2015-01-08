// This file is part of the DynObjects library.
// Copyright 2010 Stefan Dragnev.
// The library and all its files are distributed under the MIT license.
// See the full text of the license in the accompanying LICENSE.txt file or at http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Dyn;

namespace External
{
    public class ExternalComponent {}

    public class AutomaticGetter {}
}

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
internal sealed class TestAttribute : Attribute
{
    public int[] Values;
    public string SomeOtherValues;
}

namespace DynTest
{
    public sealed partial class DefaultMessageImplementations
    {
        public void Baz()
        {
            Console.WriteLine("Default Baz() called");
        }

        public int TestMessageDefaultImpl()
        {
            return -1;
        }

        public void TestMessageMulticastDefaultImpl(ref int value)
        {
            value = -2;
        }
    }

    public class Brain : Component, GameMsgs.Baz
    {
        public int Neuron = 1234567;

        [Dyn.Priority(1)]
        public void Baz()
        {
            //Console.WriteLine("Brain.Baz");
        }
    }

    namespace AnotherNamespace
    {
        public class NestedClass {}
    }

    [SerializedComponent]
    public class Stats : Component, GameMsgs.Foo, GameMsgs.Bar, GameMsgs.Baz
    {
        public int Stamina = 15;
        public int Strength { get; set; }

        public Stats()
        {
            Strength = 10;
        }

        public int Bar(int a, char b, string c)
        {
            Console.WriteLine("{0}, {1}", b, c);
            return a * 3;
        }

        [Dyn.Priority(-100)]
        public void Baz()
        {
            //Console.WriteLine("Stats.Baz");
        }

        [Dyn.Bid(100)]
        public void Foo()
        {
            //Console.Out.WriteLine("Foo");
        }
    }

    public class StatsRendering {}

    public delegate void TestDelegate();

    [SerializedComponent]
    public class Motor : Component, IDisposable, GameMsgs.Foo, GameMsgs.Baz
    {
        public int Flags = 15;

        [XmlIgnore]
        public int LocalFlags = 10;

        [Dyn.Bid(1)]
        public void Foo()
        {
            //Console.Out.WriteLine("Foo");
        }

        //protected override void DynObjectCreated()
        //{
        //    //Console.Out.WriteLine("Motor created");
        //}
    
        [Dyn.Priority(100)]
        public void Baz()
        {
            //Console.WriteLine("Motor.Baz");
        }

        public void Dispose()
        {
        }
    }

    public class Rendering 
    {
        public int Subsets = 2;
    }

    public class TestClass1 : GameMsgs.TestMessage0, GameMsgs.TestMessage1, GameMsgs.Overloaded, GameMsgs.OverloadedFloat, GameMsgs.OverloadedInt
    {
        public void TestMessage0()
        {
        }

        [Dyn.Bid(100000)]
        public int TestMessage1(out bool result)
        {
            result = true;
            return 1;
        }

        public string Overloaded(string x)
        {
            return x;
        }

        public float Overloaded(float x)
        {
            return x;
        }

        public int Overloaded(int x)
        {
            return x;
        }
    }

    public class TestClass2 : GameMsgs.TestMessageDefaultImpl, GameMsgs.TestMessage1
    {
        public int TestMessageDefaultImpl()
        {
            return 1;
        }

        [Dyn.Bid(1000000)]
        public int TestMessage1(out bool result)
        {
            result = true;
            return 2;
        }
    }

    public class TestClassMulticast : GameMsgs.TestMessageMulticast
    {
        public void TestMessageMulticast(ref List<string> result)
        {
            result.Add("normal");
        }
    }
    public class TestClassMulticastHighPriority : GameMsgs.TestMessageMulticast
    {
        [Dyn.Priority(1)]
        public void TestMessageMulticast(ref List<string> result)
        {
            result.Add("highpri");
        }
    }
    public class TestClassMulticastLowPriority : GameMsgs.TestMessageMulticast
    {
        [Dyn.Priority(-1)]
        public void TestMessageMulticast(ref List<string> result)
        {
            result.Add("lowpri");
        }
    }
    public class TestClassMulticastHigherBid1 : GameMsgs.TestMessageMulticast
    {
        [Dyn.Priority(5)]
        [Dyn.Bid(1)]
        public void TestMessageMulticast(ref List<string> result)
        {
            result.Add("highbid1");
        }
    }
    public class TestClassMulticastHigherBid2 : GameMsgs.TestMessageMulticast, GameMsgs.TestMessageMulticastDefaultImpl
    {
        [Dyn.Priority(6)]
        [Dyn.Bid(1)]
        public void TestMessageMulticast(ref List<string> result)
        {
            result.Add("highbid2");
        }

        public void TestMessageMulticastDefaultImpl(ref int value)
        {
            value = 2;
        }
    }

    public class Depended { }
    public class Dependent1 { }
    public class Dependent2 { }
    public class DependentOn1 { }
    public class Mandatory { }

    public class EqualBid1 : GameMsgs.Foo
    {
        public void Foo()
        {
        }
    }
    public class EqualBid2 : GameMsgs.Foo
    {
        public void Foo()
        {
        }
    }

    public class NoAccessorComponent
    {
    }
}
