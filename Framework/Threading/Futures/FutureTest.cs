using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Futures.Tests
{
    public class FutureTest
    {

        [Test]
        public void TestInitialization()
        {
            Future future = new Future();
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
            Future future = new Future();
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
                future.Start();
            });

            Future futureWithTask = new Future((f) => { });
            futureWithTask.Dispose();
            Assert.Throws<ObjectDisposedException>(() =>
            {
                futureWithTask.Start();
            });

            future.SetProgress(0.5f);
            Assert.AreEqual(0.5f, future.Progress.Value, 0.001f);
        }

        [Test]
        public void TestStartEmpty()
        {
            Future future = new Future();
            future.Start();
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsFalse(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            Assert.Throws<Exception>(() =>
            {
                future.Start();
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
            Future future = new Future((f) => { f.SetProgress(0.5f); });
            future.Start();
            Assert.AreEqual(0.5f, future.Progress.Value, 0.001f);
            Assert.IsFalse(future.IsDisposed.Value);
            Assert.IsFalse(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            Assert.Throws<Exception>(() =>
            {
                future.Start();
            });

            future.SetComplete();
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsFalse(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            Assert.Throws<Exception>(() =>
            {
                future.SetComplete();
            });

            future.Dispose();
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsTrue(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            future.SetComplete();
            Assert.AreEqual(1f, future.Progress.Value, 0.001f);
            Assert.IsTrue(future.IsDisposed.Value);
            Assert.IsTrue(future.IsCompleted.Value);
            Assert.IsNull(future.Error.Value);
            Assert.IsTrue(future.DidRun);

            future = new Future((f) => { });
            future.Start();
            future.SetFail(new Exception("a"));
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
            var future = new Future<int>((f) => { });
            future.Start();
            future.SetComplete(50);
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

                var future = new Future((f) => Task.Run(() =>
                {
                    taskThreadId = Thread.CurrentThread.ManagedThreadId;

                    Assert.AreNotEqual(mainThreadId, taskThreadId);

                    f.SetProgress(0.25f);
                    f.SetProgress(0.5f);
                    f.SetProgress(0.75f);
                    f.SetProgress(1f);
                    f.SetComplete();
                }))
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

                future.Start();

                while (!future.IsCompleted.Value)
                    yield return null;

                Assert.AreEqual(expectedProgress.Length, expectedProgressInx);
                Assert.AreEqual(1f, future.Progress.Value, 0.001f);
                Assert.IsTrue(future.IsCompleted.Value);
                Assert.IsFalse(future.IsDisposed.Value);

                shouldBeThreadSafe = !shouldBeThreadSafe;
                if (!shouldBeThreadSafe)
                    break;
            }
        }

        [UnityTest]
        public IEnumerator TestAwait()
        {
            Future future = new Future((f) => UnityThread.StartCoroutine(DummyLongProcess(f)));
            future.Start();

            bool finished = false;

            Action awaitFuture = async () =>
            {
                Assert.IsFalse(future.IsCompleted.Value);
                await future;
                Assert.IsTrue(future.IsCompleted.Value);
                Assert.AreEqual(1f, future.Progress.Value, 0.001f);
                finished = true;
            };
            awaitFuture();

            while (!finished)
            {
                yield return null;
            }
            Assert.IsNull(future.Error.Value);
        }

        [UnityTest]
        public IEnumerator TestAwaitAfterFinished()
        {
            Future<int> future = new Future<int>((f) => f.SetComplete(5));
            future.Start();

            bool finished = false;

            Action awaitFuture = async () =>
            {
                Assert.IsTrue(future.IsCompleted.Value);
                Assert.AreEqual(5, future.Output.Value);
                await future;
                Assert.IsTrue(future.IsCompleted.Value);
                Assert.AreEqual(5, future.Output.Value);
                finished = true;
            };
            awaitFuture();

            while (!finished)
            {
                yield return null;
            }
            Assert.IsNull(future.Error.Value);
        }

        private IEnumerator DummyLongProcess(Future future)
        {
            int i = 0;
            while (i < 100)
            {
                i++;
                future.SetProgress(i / 100f);
                yield return new WaitForSeconds(0.02f);
            }
            future.SetProgress(1f);
            future.SetComplete();
        }
    }
}