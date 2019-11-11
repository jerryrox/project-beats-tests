using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Tests
{
    public class EventProgressTest {

        private const float Delta = 0.0000001f;

        [Test]
        public void Test()
        {
            var progress = new EventProgress();
            Assert.AreEqual(0f, progress.Progress, Delta);

            progress.Report(0.45f);
            Assert.AreEqual(0.45f, progress.Progress, Delta);

            bool invoked = false;
            progress.OnFinished += () => invoked = true;
            Assert.IsFalse(invoked);

            progress.Report(1f);
            Assert.AreEqual(1f, progress.Progress, Delta);
            Assert.IsFalse(invoked);

            progress.InvokeFinished();
            Assert.AreEqual(1f, progress.Progress, Delta);
            Assert.IsTrue(invoked);
        }
    }
}