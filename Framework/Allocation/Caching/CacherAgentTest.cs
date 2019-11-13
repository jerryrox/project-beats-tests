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
            var agent = new CacherAgent<DummyCacherData>(cacher);
            Assert.AreEqual(cacher, agent.Cacher);

            agent.UseDelayedRemove = false;
            agent.RemoveDelay = 0f;

            Assert.IsNull(agent.Value);
            Assert.IsNull(agent.CurrentKey);
            Assert.IsFalse(cacher.IsCached("A"));

            // Request data A.
            agent.Request("A");
            Assert.IsFalse(cacher.IsCached("A"));

            float limit = 2f;
            while (!cacher.IsCached("A"))
            {
                Debug.Log("Progress: " + agent.Progress);
                limit -= Time.deltaTime;
                if(limit <= 0)
                    Assert.Fail("Request should've finished by now!");
                yield return null;
            }
            Assert.IsTrue(cacher.IsCached("A"));
            Assert.AreEqual(1f, agent.Progress);
            Assert.IsNotNull(agent.Value);
            Assert.AreEqual("A", agent.CurrentKey);
            Assert.AreEqual("A", agent.Value.Key);
            Assert.IsFalse(agent.Value.IsDestroyed);

            // Request data B.
            agent.Request("B");
            Assert.IsFalse(cacher.IsCached("B"));
            Assert.AreEqual(0f, agent.Progress);
            Assert.AreEqual("B", agent.CurrentKey);
            Assert.IsNull(agent.Value);

            limit = 2f;
            while (!cacher.IsCached("B"))
            {
                Debug.Log("Progress: " + agent.Progress);
                limit -= Time.deltaTime;
                if(limit <= 0)
                    Assert.Fail("Request should've finished by now!");
                yield return null;
            }
            Assert.IsTrue(cacher.IsCached("B"));
            Assert.AreEqual(1f, agent.Progress);
            Assert.IsNotNull(agent.Value);
            Assert.AreEqual("B", agent.CurrentKey);
            Assert.AreEqual("B", agent.Value.Key);
            Assert.IsFalse(agent.Value.IsDestroyed);
        }

        [UnityTest]
        public IEnumerator TestRemoveImmediate()
        {
            var cacher = new DummyCacher();
            var agent = new CacherAgent<DummyCacherData>(cacher);
            Assert.AreEqual(cacher, agent.Cacher);

            agent.UseDelayedRemove = false;
            agent.RemoveDelay = 0f;
            Assert.IsNull(agent.Value);
            Assert.IsNull(agent.CurrentKey);
            Assert.IsFalse(cacher.IsCached("A"));

            agent.Request("A");
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(agent.Value);
            yield return new WaitForSecondsRealtime(1.5f);

            var value = agent.Value;
            Assert.IsTrue(cacher.IsCached("A"));
            Assert.IsNotNull(value);
            Assert.IsFalse(value.IsDestroyed);

            agent.Remove();
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(agent.Value);
            Assert.IsTrue(value.IsDestroyed);
        }

        [UnityTest]
        public IEnumerator TestRemoveDelayed()
        {
            var cacher = new DummyCacher();
            var agent = new CacherAgent<DummyCacherData>(cacher);
            Assert.AreEqual(cacher, agent.Cacher);

            agent.UseDelayedRemove = true;
            agent.RemoveDelay = 0.5f;
            Assert.IsNull(agent.Value);
            Assert.IsNull(agent.CurrentKey);
            Assert.IsFalse(cacher.IsCached("A"));

            agent.Request("A");
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(agent.Value);
            yield return new WaitForSecondsRealtime(1.5f);

            var value = agent.Value;
            Assert.IsTrue(cacher.IsCached("A"));
            Assert.IsNotNull(value);
            Assert.IsFalse(value.IsDestroyed);

            agent.Remove();
            Assert.IsTrue(cacher.IsCached("A"));
            Assert.IsNull(agent.Value);
            Assert.IsFalse(value.IsDestroyed);

            yield return new WaitForSecondsRealtime(1f);
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(agent.Value);
            Assert.IsTrue(value.IsDestroyed);
        }
    }
}