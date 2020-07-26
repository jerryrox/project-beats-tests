using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Futures
{
    public class FutureTest {
        
        [Test]
        public void TestInitialization()
        {
            DummyFuture future = new DummyFuture();
            Assert.AreEqual(0f, future.Progress.Value, 0.001f);
            Assert.IsFalse(future.IsDisposed.Value);
            Assert.IsFalse(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.IsThreadSafe);
            Assert.IsFalse(future.DidRun);
        }

        [Test]
        public void TestDispose()
        {
            DummyFuture future = new DummyFuture();
            future.Dispose();
            Assert.AreEqual(0f, future.Progress.Value, 0.001f);
            Assert.IsTrue(future.IsDisposed.Value);
            Assert.IsFalse(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.IsThreadSafe);
            Assert.IsFalse(future.DidRun);

            Assert.Throws<ObjectDisposedException>(() =>
            {
                future.Dispose();
            });
            Assert.Throws<ObjectDisposedException>(() =>
            {
                future.StartEmpty();
            });
            Assert.Throws<ObjectDisposedException>(() =>
            {
                future.Start(() => {});
            });

            future.SetProgress(0.5f);
            Assert.AreEqual(0.5f, future.Progress.Value, 0.001f);
        }

        [Test]
        public void TestStartEmpty()
        {
            DummyFuture future = new DummyFuture();
            future.StartEmpty();
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsFalse(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            Assert.Throws<Exception>(() =>
            {
                future.StartEmpty();
            });

            future.Dispose();
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsTrue(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);
        }


        [Test]
        public void TestStart()
        {
            DummyFuture future = new DummyFuture();
            future.Start(() => { future.SetProgress(0.5f); });
            Assert.AreEqual(0.5f, future.Progress.Value, 0.001f);
            Assert.IsFalse(future.IsDisposed.Value);
            Assert.IsFalse(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            Assert.Throws<Exception>(() =>
            {
                future.Start(() => {});
            });

            future.SetComplete(null);
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsFalse(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            Assert.Throws<Exception>(() =>
            {
                future.SetComplete(null);
            });

            future.Dispose();
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsTrue(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            future.SetComplete(null);
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsTrue(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            future = new DummyFuture();
            future.Start(() => { });
            future.SetComplete(new Exception("a"));
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsFalse(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNotNull(future.Error.Value);
            Assert.AreEqual("a", future.Error.Value.Message);
            Assert.IsTrue(future.DidRun);
        }

        [Test]
        public void TestWithOutput()
        {
            var future = new DummyFuture<int>();
            future.Start(() => { });
            future.SetComplete(50, null);
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsFalse(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);
            Assert.AreEqual(50, future.Output.Value);
        }

        [UnityTest]
        public IEnumerator TestThreadSafety()
        {
            bool shouldBeThreadSafe = false;
            
            while (true)
            {
                int mainThreadId = Thread.CurrentThread.ManagedThreadId;
                int taskThreadId = mainThreadId;

                var future = new DummyFuture()
                {
                    IsThreadSafe = shouldBeThreadSafe
                };

                float[] expectedProgress = new float[] { 0.25f, 0.5f, 0.75f, 1f };
                int expectedProgressInx = 0;
                future.Progress.OnNewValue += (progress) =>
                {
                    int threadIdToCheckInEvent = shouldBeThreadSafe ? mainThreadId : taskThreadId;
                    try
                    {
                        Assert.AreEqual(threadIdToCheckInEvent, Thread.CurrentThread.ManagedThreadId);
                        Assert.AreEqual(expectedProgress[expectedProgressInx++], progress, 0.001f);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                };

                future.IsCompleted.OnNewValue += (isCompleted) =>
                {
                    int threadIdToCheckInEvent = shouldBeThreadSafe ? mainThreadId : taskThreadId;
                    try
                    {
                        Assert.AreEqual(threadIdToCheckInEvent, Thread.CurrentThread.ManagedThreadId);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                };

                Task asyncTask = new Task(() =>
                {
                    taskThreadId = Thread.CurrentThread.ManagedThreadId;

                    Assert.AreNotEqual(mainThreadId, taskThreadId);

                    future.SetProgress(0.25f);
                    future.SetProgress(0.5f);
                    future.SetProgress(0.75f);
                    future.SetProgress(1f);
                    future.SetComplete(null);
                });
                future.Start(asyncTask.Start);

                while(!future.IsCompleted.Value)
                    yield return null;

                Assert.AreEqual(expectedProgress.Length, expectedProgressInx);
                Assert.AreEqual(1f, future.Progress.Value, 0.001f);
                Assert.IsTrue(future.IsCompleted.Value);
                Assert.IsFalse(future.IsDisposed.Value);

                shouldBeThreadSafe = !shouldBeThreadSafe;
                if(!shouldBeThreadSafe)
                    break;
            }
        }

        private class DummyFuture : Future
        {
            public void StartEmpty() => StartRunning(null);

            public void Start(Action task) => StartRunning(task);

            public void SetProgress(float progress) => ReportProgress(progress);

            public void SetComplete(Exception exception) => OnComplete(exception);
        }

        private class DummyFuture<T> : Future<T>
        {
            public void StartEmpty() => StartRunning(null);

            public void Start(Action task) => StartRunning(task);

            public void SetProgress(float progress) => ReportProgress(progress);

            public void SetComplete(T value, Exception exception) => OnComplete(value, exception);
        }
    }
}