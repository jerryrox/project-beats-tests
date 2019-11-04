using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Dependencies.Tests
{
    public class TypeInjectorTest {
        
        [Test]
        public void TestCreation()
        {
            TypeInjector injector = new TypeInjector(typeof(Dummy));
            Assert.IsNull(injector.BaseInjector);

            TypeInjector injector2 = new TypeInjector(typeof(Dummy2), injector);
            Assert.IsNotNull(injector2.BaseInjector);
        }

        [Test]
        public void TestInject()
        {
            IList<int> list1 = new List<int>();
            List<float> list2 = new List<float>();
            Test test = new Test();

            var dependency = new DependencyContainer();
            dependency.Cache(list1);
            dependency.Cache(list2);
            dependency.CacheAs<IList<float>>(list2);
            dependency.Cache(test);
            dependency.CacheAs<ITest>(test);

            TypeInjector injector = new TypeInjector(typeof(Dummy));
            TypeInjector injector2 = new TypeInjector(typeof(Dummy2), injector);
            TypeInjector injector3 = new TypeInjector(typeof(EmptyDummy));

            var dummy = new Dummy();
            var dummy2 = new Dummy2();
            var dummy3 = new EmptyDummy();

            LogAssert.Expect(LogType.Error, $"TypeInjector.Inject - Injection target's type ({typeof(Dummy2).Name}) does not match the responsible type ({typeof(Dummy).Name})!");
            injector.Inject(dummy2, dependency);
            Assert.IsNull(dummy2.List1);
            Assert.IsNull(dummy2.List2);
            Assert.IsNull(dummy2.List3);
            Assert.IsNull(dummy2.TestClass);
            Assert.IsNull(dummy2.TestInterface);

            injector.Inject(dummy, dependency);
            Assert.AreSame(dummy.TestInterface, test);
            Assert.AreSame(dummy.List1, list1);
            Assert.AreSame(dummy.List2, list2);

            injector2.Inject(dummy2, dependency);
            Assert.AreSame(dummy2.TestClass, test);
            Assert.AreSame(dummy2.TestInterface, test);
            Assert.AreSame(dummy2.List1, list1);
            Assert.AreSame(dummy2.List2, list2);
            Assert.AreSame(dummy2.List3, list2);

            Assert.IsFalse(dummy3.IsCalled);
            injector3.Inject(dummy3, dependency);
            Assert.IsTrue(dummy3.IsCalled);
        }

        private interface ITest
        {
            string A { get; }
            int B { get; }
        }

        private class Test : ITest
        {
            public string A { get; set; }
            public int B { get; set; }
        }

        private class EmptyDummy
        {
            public bool IsCalled;

            [InitWithDependency]
            void Init()
            {
                IsCalled = true;
            }
        }

        private class Dummy
        {
            public ITest TestInterface;

            [ReceivesDependency]
            public List<int> List1 { get; set; }

            [ReceivesDependency]
            public IList<float> List2 { get; set; }

            [InitWithDependency]
            void Init(ITest test)
            {
                TestInterface = test;
            }
        }

        private class Dummy2 : Dummy
        {
            public Test TestClass;

            [ReceivesDependency]
            public List<float> List3 { get; set; }

            [InitWithDependency]
            void Init(Test test)
            {
                TestClass = test;
            }
        }
    }
}