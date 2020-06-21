using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace PBFramework.Testing.Tests
{
    public class TestKeyBindingTest {

        [Test]
        public void Test()
        {
            int invokeCount = 0;
            TestKeyBinding binding = new TestKeyBinding(KeyCode.A, (isAuto) => { invokeCount++; return DummyAction(); }, "Executes dummy action");

            Assert.IsNotNull(binding.RunAction(false));
            Assert.AreEqual(1, invokeCount);

            Assert.IsNull(binding.RunAction(true));
            Assert.AreEqual(1, invokeCount);
        }

        [Test]
        public void TestUsage()
        {
            TestKeyBinding binding = new TestKeyBinding(KeyCode.A, null, "Executes dummy action");
            Assert.AreEqual($"[KeyCode({KeyCode.A})] : Executes dummy action", binding.GetUsage());
        }

        [Test]
        public void TestForceManual()
        {
            int invokeCount = 0;
            TestKeyBinding binding = new TestKeyBinding(KeyCode.A, (isAuto) => { invokeCount++; return DummyAction(); }, "Executes dummy action");
            binding.ForceManual = true;

            Assert.IsNull(binding.RunAction(false));
            Assert.AreEqual(0, invokeCount);

            Assert.IsNull(binding.RunAction(true));
            Assert.AreEqual(0, invokeCount);
        }

        private IEnumerator DummyAction() { yield break; }
    }
}