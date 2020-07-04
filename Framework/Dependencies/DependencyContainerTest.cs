using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Dependencies.Tests
{
    public class DependencyContainerTest {
        
        [Test]
        public void TestCache()
        {
            var dependency = new DependencyContainer();

            try
            {
                dependency.Cache(null);
            }
            catch (ArgumentNullException)
            {
            }

            Dummy dummy = new Dummy();
            Dummy dummy2 = new Dummy();

            dependency.Cache(dummy);
            Assert.AreSame(dummy, dependency.Get<Dummy>());

            LogAssert.Expect(LogType.Warning, $"DependencyContainer.CacheAs - A dependency already exists for type ({typeof(Dummy).Name})!");
            dependency.Cache(dummy2);

            dependency.Cache(dummy2, true);
            Assert.AreSame(dummy2, dependency.Get<Dummy>());
            Assert.AreNotSame(dummy2, dummy);
        }

        [Test]
        public void TestCacheAs()
        {
            var dependency = new DependencyContainer();

            try
            {
                dependency.CacheAs<Dummy>(null);
            }
            catch (ArgumentNullException)
            {
            }

            Dummy dummy = new Dummy();
            Dummy dummy2 = new Dummy();

            dependency.CacheAs(dummy);
            Assert.AreSame(dummy, dependency.Get<Dummy>());

            dependency.CacheAs<IDummy>(dummy);
            Assert.AreSame(dummy, dependency.Get<IDummy>());

            LogAssert.Expect(LogType.Warning, $"DependencyContainer.CacheAs - A dependency already exists for type ({typeof(Dummy).Name})!");
            dependency.CacheAs(dummy2);

            LogAssert.Expect(LogType.Warning, $"DependencyContainer.CacheAs - A dependency already exists for type ({typeof(IDummy).Name})!");
            dependency.CacheAs<IDummy>(dummy2);

            dependency.CacheAs(dummy2, true);
            Assert.AreSame(dummy2, dependency.Get<Dummy>());
            Assert.AreNotSame(dummy2, dummy);

            dependency.CacheAs<IDummy>(dummy2, true);
            Assert.AreSame(dummy2, dependency.Get<IDummy>());
            Assert.AreNotSame(dummy2, dummy);
        }

        [Test]
        public void TestCacheAndInject()
        {
            var dependency = new DependencyContainer();
            Dummy dummy = new Dummy();
            Assert.IsNull(dummy.Dependency);

            dependency.CacheAndInject(dummy);
            Assert.AreEqual(dummy, dependency.Get<Dummy>());
            Assert.AreEqual(dependency, dummy.Dependency);
        }

        [Test]
        public void TestCacheFrom()
        {
            var dependency = new DependencyContainer();
            var newDependency = new DependencyContainer();

            List<int> a = new List<int>();
            List<float> b = new List<float>();

            dependency.Cache(a);
            dependency.Cache(b);
            Assert.AreSame(a, dependency.Get<List<int>>());
            Assert.AreSame(b, dependency.Get<List<float>>());

            newDependency.CacheFrom(dependency);
            Assert.AreSame(dependency.Get<List<int>>(), newDependency.Get<List<int>>());
            Assert.AreSame(dependency.Get<List<float>>(), newDependency.Get<List<float>>());

            var newA = new List<int>();
            var newB = new List<float>();
            Assert.AreNotSame(newA, dependency.Get<List<int>>());
            Assert.AreNotSame(newA, newDependency.Get<List<int>>());
            Assert.AreNotSame(newB, dependency.Get<List<float>>());
            Assert.AreNotSame(newB, newDependency.Get<List<float>>());

            var lastDependency = new DependencyContainer();
            lastDependency.Cache(newA);
            lastDependency.Cache(newB);

            newDependency.CacheFrom(lastDependency);
            Assert.AreNotSame(newA, newDependency.Get<List<int>>());
            Assert.AreNotSame(newB, newDependency.Get<List<float>>());

            newDependency.CacheFrom(lastDependency, true);
            Assert.AreSame(newA, newDependency.Get<List<int>>());
            Assert.AreSame(newB, newDependency.Get<List<float>>());
        }

        [Test]
        public void TestClone()
        {
            var dependency = new DependencyContainer();

            Dummy dummy = new Dummy();
            dependency.Cache(dummy);
            dependency.CacheAs<IDummy>(dummy);

            var cloned = dependency.Clone();
            Assert.IsNotNull(cloned);
            Assert.AreSame(dependency.Get<Dummy>(), cloned.Get<Dummy>());
            Assert.AreSame(dependency.Get<IDummy>(), cloned.Get<IDummy>());
        }

        [Test]
        public void TestRemove()
        {
            var dependency = new DependencyContainer();

            Dummy dummy = new Dummy();
            dependency.Cache(dummy);
            dependency.CacheAs<IDummy>(dummy);

            Assert.AreSame(dummy, dependency.Get<Dummy>());
            Assert.AreSame(dummy, dependency.Get<IDummy>());

            dependency.Remove(dummy);
            Assert.IsNull(dependency.Get<Dummy>());
            Assert.AreSame(dummy, dependency.Get<IDummy>());

            dependency.Remove<IDummy>();
            Assert.IsNull(dependency.Get<Dummy>());
            Assert.IsNull(dependency.Get<IDummy>());
        }

        [Test]
        public void TestGet()
        {
            var dependency = new DependencyContainer();

            Dummy dummy = new Dummy();
            dependency.Cache(dummy);
            dependency.CacheAs<IDummy>(dummy);

            Assert.AreSame(dummy, dependency.Get<Dummy>());
            Assert.AreSame(dummy, dependency.Get<IDummy>());
            Assert.AreSame(dummy, dependency.Get(typeof(Dummy)));
            Assert.AreSame(dummy, dependency.Get(typeof(IDummy)));

            LogAssert.Expect(LogType.Warning, $"DependencyContainer.Get - Failed to find dependency of type: {typeof(string).Name}");
            Assert.IsNull(dependency.Get<string>());
            LogAssert.Expect(LogType.Warning, $"DependencyContainer.Get - Failed to find dependency of type: {typeof(string).Name}");
            Assert.IsNull(dependency.Get(typeof(string)));
        }

        private interface IDummy
        {

        }

        private class Dummy : IDummy
        {
            [ReceivesDependency]
            public IDependencyContainer Dependency { get; set; }
        }
    }
}