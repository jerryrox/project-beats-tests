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
            Assert.IsFalse(multiFuture.DidRun);
            Assert.IsFalse(multiFuture.IsCompleted.Value);

            multiFuture.Start();
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
            Assert.IsFalse(multiFuture.DidRun);
            Assert.IsFalse(multiFuture.IsCompleted.Value);
            Assert.AreEqual(0f, multiFuture.Progress.Value, 0.001f);

            future.Start();
            Assert.IsFalse(multiFuture.DidRun);
            Assert.IsFalse(multiFuture.IsCompleted.Value);
            Assert.AreEqual(0f, multiFuture.Progress.Value, 0.001f);

            multiFuture.Start();
            Assert.IsTrue(multiFuture.DidRun);
            Assert.IsTrue(multiFuture.IsCompleted.Value);
            Assert.AreEqual(1f, multiFuture.Progress.Value, 0.001f);
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
            Assert.IsFalse(multiFuture.DidRun);
            Assert.IsFalse(multiFuture.IsCompleted.Value);
            Assert.AreEqual(0f, multiFuture.Progress.Value, 0.001f);

            multiFuture.Start();
            Assert.IsTrue(multiFuture.DidRun);
            Assert.IsTrue(multiFuture.IsCompleted.Value);
            Assert.AreEqual(1f, multiFuture.Progress.Value, 0.001f);
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
            multiFuture.Start();
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

        [UnityTest]
        public IEnumerator TestAwait()
        {
            List<Future> futures = Enumerable.Range(0, 10).Select(i => {
                var future = new Future((f) => UnityThread.StartCoroutine(DummyProcess(f)));
                future.Start();
                return future;
            }).ToList();

            MultiFuture multiFuture = new MultiFuture(futures);
            multiFuture.Start();
            Assert.AreEqual(10, multiFuture.Futures.Count);

            bool checkFinished = false;

            Action awaitFuture = async () =>
            {
                Assert.IsFalse(multiFuture.IsCompleted.Value);
                await multiFuture;
                Assert.IsTrue(multiFuture.IsCompleted.Value);
                futures.ForEach(f => Assert.IsTrue(f.IsCompleted.Value));
                Assert.AreEqual(1f, multiFuture.Progress.Value, 0.001f);
                checkFinished = true;
            };
            awaitFuture();

            while (!checkFinished)
            {
                yield return null;
            }
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