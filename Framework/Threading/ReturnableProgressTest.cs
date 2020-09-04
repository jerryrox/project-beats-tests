using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Tests
{
    public class ReturnableProgressTest {

        private const float Delta = 0.0000001f;


        [Test]
        public void TestSimpleProgress()
        {
            var progress = new ReturnableProgress<bool>();
            Assert.AreEqual(0f, progress.Progress, Delta);

            progress.Report(0.75f);
            Assert.AreEqual(0.75f, progress.Progress, Delta);
        }

        [Test]
        public void TestEvents()
        {
            var progress = new ReturnableProgress<bool>();
            bool flag = false;
            int i = 0;
            progress.OnFinished += (f) => flag = f;
            ((IEventProgress)progress).OnFinished += () => i++;
            Assert.IsFalse(flag);
            Assert.AreEqual(0, i);

            progress.Report(0.5f);
            Assert.IsFalse(flag);
            Assert.AreEqual(0, i);

            progress.InvokeFinished(true);
            Assert.IsTrue(flag);
            Assert.AreEqual(1, i);

            progress.InvokeFinished();
            Assert.IsFalse(flag);
            Assert.AreEqual(2, i);

            progress.InvokeFinished((object)true);
            Assert.IsTrue(flag);
            Assert.AreEqual(3, i);

            try
            {
                progress.InvokeFinished((object)"asdf");
                Assert.Fail("There should've been a cast exception");
            }
            catch (InvalidCastException) {}
            Assert.IsTrue(flag);
            Assert.AreEqual(3, i);

            ((IEventProgress)progress).InvokeFinished();
            Assert.IsFalse(flag);
            Assert.AreEqual(4, i);
        }

        [Test]
        public void TestValue()
        {
            var progress = new ReturnableProgress<bool>();
            progress.Value = true;
            Assert.IsTrue(progress.Value);
            Assert.IsTrue((bool)((IReturnableProgress)progress).Value);

            try
            {
                ((IReturnableProgress)progress).Value = 12345;
                Assert.Fail("There should've been a cast exception");
            }
            catch (InvalidCastException) { }
            Assert.IsTrue(progress.Value);
            Assert.IsTrue((bool)((IReturnableProgress)progress).Value);
        }
    }
}