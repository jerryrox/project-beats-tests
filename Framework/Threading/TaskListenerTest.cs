using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Tests
{
    public class TaskListenerTest {

        private const float Delta = 0.001f;

        [Test]
        public void TestProgress()
        {
            var listener = new TaskListener();
            float progress = 0f;
            listener.OnProgress += (p) => progress = p;
            Assert.AreEqual(0f, listener.Progress, Delta);
            Assert.AreEqual(0f, listener.TotalProgress, Delta);
            Assert.AreEqual(0f, progress, Delta);

            listener.SetProgress(-1f);
            Assert.AreEqual(0f, listener.Progress, Delta);
            Assert.AreEqual(0f, listener.TotalProgress, Delta);
            Assert.AreEqual(0f, progress, Delta);

            listener.SetProgress(0.5f);
            Assert.AreEqual(0.5f, listener.Progress, Delta);
            Assert.AreEqual(0.5f, listener.TotalProgress, Delta);
            Assert.AreEqual(0.5f, progress, Delta);

            listener.SetProgress(1.1f);
            Assert.AreEqual(1f, listener.Progress, Delta);
            Assert.AreEqual(1f, listener.TotalProgress, Delta);
            Assert.AreEqual(1f, progress, Delta);
        }

        [Test]
        public void TestSubListener()
        {
            var listener = new TaskListener();
            Assert.AreEqual(0, listener.SubListenerCount);

            var sub = listener.CreateSubListener();
            Assert.AreEqual(1, listener.SubListenerCount);

            var sub2 = listener.CreateSubListener();
            Assert.AreEqual(2, listener.SubListenerCount);
        }

        [Test]
        public void TestTotalProgress()
        {
            var listener = new TaskListener();
            float progress = 0f;
            listener.OnProgress += (p) => progress = p;

            listener.SetProgress(0.4f);
            Assert.AreEqual(0.4f, listener.TotalProgress, Delta);
            Assert.AreEqual(0.4f, progress, Delta);
            Assert.AreEqual(0.4f, listener.Progress, Delta);

            var sub = listener.CreateSubListener();
            float subProgress = 0f;
            sub.OnProgress += (p) => subProgress = p;
            Assert.AreEqual(0.2f, listener.TotalProgress, Delta);
            Assert.AreEqual(0.2f, progress, Delta);
            Assert.AreEqual(0.4f, listener.Progress, Delta);
            Assert.AreEqual(0f, subProgress, Delta);
            Assert.AreEqual(0f, sub.Progress, Delta);

            sub.SetProgress(1f);
            Assert.AreEqual(0.7f, listener.TotalProgress, Delta);
            Assert.AreEqual(0.7f, progress, Delta);
            Assert.AreEqual(0.4f, listener.Progress, Delta);
            Assert.AreEqual(1f, subProgress, Delta);
            Assert.AreEqual(1f, sub.Progress, Delta);
        }

        [Test]
        public void TestFinished()
        {
            var listener = new TaskListener();
            bool isFinished = false;
            listener.OnFinished += () => isFinished = true;
            Assert.IsFalse(listener.IsFinished);
            Assert.IsFalse(isFinished);
            Assert.AreEqual(0f, listener.Progress, Delta);

            listener.SetFinished();
            Assert.IsTrue(listener.IsFinished);
            Assert.IsTrue(isFinished);
            Assert.AreEqual(1f, listener.Progress, Delta);
        }

        [UnityTest]
        public IEnumerator TestEventOnMainThread()
        {
            UnityThread.Initialize();

            var listener = new TaskListener();
            int mainThread = Thread.CurrentThread.ManagedThreadId;

            int finishedThreadFlag = 0; // 0 = idle, 1 = main, 2 = other
            int progressThreadFlag = 0;
            listener.OnFinished += () =>
            {
                finishedThreadFlag = (
                    Thread.CurrentThread.ManagedThreadId == mainThread ?
                    1 : -1
                );
            };
            listener.OnProgress += (p) =>
            {
                progressThreadFlag = (
                    Thread.CurrentThread.ManagedThreadId == mainThread ?
                    1 : -1
                );
            };

            Assert.AreEqual(0, finishedThreadFlag);
            Assert.AreEqual(0, progressThreadFlag);

            bool taskRan = false;
            Task.Run(() =>
            {
                listener.SetProgress(1f);
                listener.SetFinished();
                taskRan = true;
            });
            while (!taskRan)
                yield return null;
            Assert.AreEqual(1, finishedThreadFlag);
            Assert.AreEqual(1, progressThreadFlag);
        }

        [UnityTest]
        public IEnumerator TestEventOnOtherThread()
        {
            UnityThread.Initialize();

            var listener = new TaskListener();
            int mainThread = Thread.CurrentThread.ManagedThreadId;

            int finishedThreadFlag = 0; // 0 = idle, 1 = main, 2 = other
            int progressThreadFlag = 0;
            listener.OnFinished += () =>
            {
                finishedThreadFlag = (
                    Thread.CurrentThread.ManagedThreadId == mainThread ?
                    1 : -1
                );
            };
            listener.OnProgress += (p) =>
            {
                progressThreadFlag = (
                    Thread.CurrentThread.ManagedThreadId == mainThread ?
                    1 : -1
                );
            };

            Assert.AreEqual(0, finishedThreadFlag);
            Assert.AreEqual(0, progressThreadFlag);

            bool taskRan = false;
            listener.CallEventOnMainThread.Value = false;
            Task.Run(() =>
            {
                listener.SetProgress(1f);
                listener.SetFinished();
                taskRan = true;
            });
            while (!taskRan)
                yield return null;

            Assert.AreEqual(-1, finishedThreadFlag);
            Assert.AreEqual(-1, progressThreadFlag);
        }

        [Test]
        public void TestGeneric()
        {
            var listener = new TaskListener<int>();
            int result = 0;
            listener.OnFinished += (v) => result = v;
            Assert.AreEqual(default(int), listener.Value);
            Assert.AreEqual(default(int), result);

            listener.SetValue(5);
            Assert.AreEqual(5, listener.Value);
            Assert.AreEqual(0, result);

            listener.SetFinished();
            Assert.AreEqual(5, listener.Value);
            Assert.AreEqual(5, result);
        }

        [Test]
        public void TestFinishedState()
        {
            var listener = new TaskListener<string>();
            listener.SetValue("lolz");
            listener.SetFinished();

            Assert.AreEqual("lolz", listener.Value);
            Assert.IsTrue(listener.IsFinished);
            Assert.AreEqual(1f, listener.TotalProgress, Delta);

            listener.SetProgress(0.5f);
            listener.SetValue("a");
            Assert.AreEqual("lolz", listener.Value);
            Assert.IsTrue(listener.IsFinished);
            Assert.AreEqual(1f, listener.TotalProgress, Delta);
        }

        [Test]
        public void TestHasOwnProgress()
        {
            var listener = new TaskListener();
            Assert.IsTrue(listener.HasOwnProgress);

            listener.SetProgress(0.5f);
            Assert.AreEqual(0.5f, listener.Progress, Delta);
            Assert.AreEqual(0.5f, listener.TotalProgress, Delta);

            var subListener = listener.CreateSubListener();
            Assert.AreEqual(0.5f, listener.Progress, Delta);
            Assert.AreEqual(0f, subListener.Progress, Delta);
            Assert.AreEqual(0.25f, listener.TotalProgress, Delta);

            listener.HasOwnProgress = false;
            Assert.IsFalse(listener.HasOwnProgress);
            Assert.AreEqual(0.5f, listener.Progress, Delta);
            Assert.AreEqual(0f, subListener.Progress, Delta);
            Assert.AreEqual(0f, listener.TotalProgress, Delta);

            listener.SetProgress(1f);
            subListener.SetProgress(0.25f);
            Assert.AreEqual(1f, listener.Progress, Delta);
            Assert.AreEqual(0.25f, subListener.Progress, Delta);
            Assert.AreEqual(0.25f, listener.TotalProgress, Delta);
        }

        [Test]
        public void TestAutoFinish()
        {
            var listener = new TaskListener();
            Assert.IsFalse(listener.IsAutoFinish);
            Assert.IsFalse(listener.IsFinished);
            listener.SetFinished();
            Assert.IsTrue(listener.IsFinished);

            listener = new TaskListener();
            listener.IsAutoFinish = true;
            Assert.IsFalse(listener.IsFinished);
            Assert.IsTrue(listener.IsAutoFinish);
            listener.SetFinished();
            Assert.IsTrue(listener.IsFinished);

            listener = new TaskListener();
            var sub = listener.CreateSubListener();
            sub.SetFinished();
            Assert.IsFalse(listener.IsFinished);
            var sub2 = listener.CreateSubListener();
            var sub3 = listener.CreateSubListener();
            listener.IsAutoFinish = true;
            Assert.IsFalse(listener.IsFinished);
            sub2.SetFinished();
            Assert.IsFalse(listener.IsFinished);
            sub3.SetFinished();
            Assert.IsTrue(listener.IsFinished);
        }
    }
}