using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;

namespace PBFramework.Allocation.Caching.Tests
{
    public class CacheRequestTest {
        
        [Test]
        public void Test()
        {
            var request = new ManualTask<bool>((f) => f.SetFinished(true));
            var cacheReq = new CacheRequest<bool>(request);
            Assert.AreEqual(request, cacheReq.Request);
            Assert.AreEqual(0, cacheReq.ListenerCount);

            var listener = new TaskListener<bool>();
            var id = cacheReq.Listen(listener);
            Assert.Greater(id, 0);
            Assert.AreEqual(1, cacheReq.ListenerCount);
            Assert.IsFalse(listener.Value);

            var listener2 = new TaskListener<bool>();
            var id2 = cacheReq.Listen(listener2);
            Assert.Greater(id2, 0);
            Assert.AreEqual(2, cacheReq.ListenerCount);
            Assert.IsFalse(listener2.Value);

            cacheReq.Remove(10000);
            Assert.AreEqual(2, cacheReq.ListenerCount);

            cacheReq.StartRequest();
            Assert.IsTrue(listener.Value);
            Assert.IsTrue(listener2.Value);
        }

        [Test]
        public void TestRemoveListener()
        {
            var request = new ManualTask<bool>((f) => f.SetFinished(true));
            var cacheReq = new CacheRequest<bool>(request);

            var listener = new TaskListener<bool>();
            var id = cacheReq.Listen(listener);
            var listener2 = new TaskListener<bool>();
            var id2 = cacheReq.Listen(listener2);

            cacheReq.Remove(id);
            Assert.AreEqual(1, cacheReq.ListenerCount);
            cacheReq.StartRequest();
            Assert.IsFalse(listener.Value);
            Assert.IsTrue(listener2.Value);
        }
    }
}