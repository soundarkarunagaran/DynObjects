// This file is part of the DynObjects library.
// Copyright 2010 Stefan Dragnev.
// The library and all its files are distributed under the MIT license.
// See the full text of the license in the accompanying LICENSE.txt file or at http://www.opensource.org/licenses/mit-license.php

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dyn;
using DynTest.More;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynTest
{
    /// <summary>
    /// Summary description for MultipleProviders
    /// </summary>
    [TestClass]
    public class MultipleProviders
    {
        public MultipleProviders()
        {
            //
            // TODO: Add constructor logic here
            //
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

        [TestMethod]
        public void TestMultipleProviders()
        {
            var provider = Dyn.ComponentProvider.Merge(new GameComponents(), new MoreComponents());
            var factory = new DynFactory(provider);
            factory.AddInjectionRule(typeof(Brain), typeof(More.View));

            var obj = factory.CreateObject(new Stats(), new More.Web());

            Assert.IsNotNull(obj.GetStats());
            Assert.IsNotNull(obj.GetComponent<Stats>());
            Assert.IsNotNull(obj.GetWeb());
            Assert.IsNotNull(obj.GetComponent<More.Web>());

            obj.Foo();
            obj.TestMessageDefaultImpl();
            obj.Baz();

            obj.Browse();
            obj.DoSomething();

            obj.Mutate(new Brain());

            Assert.IsNotNull(obj.GetStats());
            Assert.IsNotNull(obj.GetComponent<Stats>());
            Assert.IsNotNull(obj.GetWeb());
            Assert.IsNotNull(obj.GetComponent<Web>());
            Assert.IsNotNull(obj.GetBrain());
            Assert.IsNotNull(obj.GetComponent<Brain>());
            Assert.IsNotNull(obj.GetView());
            Assert.IsNotNull(obj.GetComponent<View>());

            obj.Foo();
            obj.TestMessageDefaultImpl();

            obj.Browse();
            obj.DoSomething();
        }
    }
}
