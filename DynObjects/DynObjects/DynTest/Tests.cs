// This file is part of the DynObjects library.
// Copyright 2010 Stefan Dragnev.
// The library and all its files are distributed under the MIT license.
// See the full text of the license in the accompanying LICENSE.txt file or at http://www.opensource.org/licenses/mit-license.php

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dyn;
using External;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DynTest
{
    /// <summary>
    /// Summary description for Tests
    /// </summary>
    [TestClass]
    public class Tests
    {
        public Tests()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        public void CheckDynObjectReference(DynObject obj)
        {
            foreach (var c in obj._Components)
            {
                var hook = c as Dyn.IHasObjectHook;
                if (hook != null)
                    Assert.AreEqual(obj, ((Component) hook.ObjectHook).DynThis);
            }
            obj.TestMessageDefaultImpl();
        }

        [TestMethod]
        public void TestObjectCreation()
        {
            var factory = new DynFactory(new GameComponents());
            var obj = factory.CreateObject(new Brain(), new Stats(), new Motor());
            CheckDynObjectReference(obj);
            Assert.IsNotNull(obj.GetComponent<Brain>());
            Assert.IsNotNull(obj.GetBrain());
            Assert.IsNotNull(obj.GetComponent<Stats>());
            Assert.IsNotNull(obj.GetStats());
            Assert.IsNotNull(obj.GetComponent<Motor>());
            Assert.IsNotNull(obj.GetMotor());
            Assert.IsNull(obj.GetComponent<Rendering>());
            Assert.IsNull(obj.GetRendering());

            Assert.AreEqual(obj, obj.GetBrain().DynThis);
            Assert.AreEqual(obj, obj.GetStats().DynThis);
            Assert.AreEqual(obj, obj.GetMotor().DynThis);

            obj = factory.CreateObjectType(typeof(Motor)).CreateObject();
            CheckDynObjectReference(obj);
            Assert.IsNull(obj.GetComponent<Brain>());
            Assert.IsNull(obj.GetBrain());
            Assert.IsNull(obj.GetComponent<Stats>());
            Assert.IsNull(obj.GetStats());
            Assert.IsNotNull(obj.GetComponent<Motor>());
            Assert.IsNotNull(obj.GetMotor());
            Assert.IsNull(obj.GetComponent<Rendering>());
            Assert.IsNull(obj.GetRendering());

            Assert.AreEqual(obj, obj.GetMotor().DynThis);
        }

        [TestMethod]
        public void TestNullObject()
        {
            var factory = new DynFactory(new GameComponents());
            var type = factory.CreateObjectType();
            var nullObj = factory.CreateObject();
            Assert.IsNull(nullObj.GetComponent<Brain>());
            Assert.IsNull(nullObj.GetBrain());
            Assert.IsNull(nullObj.GetComponent<Stats>());
            Assert.IsNull(nullObj.GetStats());
            Assert.IsNull(nullObj.GetComponent<Motor>());
            Assert.IsNull(nullObj.GetMotor());
            Assert.IsNull(nullObj.GetComponent<Rendering>());
            Assert.IsNull(nullObj.GetRendering());

            nullObj = type.CreateObject();
            Assert.IsNull(nullObj.GetComponent<Brain>());
            Assert.IsNull(nullObj.GetBrain());
            Assert.IsNull(nullObj.GetComponent<Stats>());
            Assert.IsNull(nullObj.GetStats());
            Assert.IsNull(nullObj.GetComponent<Motor>());
            Assert.IsNull(nullObj.GetMotor());
            Assert.IsNull(nullObj.GetComponent<Rendering>());
            Assert.IsNull(nullObj.GetRendering());
        }

        [TestMethod]
        public void TestTypeInfo()
        {
            var factory = new DynFactory(new GameComponents());
            var type = factory.CreateObjectType(typeof(Motor), typeof(Stats), typeof(Brain), typeof(AnotherNamespace.NestedClass), typeof(External.ExternalComponent));
            var obj = type.CreateObject();
            Assert.IsNotNull(obj.GetComponent<Brain>());
            Assert.IsNotNull(obj.GetComponent<Stats>());
            Assert.IsNotNull(obj.GetComponent<Motor>());
            Assert.IsNull(obj.GetComponent<Rendering>());

            //testing components that reside in a nested namespace (relative to the component accessor's namespace) and in a totally unrelated namespace
            Assert.IsNotNull(obj.GetAnothersNestedClass());
            Assert.IsNotNull(obj.GetComponent<AnotherNamespace.NestedClass>());
            Assert.IsNotNull(obj.GetExternalComponent());
            Assert.IsNotNull(obj.GetComponent<External.ExternalComponent>());
        }

        [TestMethod]
        public void TestUnicastMessages()
        {
            var factory = new DynFactory(new GameComponents());
            var obj = factory.CreateObject(new TestClass1());
            obj.TestMessage0();

            bool result;
            int ret = obj.TestMessage1(out result);
            Assert.AreEqual(1, ret);
            Assert.AreEqual(true, result);

            Assert.IsFalse(obj.ImplementsMessage(typeof(GameMsgs.TestMessageDefaultImpl)));
            Assert.AreEqual(-1, obj.TestMessageDefaultImpl());

            obj.Mutate(new TestClass2());
            CheckDynObjectReference(obj);
            obj.TestMessageDefaultImpl();
            result = false;
            ret = obj.TestMessage1(out result);
            Assert.AreEqual(2, ret);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestMulticast()
        {
            var factory = new DynFactory(new GameComponents());
            var obj = factory.CreateObject(new TestClassMulticast(), new TestClassMulticastHighPriority(), new TestClassMulticastLowPriority());
            var list = new List<string>();

            obj.TestMessageMulticast(ref list);
            Assert.IsTrue(list.SequenceEqual(new[] { "highpri", "normal", "lowpri" }));

            int result = 0;
            Assert.IsFalse(obj.ImplementsMessage(typeof(GameMsgs.TestMessageMulticastDefaultImpl)));
            obj.TestMessageMulticastDefaultImpl(ref result);
            Assert.AreEqual(-2, result);

            list.Clear();
            obj.Mutate(new TestClassMulticastHigherBid1(), new TestClassMulticastHigherBid2());
            CheckDynObjectReference(obj);
            obj.TestMessageDefaultImpl();
            obj.TestMessageMulticast(ref list);
            Assert.IsTrue(list.SequenceEqual(new[] { "highbid2", "highbid1" }));
        }

        [TestMethod]
        public void TestImplementsMessage()
        {
            var factory = new DynFactory(new GameComponents());
            var obj = factory.CreateObject(new TestClass1());

            Assert.IsTrue(obj.ImplementsMessage(typeof(GameMsgs.TestMessage0)));
            Assert.IsTrue(obj.ImplementsMessage(typeof(GameMsgs.TestMessage1)));
            Assert.IsFalse(obj.ImplementsMessage(typeof(GameMsgs.TestMessageDefaultImpl)));
            Assert.IsFalse(obj.ImplementsMessage(typeof(GameMsgs.TestMessageMulticast)));

            obj = factory.CreateObject(new TestClass1(), new TestClass2());
            Assert.IsTrue(obj.ImplementsMessage(typeof(GameMsgs.TestMessage0)));
            Assert.IsTrue(obj.ImplementsMessage(typeof(GameMsgs.TestMessage1)));
            Assert.IsTrue(obj.ImplementsMessage(typeof(GameMsgs.TestMessageDefaultImpl)));
            Assert.IsFalse(obj.ImplementsMessage(typeof(GameMsgs.TestMessageMulticast)));

            obj = factory.CreateObject(new TestClassMulticastHigherBid1());
            Assert.IsTrue(obj.ImplementsMessage(typeof(GameMsgs.TestMessageMulticast)));
            Assert.IsFalse(obj.ImplementsMessage(typeof(GameMsgs.TestMessageMulticastDefaultImpl)));

            obj = factory.CreateObject(new TestClassMulticastHigherBid1(), new TestClassMulticastHigherBid2());
            Assert.IsTrue(obj.ImplementsMessage(typeof(GameMsgs.TestMessageMulticast)));
            Assert.IsTrue(obj.ImplementsMessage(typeof(GameMsgs.TestMessageMulticastDefaultImpl)));
        }

        [TestMethod]
        public void TestXmlSerializer()
        {
            var factory = new DynFactory(new GameComponents());
            factory.AddInjectionRule(typeof(Stats), typeof(StatsRendering));
            var obj = factory.CreateObject(new Brain(), new Stats(), new Motor());

            var stats = obj.GetStats();
            stats.Stamina = 3333;
            stats.Strength = 7777;
            var motor = obj.GetMotor();
            motor.LocalFlags = 777;

            var serialized = factory.Serialize(obj);

            var obj2 = factory.Deserialize(serialized, null);
            Assert.IsNotNull(obj2.GetStats()); // Stats is [Serializable]
            Assert.IsNotNull(obj2.GetComponent<Stats>()); // Stats is [Serializable]
            Assert.IsNull(obj2.GetBrain()); // Brain isn't [Serializable]
            Assert.IsNotNull(obj2.GetMotor()); // Motor is [Serializable]
            Assert.IsNotNull(obj2.GetComponent<Motor>()); // Motor is [Serializable]
            Assert.IsNull(obj2.GetRendering()); // Rendering isn't included
            Assert.IsNotNull(obj2.GetComponent<StatsRendering>()); // injection rule
            stats = obj2.GetComponent<Stats>();
            Assert.AreEqual(3333, stats.Stamina);
            Assert.AreEqual(7777, stats.Strength);
            motor = obj2.GetMotor();
            Assert.AreEqual(10, motor.LocalFlags); // non-data members should not be serialized

            CheckDynObjectReference(obj);
            CheckDynObjectReference(obj2);
        }

        [TestMethod]
        public void TestMutation()
        {
            var factory = new DynFactory(new GameComponents());
            var obj = factory.CreateObject(new Brain(), new Stats());

            obj.Mutate(new object[] { new Motor(), new Rendering() }, new[] { typeof(Brain) });
            Assert.IsNull(obj.GetBrain());
            Assert.IsNotNull(obj.GetStats());
            Assert.IsNotNull(obj.GetComponent<Stats>());
            Assert.IsNotNull(obj.GetMotor());
            Assert.IsNotNull(obj.GetComponent<Motor>());
            Assert.IsNotNull(obj.GetRendering());
            Assert.IsNotNull(obj.GetComponent<Rendering>());
            CheckDynObjectReference(obj);
            obj.TestMessageDefaultImpl();

            obj.Mutate(new Brain());
            Assert.IsNotNull(obj.GetBrain());
            Assert.IsNotNull(obj.GetComponent<Brain>());
            CheckDynObjectReference(obj);
            obj.TestMessageDefaultImpl();

            obj.Mutate(null, null);
            Assert.IsNotNull(obj.GetBrain());
            Assert.IsNotNull(obj.GetComponent<Brain>());
            Assert.IsNotNull(obj.GetStats());
            Assert.IsNotNull(obj.GetComponent<Stats>());
            Assert.IsNotNull(obj.GetMotor());
            Assert.IsNotNull(obj.GetComponent<Motor>());
            Assert.IsNotNull(obj.GetRendering());
            Assert.IsNotNull(obj.GetComponent<Rendering>());
            CheckDynObjectReference(obj);
            obj.TestMessageDefaultImpl();

            obj.Mutate(typeof(Brain), typeof(Motor), typeof(Rendering), typeof(Stats));
            Assert.IsNull(obj.GetBrain());
            Assert.IsNull(obj.GetStats());
            Assert.IsNull(obj.GetMotor());
            Assert.IsNull(obj.GetRendering());
            CheckDynObjectReference(obj);
            obj.TestMessageDefaultImpl();

            obj.Mutate(new Brain(), new Stats());
            Assert.IsNotNull(obj.GetBrain());
            Assert.IsNotNull(obj.GetComponent<Brain>());
            Assert.IsNotNull(obj.GetStats());
            Assert.IsNotNull(obj.GetComponent<Stats>());
            Assert.IsNull(obj.GetMotor());
            Assert.IsNull(obj.GetComponent<Motor>());
            Assert.IsNull(obj.GetRendering());
            Assert.IsNull(obj.GetComponent<Rendering>());
            CheckDynObjectReference(obj);
            obj.TestMessageDefaultImpl();

            try
            {
                // you mustn't be able to add components that are already part of the object
                obj.Mutate(new Brain());
                Assert.Fail();
            }
            catch (DynException)
            {
                Assert.IsNotNull(obj.GetBrain());
                Assert.IsNotNull(obj.GetStats());
                Assert.IsNull(obj.GetMotor());
                Assert.IsNull(obj.GetRendering());
                CheckDynObjectReference(obj);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestInvalidMessageCall()
        {
            var factory = new DynFactory(new GameComponents());
            var obj = factory.CreateObject();

            try
            {
                obj.Foo();
                Assert.Fail();
            }
            catch (DynException)
            {
                // ok, unimplemented unicast message call throws a DynException
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            var foo = new List<string>();
            obj.TestMessageMulticast(ref foo); // multicast messages may have absolutely no implementation, not even a default one
        }

        class UnpublishedComponent
        {
        }

        [TestMethod]
        public void TestUnpublishedComponent()
        {
            var factory = new DynFactory(new GameComponents());
            try
            {
                var obj = factory.CreateObject(new UnpublishedComponent());
                Assert.Fail();
            }
            catch (DynException)
            {
                // success
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestOverloads()
        {
            var factory = new DynFactory(new GameComponents());
            var obj = factory.CreateObject(new TestClass1());

            Assert.AreEqual(10, obj.Overloaded(10));
            Assert.AreEqual(30f, obj.Overloaded(30f));
            Assert.AreEqual("overload", obj.Overloaded("overload"));
        }

        [TestMethod]
        public void TestDisposable()
        {
            var factory = new DynFactory(new GameComponents());
            using (var obj = factory.CreateObject(new Brain(), new Motor()))
            {
            }
        }

        private DynFactory TestDependencies_CreateFactoryWithInjectionRules()
        {
            var factory = new DynFactory(new GameComponents());
            factory.AddInjectionRule(typeof(Depended), typeof(Dependent1));
            factory.AddInjectionRule(typeof(Depended), typeof(Dependent2));
            factory.AddInjectionRule(typeof(Dependent1), typeof(DependentOn1));
            return factory;
        }

        [TestMethod]
        public void TestDependencies()
        {
            var factory = TestDependencies_CreateFactoryWithInjectionRules();
            var objType = factory.CreateObjectType(typeof(Depended));
            var obj = objType.CreateObject(new Depended());

            Assert.IsNotNull(obj.GetComponent<Depended>());
            Assert.IsNotNull(obj.GetComponent<Dependent1>());
            Assert.IsNotNull(obj.GetComponent<Dependent2>());
            Assert.IsNotNull(obj.GetComponent<DependentOn1>());
            Assert.IsNull(obj.GetMandatory());

            obj = objType.CreateObject(new Dependent1());
            Assert.IsNotNull(obj.GetComponent<Depended>());
            Assert.IsNotNull(obj.GetComponent<Dependent1>());
            Assert.IsNotNull(obj.GetComponent<Dependent2>());
            Assert.IsNotNull(obj.GetComponent<DependentOn1>());
            Assert.IsNull(obj.GetMandatory());

            factory = TestDependencies_CreateFactoryWithInjectionRules();
            factory.AddMandatoryComponent(typeof(Mandatory));
            objType = factory.CreateObjectType(typeof(Dependent1));
            obj = objType.CreateObject();
            Assert.IsNull(obj.GetComponent<Depended>());
            Assert.IsNotNull(obj.GetComponent<Dependent1>());
            Assert.IsNull(obj.GetComponent<Dependent2>());
            Assert.IsNotNull(obj.GetComponent<DependentOn1>());
            Assert.IsNotNull(obj.GetMandatory());
        }

        [TestMethod]
        public void TestEqualBid()
        {
            var factory = new DynFactory(new GameComponents());
            try
            {
                var obj = factory.CreateObject(new EqualBid1(), new EqualBid2());
                Assert.Fail();
            }
            catch (DynException)
            {
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestAutomaticGetter()
        {
            var factory = new DynFactory(new GameComponents());
            var obj = factory.CreateObject(new AutomaticGetter());

            var component = obj.GetAutomaticGetter();
            Assert.IsNotNull(component);

            obj = factory.CreateObject();
            component = obj.GetAutomaticGetter();
            Assert.IsNull(component);
        }

        [TestMethod]
        public void TestCreateObjectTypeFromVariousStuff()
        {
            var factory = new DynFactory(new GameComponents());

            var obj = factory.CreateObject(new Brain(), new Motor());
            var type = factory.CreateObjectType(typeof(Rendering), typeof(Brain));

            var combinedTypes = DynFactory.GetTypes(obj, type, typeof(StatsRendering), new Stats());
            var objectType = factory.CreateObjectType(combinedTypes);
            var combined = objectType.CreateObject();

            var combined2 = factory.CreateObjectType(
                DynFactory.GetTypes(objectType, combined, combined, combinedTypes, objectType)
                ).CreateObject();

            Assert.IsNotNull(combined.GetBrain());
            Assert.IsNotNull(combined.GetMotor());
            Assert.IsNotNull(combined.GetRendering());
            Assert.IsNotNull(combined.GetStatsRendering());
            Assert.IsNotNull(combined.GetStats());
            Assert.IsTrue(combined._Components.Zip(combined2._Components, (o, o1) => o.GetType() == o1.GetType()).All(_ => _));

            var empty = factory.CreateObject();
            var emptyTypes = DynFactory.GetTypes(empty);
            Assert.AreEqual(0, emptyTypes.Length);
        }

        //TODO: test call next bidder
        //TODO: test ObjectHook and all cases when the events should fire
        //TODO: test that int.MinValue can't be used as a message bid
        //TODO: more tests are necessary for messages called thru interfaces
        //TODO: test messages implemented thru explicit interfaces
        //TODO: fuller tests for merged component providers
    }
}
