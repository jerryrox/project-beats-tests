using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace PBFramework.Testing.Tests
{
    public class TestActionTest {

        [Test]
        public void TestAuto()
        {
            int invokeCount = 0;
            TestAction action = new TestAction(() => DummyAction(), "");
            
            Assert.IsNotNull(action.RunAction(false));
            Assert.AreEqual(0, invokeCount);

            Assert.IsNull(action.RunAction(true));
            Assert.AreEqual(1, invokeCount);
        }

        [Test]
        public void TestManual()
        {
            int invokeCount = 0;
            TestAction action = new TestAction(true, KeyCode.A, () => DummyAction(), "");
            
            Assert.IsNotNull(action.RunAction(false));
            Assert.AreEqual(0, invokeCount);

            Assert.IsNull(action.RunAction(true));
            Assert.AreEqual(0, invokeCount);
        }

        [Test]
        public void TestBoth()
        {
            int invokeCount = 0;
            TestAction action = new TestAction(false, KeyCode.A, () => DummyAction(), "");
            
            Assert.IsNotNull(action.RunAction(false));
            Assert.AreEqual(0, invokeCount);

            Assert.IsNull(action.RunAction(true));
            Assert.AreEqual(1, invokeCount);
        }

        [Test]
        public void TestUsage()
        {
            TestAction autoAction = new TestAction(() => DummyAction(), "Executes dummy action");
            Assert.AreEqual("Executes dummy action", autoAction.GetUsage());

            TestAction manualAction = new TestAction(true, KeyCode.A, () => DummyAction(), "Executes dummy action");
            Assert.AreEqual($"[KeyCode({KeyCode.A})] : Executes dummy action", manualAction.GetUsage());

            TestAction bothAction = new TestAction(true, KeyCode.A, () => DummyAction(), "Executes dummy action");
            Assert.AreEqual($"[KeyCode({KeyCode.A})] : Executes dummy action", bothAction.GetUsage());
        }

        private IEnumerator DummyAction() { yield break; }
    }
}