using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Tests
{
    public class ManualTaskTest {

        private const float Delta = 0.001f;

        [Test]
        public void TestInitialState()
        {
            var task = new ManualTask();
            Assert.IsFalse(task.DidRun);
            Assert.IsFalse(task.IsFinished);
            Assert.IsFalse(task.IsRevoked.Value);
            Assert.IsNull(task.Listener);
        }

        [Test]
        public void TestStartEmpty()
        {
            var task = new ManualTask();
            var listener = new TaskListener();

            task.StartTask(listener);
            Assert.IsTrue(listener.IsFinished);
            Assert.AreEqual(1f, listener.Progress, Delta);
            Assert.IsTrue(task.DidRun);
            Assert.IsFalse(task.IsRevoked.Value);
            Assert.IsTrue(task.IsFinished);
            Assert.AreEqual(listener, task.Listener);
        }

        [Test]
        public void TestStart()
        {
            var task = new ManualTask((t) => {});
            var listener = new TaskListener();

            task.StartTask(listener);
            Assert.IsFalse(listener.IsFinished);
            Assert.AreEqual(0f, listener.Progress, Delta);
            Assert.IsTrue(task.DidRun);
            Assert.IsFalse(task.IsRevoked.Value);
            Assert.IsFalse(task.IsFinished);
            Assert.AreEqual(listener, task.Listener);

            task.SetProgress(0.25f);
            Assert.IsFalse(listener.IsFinished);
            Assert.AreEqual(0.25f, listener.Progress, Delta);
            Assert.IsTrue(task.DidRun);
            Assert.IsFalse(task.IsRevoked.Value);
            Assert.IsFalse(task.IsFinished);
            Assert.AreEqual(listener, task.Listener);

            task.SetFinished();
            Assert.IsTrue(listener.IsFinished);
            Assert.AreEqual(1f, listener.Progress, Delta);
            Assert.IsTrue(task.DidRun);
            Assert.IsFalse(task.IsRevoked.Value);
            Assert.IsTrue(task.IsFinished);
            Assert.AreEqual(listener, task.Listener);
        }

        [Test]
        public void TestOnFinished()
        {
            var task = new ManualTask();

            bool isFinished = false;
            task.OnFinished += () => isFinished = true;
            Assert.IsFalse(isFinished);

            task.StartTask();
            Assert.IsTrue(isFinished);

            isFinished = false;
            task.StartTask();
            Assert.IsFalse(isFinished);
        }

        [Test]
        public void TestGenericStart()
        {
            var task = new ManualTask<int>();
            var listener = new TaskListener<int>();

            task.StartTask(listener);
            Assert.IsTrue(listener.IsFinished);
            Assert.AreEqual(1f, listener.Progress, Delta);
            Assert.IsTrue(task.DidRun);
            Assert.IsFalse(task.IsRevoked.Value);
            Assert.IsTrue(task.IsFinished);
            Assert.AreEqual(listener, task.Listener);
            Assert.AreEqual(default(int), listener.Value);

            task = new ManualTask<int>((t) => t.SetFinished(100));
            listener = new TaskListener<int>();
            task.StartTask(listener);
            Assert.IsTrue(listener.IsFinished);
            Assert.AreEqual(1f, listener.Progress, Delta);
            Assert.IsTrue(task.DidRun);
            Assert.IsFalse(task.IsRevoked.Value);
            Assert.IsTrue(task.IsFinished);
            Assert.AreEqual(listener, task.Listener);
            Assert.AreEqual(100, listener.Value);
        }

        [Test]
        public void TestGenericOnFinished()
        {
            var task = new ManualTask<int>((t) => t.SetFinished(50));

            int value = 0;
            task.OnFinished += (v) => value = v;

            task.StartTask();
            Assert.AreEqual(50, value);
        }
    }
}