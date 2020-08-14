using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Futures.Tests
{
    public class AsyncFutureTest {
        
        [UnityTest]
        public IEnumerator TestNormalRun()
        {
            int mainThreadId = Thread.CurrentThread.ManagedThreadId;

            AsyncFuture future = new AsyncFuture((f) =>
            {
                Assert.AreNotEqual(mainThreadId, Thread.CurrentThread.ManagedThreadId);
                f.SetComplete();
            });
            future.Start();

            while (!future.IsCompleted.Value)
            {
                yield return null;
            }

            Assert.IsNull(future.Error.Value);
        }

        [UnityTest]
        public IEnumerator TestNormalRunGeneric()
        {
            AsyncFuture<string> future = new AsyncFuture<string>((f) =>
            {
                f.SetComplete("hi");
            });
            future.Start();

            while (!future.IsCompleted.Value)
            {
                yield return null;
            }

            Assert.IsNull(future.Error.Value);
            Assert.AreEqual("hi", future.Output.Value);
        }

        [UnityTest]
        public IEnumerator TestAwait()
        {
            AsyncFuture<int> future = new AsyncFuture<int>((f) =>
            {
                try
                {
                    Thread.Sleep(3000);
                    f.SetProgress(1f);
                    f.SetComplete(512);
                }
                catch (Exception e)
                {
                    Debug.Log($"Error'd: ${e.ToString()}");
                    throw e;
                }
            });
            future.Start();

            bool finished = false;

            Action awaitFuture = async () =>
            {
                Assert.AreNotEqual(1f, future.Progress.Value);
                Assert.IsFalse(future.IsCompleted.Value);
                Assert.AreEqual(0, future.Output.Value);

                await future;

                Assert.AreEqual(1f, future.Progress.Value, 0.001f);
                Assert.IsTrue(future.IsCompleted.Value);
                Assert.AreEqual(512, future.Output.Value);

                finished = true;
            };
            awaitFuture();

            while (!finished)
            {
                yield return null;
            }
            Assert.IsNull(future.Error.Value);
        }
    }
}