using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Futures.Tests
{
    public class ProxyFutureTest {
        
        [Test]
        public void TestStart()
        {
            int a = 0;
            ProxyFuture future = new ProxyFuture((f) => {
                a += 100;
                f.SetProgress(1f);
                f.SetComplete(null);
            });
            Assert.AreEqual(0, a);
            Assert.IsFalse(future.IsCompleted.Value);
            Assert.AreEqual(0f, future.Progress.Value, 0.001f);

            future.Start();
            Assert.AreEqual(100, a);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);

            Assert.Throws<Exception>(() => future.Start());
        }

        [Test]
        public void TestStartWithError()
        {
            ProxyFuture future = new ProxyFuture((f) => {
                f.SetFail(new Exception("asdf"));
            });
            Assert.IsFalse(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);

            future.Start();
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNotNull(future.Error.Value);
        }

        [Test]
        public void TestHandler()
        {
            int a = 0;
            ProxyFuture future = new ProxyFuture((f) => a = 50);
            future.Handler = (f) => a = 30;
            future.Start();
            Assert.AreEqual(30, a);

            Assert.Throws<Exception>(() => future.Handler = (f) => a = 10);
        }
    }
}