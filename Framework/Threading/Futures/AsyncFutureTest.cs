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
    }
}