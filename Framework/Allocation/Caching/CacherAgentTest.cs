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
            Assert.IsNull(agent.CurrentKey);
            Assert.IsFalse(cacher.IsCached("A"));

            // Request data A.
            var listener = agent.Request("A");
            Assert.IsNotNull(listener);
            Assert.AreEqual(listener, agent.Listener);
            Assert.IsNotNull(agent.CurrentKey);
            Assert.IsFalse(cacher.IsCached("A"));

            float limit = 2f;
            while (!cacher.IsCached("A"))
            {
                Debug.Log("Progress: " + listener.Progress.Value);
                limit -= Time.deltaTime;
                if(limit <= 0)
                    Assert.Fail("Request should've finished by now!");
                yield return null;
            }
            Assert.IsTrue(cacher.IsCached("A"));
            Assert.AreEqual(1f, listener.Progress.Value);
            Assert.IsNotNull(listener.Output.Value);
            Assert.AreEqual("A", agent.CurrentKey);
            Assert.AreEqual("A", listener.Output.Value.Key);
            Assert.IsFalse(listener.Output.Value.IsDestroyed);

            // Request data B.
            listener = agent.Request("B");
            Assert.IsNotNull(listener);
            Assert.AreEqual(listener, agent.Listener);
            Assert.IsFalse(cacher.IsCached("B"));

            Assert.AreEqual(0f, listener.Progress.Value);
            Assert.AreEqual("B", agent.CurrentKey);
            Assert.IsNull(listener.Output.Value);

            limit = 2f;
            while (!cacher.IsCached("B"))
            {
                Debug.Log("Progress: " + listener.Progress.Value);
                limit -= Time.deltaTime;
                if(limit <= 0)
                    Assert.Fail("Request should've finished by now!");
                yield return null;
            }
            Assert.IsTrue(cacher.IsCached("B"));
            Assert.AreEqual(1f, listener.Progress.Value);
            Assert.IsNotNull(listener.Output.Value);
            Assert.AreEqual("B", agent.CurrentKey);
            Assert.AreEqual("B", listener.Output.Value.Key);
            Assert.IsFalse(listener.Output.Value.IsDestroyed);
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
            Assert.IsNull(agent.CurrentKey);
            Assert.IsFalse(cacher.IsCached("A"));

            var listener = agent.Request("A");
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(listener.Output.Value);
            yield return new WaitForSecondsRealtime(1.5f);

            var value = listener.Output.Value;
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
            Assert.IsNull(agent.CurrentKey);
            Assert.IsFalse(cacher.IsCached("A"));

            var listener = agent.Request("A");
            Assert.IsFalse(cacher.IsCached("A"));
            Assert.IsNull(listener.Output.Value);
            yield return new WaitForSecondsRealtime(1.5f);

            var value = listener.Output.Value;
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