using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Futures.Tests
{
    public class ProxyFutureTest {

        private const float Delta = 0.001f;

        [Test]
        public void TestExceptions()
        {
            Assert.Throws<ArgumentNullException>(() => new ProxyFuture(null));
            Assert.Throws<Exception>(() => new ProxyFuture(new Future() as IFuture));

            ProxyFuture proxyFuture = new ProxyFuture(new Future() as IControlledFuture);
            Assert.Throws<InvalidOperationException>(() => proxyFuture.SetComplete());
            Assert.Throws<InvalidOperationException>(() => proxyFuture.SetFail(null));
            Assert.Throws<InvalidOperationException>(() => proxyFuture.SetProgress(1f));
        }

        [Test]
        public void TestControlledComplete()
        {
            Future future = new Future();

            ProxyFuture proxyFuture = new ProxyFuture(future as IControlledFuture);
            Assert.AreEqual(0f, proxyFuture.Progress.Value, Delta);
            Assert.IsFalse(proxyFuture.IsCompleted.Value);
            Assert.IsFalse(proxyFuture.IsDisposed.Value);

            future.SetProgress(0.5f);
            Assert.AreEqual(0.5f, proxyFuture.Progress.Value, Delta);

            future.SetComplete();
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.IsFalse(proxyFuture.IsDisposed.Value);

            future.Dispose();
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.IsTrue(proxyFuture.IsDisposed.Value);
        }


        [Test]
        public void TestUncontrolledComplete()
        {
            Future future = new Future((f) => {
                f.SetProgress(1f);
                f.SetComplete();
            });
            future.Start();

            ProxyFuture proxyFuture = new ProxyFuture(future as IFuture);
            Assert.AreEqual(1f, proxyFuture.Progress.Value, Delta);
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.IsFalse(proxyFuture.IsDisposed.Value);

            future.SetProgress(0.5f);
            Assert.AreEqual(0.5f, proxyFuture.Progress.Value, Delta);

            future.Dispose();
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.IsTrue(proxyFuture.IsDisposed.Value);
        }

        [Test]
        public void TestCreateWithDisposed()
        {
            Future future = new Future();
            future.Dispose();

            ProxyFuture proxyFuture = new ProxyFuture(future as IControlledFuture);
            Assert.AreEqual(0f, proxyFuture.Progress.Value, Delta);
            Assert.IsTrue(proxyFuture.IsDisposed.Value);

            Assert.Throws<ObjectDisposedException>(() => proxyFuture.Start());
            Assert.Throws<ObjectDisposedException>(() => proxyFuture.Dispose());

            future.SetProgress(0.5f);
            Assert.AreEqual(0.5f, future.Progress.Value, Delta);
            Assert.AreEqual(0f, proxyFuture.Progress.Value, Delta);
        }
    }
}