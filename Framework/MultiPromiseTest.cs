using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Tests
{
    public class MultiPromiseTest {

        private const float Delta = 0.00001f;

        
        [Test]
        public void Test()
        {
            var promises = new TestPromise[]
            {
                new TestPromise(),
                new TestPromise(),
                new TestPromise(),
            };
            float progress = 0f;
            bool finished = false;

            var multiPromise = new MultiPromise(promises);
            multiPromise.OnProgress += (p) => progress = p;
            multiPromise.OnFinished += () => finished = true;
            promises.ForEach(p =>
            {
                Assert.IsFalse(p.IsStarted);
                Assert.IsFalse(p.IsRevoked);
            });
            Assert.AreEqual(0f, multiPromise.Progress, Delta);
            Assert.IsFalse(finished);

            multiPromise.Start();
            promises.ForEach(p =>
            {
                Assert.IsTrue(p.IsStarted);
                Assert.IsFalse(p.IsRevoked);
            });

            multiPromise.Revoke();
            promises.ForEach(p =>
            {
                Assert.IsTrue(p.IsStarted);
                Assert.IsTrue(p.IsRevoked);
            });

            promises[0].SetProgress(1f);
            promises[0].Resolve(null);
            Assert.AreEqual(0.3333333333f, multiPromise.Progress, Delta);
            Assert.AreEqual(0.3333333333f, progress, Delta);
            Assert.IsFalse(finished);
            Assert.IsFalse(multiPromise.IsFinished);

            promises[1].SetProgress(1f);
            promises[1].Resolve(null);
            Assert.AreEqual(0.6666666666f, multiPromise.Progress, Delta);
            Assert.AreEqual(0.6666666666f, progress, Delta);
            Assert.IsFalse(finished);
            Assert.IsFalse(multiPromise.IsFinished);

            promises[2].SetProgress(1f);
            promises[2].Resolve(null);
            Assert.AreEqual(1f, multiPromise.Progress, Delta);
            Assert.AreEqual(1f, progress, Delta);
            Assert.IsTrue(finished);
            Assert.IsTrue(multiPromise.IsFinished);
        }

        private class TestPromise : ProxyPromise
        {
            public bool IsStarted { get; set; } = false;

            public bool IsRevoked { get; set; } = false;


            public TestPromise() : base()
            {
                startAction = () => IsStarted = true;
                revokeAction = () => IsRevoked = true;
            }
        }
    }
}