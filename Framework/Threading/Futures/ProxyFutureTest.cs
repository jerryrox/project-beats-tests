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
        public void TestDisposeFromProxyFuture()
        {
            Future<int> future = new Future<int>((f) => f.SetComplete(50));
            ProxyFuture<int> proxyFuture = new ProxyFuture<int>(future);

            proxyFuture.Dispose();
            Assert.IsTrue(future.IsDisposed.Value);
            Assert.IsTrue(proxyFuture.IsDisposed.Value);
            Assert.IsFalse(future.IsCompleted.Value);
            Assert.IsFalse(proxyFuture.IsCompleted.Value);
        }

        [Test]
        public void TestStartFromProxyFuture()
        {
            Future<int> future = new Future<int>((f) => f.SetComplete(4));
            ProxyFuture<int> proxyFuture = new ProxyFuture<int>(future);
            Assert.IsFalse(future.DidRun);
            Assert.IsFalse(proxyFuture.DidRun);

            proxyFuture.Start();
            Assert.IsTrue(future.DidRun);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsTrue(proxyFuture.DidRun);
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.AreEqual(4, future.Output.Value);
            Assert.AreEqual(4, proxyFuture.Output.Value);
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

        [Test]
        public void TestGeneric()
        {
            Future<int> future = new Future<int>((f) => f.SetComplete(1000));

            ProxyFuture<int> proxyFuture = new ProxyFuture<int>(future as IControlledFuture<int>);
            Assert.AreEqual(0f, proxyFuture.Progress.Value, Delta);
            Assert.IsFalse(proxyFuture.IsCompleted.Value);
            Assert.AreEqual(0, proxyFuture.Output.Value);

            future.SetProgress(1f);
            Assert.AreEqual(1f, proxyFuture.Progress.Value, Delta);

            future.Start();
            Assert.AreEqual(1000, proxyFuture.Output.Value);
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
        }

        [Test]
        public void TestGenericCompleted()
        {
            Future<int> future = new Future<int>((f) => f.SetComplete(1000));
            future.Start();

            ProxyFuture<int> proxyFuture = new ProxyFuture<int>(future as IControlledFuture<int>);
            Assert.AreEqual(1f, proxyFuture.Progress.Value, Delta);
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.AreEqual(1000, proxyFuture.Output.Value);
        }

        [Test]
        public void TestGenericErrored()
        {
            Future<int> future = new Future<int>((f) => f.SetFail(new Exception()));
            ProxyFuture<int> proxyFuture = new ProxyFuture<int>(future as IControlledFuture<int>);

            future.Start();
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.IsNotNull(proxyFuture.Error.Value);
            Assert.AreEqual(0, proxyFuture.Output.Value);
        }

        [Test]
        public void TestGenericErroredFromStart()
        {
            Future<int> future = new Future<int>((f) => f.SetFail(new Exception()));
            future.Start();

            ProxyFuture<int> proxyFuture = new ProxyFuture<int>(future as IControlledFuture<int>);
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.IsNotNull(proxyFuture.Error.Value);
            Assert.AreEqual(0, proxyFuture.Output.Value);
        }

        [Test]
        public void TestGenericDispose()
        {
            Future<int> future = new Future<int>((f) => f.SetComplete(1));
            ProxyFuture<int> proxyFuture = new ProxyFuture<int>(future as IControlledFuture<int>);

            future.Start();
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.IsFalse(proxyFuture.IsDisposed.Value);
            Assert.IsNull(proxyFuture.Error.Value);
            Assert.AreEqual(1, proxyFuture.Output.Value);

            future.Dispose();
            Assert.IsTrue(proxyFuture.IsCompleted.Value);
            Assert.IsTrue(proxyFuture.IsDisposed.Value);
            Assert.IsNull(proxyFuture.Error.Value);
            Assert.AreEqual(1, proxyFuture.Output.Value);
        }


        [Test]
        public void TestGenericDisposedFromStart()
        {
            Future<int> future = new Future<int>((f) => f.SetComplete(1));
            future.Start();
            future.Dispose();

            ProxyFuture<int> proxyFuture = new ProxyFuture<int>(future as IControlledFuture<int>);
            Assert.IsFalse(proxyFuture.IsCompleted.Value);
            Assert.IsTrue(proxyFuture.IsDisposed.Value);
            Assert.IsNull(proxyFuture.Error.Value);
            Assert.AreEqual(0, proxyFuture.Output.Value);
        }

        [Test]
        public void TestGenericWithDifferentType()
        {
            Future<int> future = new Future<int>((f) => f.SetComplete(100));

            ProxyFuture<int, string> proxyFuture = new DummyProxyFuture(future);
            Assert.AreEqual("0", proxyFuture.Output.Value);

            future.Start();
            Assert.AreEqual("100", proxyFuture.Output.Value);
        }

        private class DummyProxyFuture : ProxyFuture<int, string>
        {
            public DummyProxyFuture(IFuture<int> future) : base(future)
            {
            }

            public DummyProxyFuture(IControlledFuture<int> future) : base(future)
            {
            }

            protected override string ConvertOutput(int source) => source.ToString();
        }
    }
}