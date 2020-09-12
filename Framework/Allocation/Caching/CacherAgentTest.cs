using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Allocation.Caching.Tests
{
    public class CacherAgentTest {
        
        [UnityTest]
        public IEnumerator TestRequest()
        {
            var cacher = new DummyCacher();
            var agent = new CacherAgent<string, DummyCacherData>(cacher);
            Assert.AreEqual(cacher, agent.Cacher);

            agent.UseDelayedRemove = false;
            agent.RemoveDelay = 0f;

            Assert.IsNull(agent.Listener);
            Assert.IsFalse(cacher.IsCached("A"));

            // Request data A.
            agent.Request("A");
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNotNull(agent.Listener);

            float limit = 2f;
            while (!cacher.IsCached("A"))
            {
                Debug.Log("Progress: " + agent.Listener.Progress);
                limit -= Time.deltaTime;
                if(limit <= 0)
                    Assert.Fail("Request should've finished by now!");
                yield return null;
            }
            Assert.IsTrue(cacher.IsCached("A"));
            Assert.AreEqual(1f, agent.Listener.Progress);
            Assert.IsNotNull(agent.Listener.Value);
            Assert.AreEqual("A", agent.Listener.Key);
            Assert.AreEqual("A", agent.Listener.Value.Key);
            Assert.IsFalse(agent.Listener.Value.IsDestroyed);

            // Request data B.
            agent.Request("B");
            Assert.IsFalse(cacher.IsCached("B"));
            Assert.AreEqual(0f, agent.Listener.Progress);
            Assert.AreEqual("B", agent.Listener.Key);
            Assert.IsNull(agent.Listener.Value);

            limit = 2f;
            while (!cacher.IsCached("B"))
            {
                Debug.Log("Progress: " + agent.Listener.Progress);
                limit -= Time.deltaTime;
                if(limit <= 0)
                    Assert.Fail("Request should've finished by now!");
                yield return null;
            }
            Assert.IsTrue(cacher.IsCached("B"));
            Assert.AreEqual(1f, agent.Listener.Progress);
            Assert.IsNotNull(agent.Listener.Value);
            Assert.AreEqual("B", agent.Listener.Key);
            Assert.AreEqual("B", agent.Listener.Value.Key);
            Assert.IsFalse(agent.Listener.Value.IsDestroyed);
        }

        [UnityTest]
        public IEnumerator TestRemoveImmediate()
        {
            var cacher = new DummyCacher();
            var agent = new CacherAgent<string, DummyCacherData>(cacher);
            Assert.AreEqual(cacher, agent.Cacher);

            agent.UseDelayedRemove = false;
            agent.RemoveDelay = 0f;
            Assert.IsNull(agent.Listener);
            Assert.IsFalse(cacher.IsCached("A"));

            agent.Request("A");
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(agent.Listener.Value);
            yield return new WaitForSecondsRealtime(1.5f);

            var value = agent.Listener.Value;
            Assert.IsTrue(cacher.IsCached("A"));
            Assert.IsNotNull(value);
            Assert.IsFalse(value.IsDestroyed);

            agent.Remove();
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(agent.Listener);
            Assert.IsTrue(value.IsDestroyed);
        }

        [UnityTest]
        public IEnumerator TestRemoveDelayed()
        {
            var cacher = new DummyCacher();
            var agent = new CacherAgent<string, DummyCacherData>(cacher);
            Assert.AreEqual(cacher, agent.Cacher);

            agent.UseDelayedRemove = true;
            agent.RemoveDelay = 0.5f;
            Assert.IsNull(agent.Listener);
            Assert.IsFalse(cacher.IsCached("A"));

            agent.Request("A");
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(agent.Listener.Value);
            yield return new WaitForSecondsRealtime(1.5f);

            var value = agent.Listener.Value;
            Assert.IsTrue(cacher.IsCached("A"));
            Assert.IsNotNull(value);
            Assert.IsFalse(value.IsDestroyed);

            agent.Remove();
            Assert.IsTrue(cacher.IsCached("A"));
            Assert.IsNull(agent.Listener);
            Assert.IsFalse(value.IsDestroyed);

            yield return new WaitForSecondsRealtime(1f);
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(agent.Listener);
            Assert.IsTrue(value.IsDestroyed);
        }
    }
}