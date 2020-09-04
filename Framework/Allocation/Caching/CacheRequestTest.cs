using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;
using PBFramework.Threading.Futures;

namespace PBFramework.Allocation.Caching.Tests
{
    public class CacheRequestTest {
        
        [Test]
        public void Test()
        {
            var request = new Future<bool>((f) => f.SetComplete(true));
            var cacheReq = new CacheRequest<bool>("asdf", request);
            Assert.AreEqual(request, cacheReq.Request);
            Assert.AreEqual(0, cacheReq.Listeners.Count);

            var listener = cacheReq.Listen();
            Assert.IsNotNull(listener);
            Assert.AreEqual(1, cacheReq.Listeners.Count);
            Assert.IsFalse(listener.Output.Value);

            var listener2 = cacheReq.Listen();
            Assert.IsNotNull(listener);
            Assert.AreNotEqual(listener, listener2);
            Assert.AreEqual(2, cacheReq.Listeners.Count);
            Assert.IsFalse(listener2.Output.Value);

            cacheReq.Unlisten(new CacheListener<bool>("fdsa", null));
            cacheReq.Unlisten(null);
            Assert.AreEqual(2, cacheReq.Listeners.Count);

            request.Start();
            Assert.IsTrue(listener.Output.Value);
            Assert.IsTrue(listener2.Output.Value);
        }

        [Test]
        public void TestRemoveListener()
        {
            var request = new Future<bool>((f) => f.SetComplete(true));
            var cacheReq = new CacheRequest<bool>("asdf", request);

            var listener = cacheReq.Listen();
            var listener2 = cacheReq.Listen();

            cacheReq.Unlisten(listener);
            Assert.AreEqual(1, cacheReq.Listeners.Count);
            request.Start();
            Assert.IsFalse(listener.Output.Value);
            Assert.IsTrue(listener2.Output.Value);
        }

        [Test]
        public void TestDispose()
        {
            var request = new Future<bool>((f) => f.SetComplete(true));
            var cacheReq = new CacheRequest<bool>("asdf", request);
            var listener = cacheReq.Listen();
            var listener2 = cacheReq.Listen();
            cacheReq.Dispose();

            Assert.AreEqual(0, cacheReq.Listeners.Count);
            Assert.Throws<ObjectDisposedException>(() => cacheReq.Listen());
            Assert.Throws<ObjectDisposedException>(() => cacheReq.Unlisten(listener));
            Assert.IsNull(cacheReq.Request);
        }
    }
}