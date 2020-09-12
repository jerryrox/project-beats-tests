using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Tests
{
    public class MultiTaskTest {

        private const float Delta = 0.001f;


        [Test]
        public void TestInitialState()
        {
            var task = new MultiTask();
            Assert.IsNull(task.Listener);
            Assert.AreEqual(0, task.Tasks.Count);
            Assert.IsFalse(task.IsFinished);
            Assert.IsFalse(task.DidRun);
            Assert.IsFalse(task.IsRevoked.Value);
        }

        [Test]
        public void TestEmptyTasks()
        {
            var task = new MultiTask();
            var listener = new TaskListener();

            task.StartTask(listener);
            Assert.AreEqual(listener, task.Listener);
            Assert.AreEqual(0, task.Tasks.Count);
            Assert.IsTrue(task.IsFinished);
            Assert.IsTrue(task.DidRun);
            Assert.AreEqual(1f, listener.TotalProgress, Delta);
            Assert.IsTrue(listener.IsFinished);
        }

        [Test]
        public void TestWithAllFinishedTasks()
        {
            var tasks = new List<ManualTask>()
            {
                new ManualTask(),
                new ManualTask(),
                new ManualTask()
            };
            tasks.ForEach(t => t.SetFinished());

            var task = new MultiTask(tasks);
            var listener = new TaskListener();
            task.StartTask(listener);
            Assert.AreEqual(listener, task.Listener);
            Assert.AreEqual(3, task.Tasks.Count);
            Assert.IsTrue(task.IsFinished);
            Assert.IsTrue(task.DidRun);
            Assert.AreEqual(1f, listener.TotalProgress, Delta);
            Assert.IsTrue(listener.IsFinished);
        }

        [Test]
        public void TestWithPartialFinishedTasks()
        {
            var tasks = new List<ManualTask>()
            {
                new ManualTask(),
                new ManualTask((t) => {}),
                new ManualTask((t) => {})
            };

            var task = new MultiTask(tasks);
            var listener = new TaskListener();
            task.StartTask(listener);
            Assert.AreEqual(listener, task.Listener);
            Assert.AreEqual(3, task.Tasks.Count);
            Assert.IsFalse(task.IsFinished);
            Assert.IsTrue(task.DidRun);
            Assert.AreEqual(0.33333f, listener.TotalProgress, Delta);
            Assert.IsFalse(listener.IsFinished);

            tasks[1].SetFinished();
            Assert.IsFalse(task.IsFinished);
            Assert.AreEqual(0.66666f, listener.TotalProgress, Delta);
            Assert.IsFalse(listener.IsFinished);

            tasks[2].SetFinished();
            Assert.IsTrue(task.IsFinished);
            Assert.AreEqual(1f, listener.TotalProgress, Delta);
            Assert.IsTrue(listener.IsFinished);
        }

        [Test]
        public void TestWithNoFinishedTasks()
        {
            var tasks = new List<ManualTask>()
            {
                new ManualTask((t) => {}),
                new ManualTask((t) => {}),
                new ManualTask((t) => {})
            };

            var task = new MultiTask(tasks);
            var listener = new TaskListener();
            task.StartTask(listener);
            Assert.AreEqual(listener, task.Listener);
            Assert.AreEqual(3, task.Tasks.Count);
            Assert.IsFalse(task.IsFinished);
            Assert.IsTrue(task.DidRun);
            Assert.AreEqual(0f, listener.TotalProgress, Delta);
            Assert.IsFalse(listener.IsFinished);

            tasks.ForEach(t => t.SetFinished());
            Assert.IsTrue(task.IsFinished);
            Assert.AreEqual(1f, listener.TotalProgress, Delta);
            Assert.IsTrue(listener.IsFinished);
        }

        [Test]
        public void TestWithComplexTasks()
        {
            var manualTask = new ManualTask((t) => { });
            var multiTask = new MultiTask(new List<ITask>() {
                new ManualTask((t) => {}),
                new ManualTask((t) => {})
            });
            var tasks = new List<ITask>()
            {
                manualTask,
                multiTask
            };

            var task = new MultiTask(tasks);
            var listener = new TaskListener();
            task.StartTask(listener);
            Assert.AreEqual(listener, task.Listener);
            Assert.AreEqual(2, task.Tasks.Count);
            Assert.IsFalse(task.IsFinished);
            Assert.IsTrue(task.DidRun);
            Assert.AreEqual(0f, listener.TotalProgress, Delta);
            Assert.IsFalse(listener.IsFinished);

            (multiTask.Tasks[0] as ManualTask).SetFinished();
            Assert.IsFalse(task.IsFinished);
            Assert.AreEqual(0.25f, listener.TotalProgress, Delta);
            Assert.IsFalse(listener.IsFinished);

            manualTask.SetFinished();
            Assert.IsFalse(task.IsFinished);
            Assert.AreEqual(0.75f, listener.TotalProgress, Delta);
            Assert.IsFalse(listener.IsFinished);

            (multiTask.Tasks[1] as ManualTask).SetFinished();
            Assert.IsTrue(task.IsFinished);
            Assert.AreEqual(1f, listener.TotalProgress, Delta);
            Assert.IsTrue(listener.IsFinished);
        }

        [Test]
        public void TestRevoke()
        {
            var tasks = new List<ITask>()
            {
                new ManualTask(),
                new MultiTask(new List<ITask>() {
                    new ManualTask(),
                    new ManualTask()
                })
            };

            var task = new MultiTask(tasks);
            task.StartTask();
            Assert.IsTrue(task.IsFinished);

            task.RevokeTask(true);
            Assert.IsTrue(task.IsRevoked.Value);
            Assert.IsTrue((tasks[0] as ManualTask).IsRevoked.Value);
            Assert.IsTrue((tasks[1] as MultiTask).IsRevoked.Value);
            Assert.IsTrue(((tasks[1] as MultiTask).Tasks[0] as ManualTask).IsRevoked.Value);
            Assert.IsTrue(((tasks[1] as MultiTask).Tasks[1] as ManualTask).IsRevoked.Value);
        }
    }
}