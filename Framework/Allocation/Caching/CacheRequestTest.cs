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
            var request = new DummyRequest();
            var cacheReq = new CacheRequest<bool>(request);
            Assert.AreEqual(request, cacheReq.Request);
            Assert.AreEqual(0, cacheReq.ListenerCount);

            var listener = new ReturnableProgress<bool>();
            var id = cacheReq.Listen(listener);
            Assert.Greater(id, 0);
            Assert.AreEqual(1, cacheReq.ListenerCount);
            Assert.IsFalse(listener.Value);

            var listener2 = new ReturnableProgress<bool>();
            var id2 = cacheReq.Listen(listener2);
            Assert.Greater(id2, 0);
            Assert.AreEqual(2, cacheReq.ListenerCount);
            Assert.IsFalse(listener2.Value);

            cacheReq.Remove(10000);
            Assert.AreEqual(2, cacheReq.ListenerCount);

            request.Start();
            request.InvokeFinish(true);
            Assert.IsTrue(listener.Value);
            Assert.IsTrue(listener2.Value);

            cacheReq.Remove(id);
            Assert.AreEqual(1, cacheReq.ListenerCount);
            request.InvokeFinish(false);
            Assert.IsTrue(listener.Value);
            Assert.IsFalse(listener2.Value);
        }

        private class DummyRequest : IPromise<bool>
        {
            public event Action<bool> OnFinishedResult;
            public event Action OnFinished
            {
                add => OnFinishedResult += (v) => value();
                remove => OnFinishedResult -= (v) => value();
            }

            public event Action<float> OnProgress;

            public bool Result { get; set; } = false;
            object IPromise.Result => Result;

            public bool IsFinished { get; private set; }

            public float Progress { get; }

            public void Start()
            {
                IsFinished = false;
            }

            public void Revoke()
            {
                IsFinished = false;
            }

            public void InvokeFinish(bool result)
            {
                IsFinished = true;
                Result = result;
                OnFinishedResult?.Invoke(result);
            }
        }
    }
}