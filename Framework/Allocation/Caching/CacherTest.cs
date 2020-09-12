using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;
using PBFramework.Tests;

namespace PBFramework.Allocation.Caching.Tests
{
    public class CacherTest
    {

        [UnityTest]
        public IEnumerator TestRequest()
        {
            var cacher = new DummyCacher();
            var listener = cacher.Request("Key1");
            var listener2 = cacher.Request("Key2");

            Assert.IsNull(listener.Value);
            Assert.IsNull(listener2.Value);
            Assert.IsFalse(cacher.IsCached("Key1"));
            Assert.IsFalse(cacher.IsCached("Key2"));

            // Premature checking
            yield return new WaitForSecondsRealtime(0.5f);
            Assert.IsNull(listener.Value);
            Assert.IsNull(listener2.Value);
            Assert.IsFalse(cacher.IsCached("Key1"));
            Assert.IsFalse(cacher.IsCached("Key2"));

            // Guaranteed finished checking
            float limit = 3f;
            while (!cacher.IsCached("Key1") || !cacher.IsCached("Key2"))
            {
                limit -= Time.deltaTime;
                if(limit <= 0)
                    Assert.Fail("Request should've finished by now.");
                yield return null;
            }
            Assert.AreEqual(1, listener.Progress);
            Assert.AreEqual(1, listener2.Progress);
            Assert.IsNotNull(listener.Value);
            Assert.IsNotNull(listener2.Value);
            Assert.IsTrue(cacher.IsCached("Key1"));
            Assert.IsTrue(cacher.IsCached("Key2"));
            Assert.IsFalse(cacher.IsCached("ASDF"));
            Assert.AreEqual("Key1", listener.Value.Key);
            Assert.AreEqual("Key2", listener2.Value.Key);
        }

        [UnityTest]
        public IEnumerator TestRemove()
        {
            var cacher = new DummyCacher();

            var listener = cacher.Request("AA");
            var listener2 = cacher.Request("BB");
            var listener3 = cacher.Request("CC");
            Assert.IsFalse(cacher.IsCached("AA"));
            Assert.IsFalse(cacher.IsCached("BB"));
            Assert.IsFalse(cacher.IsCached("CC"));

            yield return new WaitForSeconds(0.5f);
            cacher.Remove(listener);

            yield return new WaitForSeconds(2f);
            Assert.IsFalse(cacher.IsCached("AA"));
            Assert.IsTrue(cacher.IsCached("BB"));
            Assert.IsTrue(cacher.IsCached("CC"));
            Assert.IsNotNull(listener2.Value);
            Assert.IsNotNull(listener3.Value);

            var value2 = listener2.Value;
            var value3 = listener3.Value;
            Assert.AreEqual("BB", value2.Key);
            Assert.AreEqual("CC", value3.Key);
            Assert.IsFalse(value2.IsDestroyed);
            Assert.IsFalse(value3.IsDestroyed);

            cacher.Remove(listener2);
            Assert.IsFalse(cacher.IsCached("BB"));
            Assert.IsTrue(value2.IsDestroyed);

            cacher.RemoveDelayed(listener3, 1f);
            Assert.IsTrue(cacher.IsCached("CC"));
            Assert.IsFalse(value3.IsDestroyed);

            yield return new WaitForSeconds(0.5f);
            Assert.IsTrue(cacher.IsCached("CC"));
            Assert.IsFalse(value3.IsDestroyed);
            yield return new WaitForSeconds(1.5f);
            Assert.IsFalse(cacher.IsCached("CC"));
            Assert.IsTrue(value3.IsDestroyed);
        }

        [UnityTest]
        public IEnumerator TestDataLockRequest()
        {
            var cacher = new DummyCacher();
            var listeners = new CacheListener<DummyCacherData>[3];

            for (int i = 0; i < listeners.Length; i++)
                listeners[i] = cacher.Request("aa");

            yield return new WaitForSeconds(0.5f);
            Assert.IsFalse(cacher.IsCached("aa"));
            for (int i = 0; i < listeners.Length - 1; i++)
                cacher.Remove(listeners[i]);

            yield return new WaitForSeconds(1f);
            Assert.IsTrue(cacher.IsCached("aa"));
            for (int i = 0; i < listeners.Length; i++)
            {
                if(i < listeners.Length - 1)
                    Assert.IsNull(listeners[i].Value);
                else
                    Assert.IsNotNull(listeners[i].Value);
            }
            cacher.Remove(listeners[2]);
            Assert.IsFalse(cacher.IsCached("aa"));
            Assert.IsTrue(listeners[listeners.Length - 1].Value.IsDestroyed);
        }

        [UnityTest]
        public IEnumerator TestDataLockCached()
        {
            var cacher = new DummyCacher();
            var listeners = new CacheListener<DummyCacherData>[3];

            for (int i = 0; i < listeners.Length; i++)
                listeners[i] = cacher.Request("a");

            yield return new WaitForSeconds(1.5f);
            Assert.IsTrue(cacher.IsCached("a"));

            for (int i = 0; i < listeners.Length - 1; i++)
                cacher.Remove(listeners[i]);
            Assert.IsTrue(cacher.IsCached("a"));
            cacher.Remove(listeners[2]);
            Assert.IsFalse(cacher.IsCached("a"));
            Assert.IsTrue(listeners[listeners.Length - 1].Value.IsDestroyed);

        }
    }
}