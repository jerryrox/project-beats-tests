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
            var cacheReq = new CacheRequest<bool>(0, request);
            Assert.AreEqual(request, cacheReq.Request);
            Assert.AreEqual(0, cacheReq.Listeners.Count);

            var listener = cacheReq.Listen();
            Assert.IsNotNull(listener);
            Assert.AreEqual(1, cacheReq.Listeners.Count);
            Assert.IsFalse(listener.Value);

            var listener2 = cacheReq.Listen();
            Assert.IsNotNull(listener2);
            Assert.AreEqual(2, cacheReq.Listeners.Count);
            Assert.IsFalse(listener2.Value);

            cacheReq.Unlisten(new CacheListener<bool>(0));
            Assert.AreEqual(2, cacheReq.Listeners.Count);

            cacheReq.StartRequest();
            Assert.IsTrue(listener.Value);
            Assert.IsTrue(listener2.Value);
        }

        [Test]
        public void TestRemoveListener()
        {
            var request = new ManualTask<bool>((f) => f.SetFinished(true));
            var cacheReq = new CacheRequest<bool>(0, request);

            var listener = cacheReq.Listen();
            var listener2 = cacheReq.Listen();

            cacheReq.Unlisten(listener);
            Assert.AreEqual(1, cacheReq.Listeners.Count);
            cacheReq.StartRequest();
            Assert.IsFalse(listener.Value);
            Assert.IsTrue(listener2.Value);
        }
    }
}