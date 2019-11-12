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
            var listener = new ReturnableProgress<Dummy>();
            var listener2 = new ReturnableProgress<Dummy>();

            var cacher = new DummyCacher();
            cacher.Request("Key1", listener);
            cacher.Request("Key2", listener2);

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
            var listener = new ReturnableProgress<Dummy>();
            var listener2 = new ReturnableProgress<Dummy>();
            var listener3 = new ReturnableProgress<Dummy>();

            var id = cacher.Request("AA", listener);
            var id2 = cacher.Request("BB", listener2);
            var id3 = cacher.Request("CC", listener3);
            Assert.IsFalse(cacher.IsCached("AA"));
            Assert.IsFalse(cacher.IsCached("BB"));
            Assert.IsFalse(cacher.IsCached("CC"));

            yield return new WaitForSeconds(0.5f);
            cacher.Remove("AA", id);

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

            cacher.Remove("BB", 0);
            Assert.IsFalse(cacher.IsCached("BB"));
            Assert.IsTrue(value2.IsDestroyed);

            cacher.RemoveDelayed("CC", 0, 1f);
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
            var listeners = new ReturnableProgress<Dummy>[] {
                new ReturnableProgress<Dummy>(),
                new ReturnableProgress<Dummy>(),
                new ReturnableProgress<Dummy>(),
            };
            var ids = new uint[3];

            for (int i = 0; i < listeners.Length; i++)
                ids[i] = cacher.Request("aa", listeners[i]);

            yield return new WaitForSeconds(0.5f);
            Assert.IsFalse(cacher.IsCached("aa"));
            for (int i = 0; i < listeners.Length - 1; i++)
                cacher.Remove("aa", ids[i]);

            yield return new WaitForSeconds(1f);
            Assert.IsTrue(cacher.IsCached("aa"));
            for (int i = 0; i < listeners.Length; i++)
            {
                if(i < listeners.Length - 1)
                    Assert.IsNull(listeners[i].Value);
                else
                    Assert.IsNotNull(listeners[i].Value);
            }
            cacher.Remove("aa", 0);
            Assert.IsFalse(cacher.IsCached("aa"));
            Assert.IsTrue(listeners[listeners.Length - 1].Value.IsDestroyed);
        }

        [UnityTest]
        public IEnumerator TestDataLockCached()
        {
            var cacher = new DummyCacher();
            var listeners = new ReturnableProgress<Dummy>[] {
                new ReturnableProgress<Dummy>(),
                new ReturnableProgress<Dummy>(),
                new ReturnableProgress<Dummy>(),
            };

            for (int i = 0; i < listeners.Length; i++)
                cacher.Request("a", listeners[i]);

            yield return new WaitForSeconds(1.5f);
            Assert.IsTrue(cacher.IsCached("a"));

            for (int i = 0; i < listeners.Length - 1; i++)
                cacher.Remove("a", 0);
            Assert.IsTrue(cacher.IsCached("a"));
            cacher.Remove("a", 0);
            Assert.IsFalse(cacher.IsCached("a"));
            Assert.IsTrue(listeners[listeners.Length - 1].Value.IsDestroyed);

        }


        private class DummyCacher : Cacher<string, Dummy> {

            public override string StringifyKey(string key) => key;

            protected override IPromise<Dummy> CreateRequest(string key) => new Requester(key);

            protected override void DestroyData(Dummy data)
            {
                data.IsDestroyed = true;
            }
        }

        private class Requester : DummyRequester<Dummy>
        {
            private string key;
            private SynchronizedTimer timer;


            public Requester(string key) => this.key = key;

            public override void Start()
            {
                if(timer != null) return;

                base.Start();
                timer = new SynchronizedTimer()
                {
                    Limit = 1f
                };
                timer.OnFinished += delegate
                {
                    DoFinish(new Dummy() {
                        Key = key,
                        IsDestroyed = false
                    });
                    timer = null;
                };
                timer.Start();
            }

            public override void Revoke()
            {
                if(timer == null) return;

                base.Revoke();
                timer.Stop();
                timer = null;
            }
        }

        private class Dummy {

            public string Key { get; set; } = null;

            public bool IsDestroyed { get; set; } = false;
        }
}
}