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
        public IEnumerable TestNormalRun()
        {
            int mainThreadId = Thread.CurrentThread.ManagedThreadId;

            AsyncFuture future = new AsyncFuture((f) =>
            {
                Assert.AreNotEqual(mainThreadId, Thread.CurrentThread.ManagedThreadId);
                f.SetComplete(null);
            });
            future.Start();

            while (!future.IsCompleted.Value)
            {
                yield return null;
            }

            Assert.IsNull(future.Error.Value);
        }
    }
}