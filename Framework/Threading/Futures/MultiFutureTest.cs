using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;

namespace PBFramework.Threading.Futures.Tests
{
    public class MultiFutureTest {
        
        [Test]
        public void TestEmpty()
        {
            MultiFuture multiFuture = new MultiFuture();
            Assert.IsTrue(multiFuture.DidRun);
            Assert.IsTrue(multiFuture.IsCompleted.Value);
        }

        [Test]
        public void TestStartAfterMultiFuture()
        {
            Future future = new Future((f) => {
                f.SetProgress(1f);
                f.SetComplete();
            });

            MultiFuture multiFuture = new MultiFuture(future);
            Assert.IsTrue(multiFuture.DidRun);
            Assert.IsFalse(multiFuture.IsCompleted.Value);
            Assert.AreEqual(0f, future.Progress.Value, 0.001f);

            future.Start();
            Assert.IsTrue(multiFuture.DidRun);
            Assert.IsTrue(multiFuture.IsCompleted.Value);
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
        }

        [Test]
        public void TestStartBeforeMultiFuture()
        {
            Future future = new Future((f) => {
                f.SetProgress(1f);
                f.SetComplete();
            });
            future.Start();

            MultiFuture multiFuture = new MultiFuture(future);
            Assert.IsTrue(multiFuture.DidRun);
            Assert.IsTrue(multiFuture.IsCompleted.Value);
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
        }

        [UnityTest]
        public IEnumerator TestMultiple()
        {
            List<Future> futures = Enumerable.Range(0, 5).Select(i => {
                var future = new Future((f) => UnityThread.StartCoroutine(DummyProcess(f)));
                future.Start();
                return future;
            }).ToList();

            MultiFuture multiFuture = new MultiFuture(futures);
            Assert.AreEqual(5, multiFuture.Futures.Count);

            float prevProgress = -1;
            while (!multiFuture.IsCompleted.Value)
            {
                Assert.GreaterOrEqual(multiFuture.Progress.Value, prevProgress);
                prevProgress = multiFuture.Progress.Value;
                yield return null;
            }

            Assert.AreEqual(1f, multiFuture.Progress.Value, 0.001f);
        }

        private IEnumerator DummyProcess(Future future)
        {
            int loops = UnityEngine.Random.Range(5, 20);
            for (int i = 0; i < loops; i++)
            {
                future.SetProgress((float)i / (float)loops);
                yield return new WaitForSeconds(0.1f);
            }
            future.SetProgress(1f);
            future.SetComplete();
        }
    }
}