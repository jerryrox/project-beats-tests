using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Tests
{
    public class ProxyPromiseTest {

        private const float Delta = 0.00001f;

        
        [Test]
        public void TestNonGeneric()
        {
            bool started = false;
            bool revoked = false;
            bool finished = false;
            float progress = 0f;
            Action<ProxyPromise> startAction = (p) => started = true;
            Action revokeAction = () => revoked = true;
            Action<float> progressAction = (p) => progress = p;
            Action resolveAction = () => finished = true;

            var promise = new ProxyPromise(startAction, revokeAction);
            promise.OnFinished += resolveAction;
            promise.OnProgress += progressAction;

            Assert.AreEqual(0f, promise.Progress, Delta);
            Assert.AreEqual(0f, progress, Delta);
            Assert.IsFalse(started);
            Assert.IsFalse(revoked);
            Assert.IsFalse(finished);
            Assert.IsFalse(promise.IsFinished);

            promise.Start();
            Assert.IsTrue(started);
            Assert.IsFalse(revoked);
            Assert.IsFalse(finished);
            Assert.IsFalse(promise.IsFinished);

            promise.Revoke();
            Assert.IsTrue(started);
            Assert.IsTrue(revoked);
            Assert.IsFalse(finished);
            Assert.IsFalse(promise.IsFinished);

            promise.SetProgress(0.5f);
            Assert.AreEqual(0.5f, promise.Progress, Delta);
            Assert.AreEqual(0.5f, progress, Delta);

            promise.Resolve(1);
            Assert.IsTrue(finished);
            Assert.IsTrue(promise.IsFinished);
            Assert.AreEqual(1, promise.RawResult);
        }

        [Test]
        public void TestGeneric()
        {
            bool started = false;
            bool revoked = false;
            bool finished = false;
            float progress = 0f;
            int result = 0;
            Action<ProxyPromise> startAction = (p) => started = true;
            Action revokeAction = () => revoked = true;
            Action<float> progressAction = (p) => progress = p;
            Action resolveAction = () => finished = true;
            Action<int> resultAction = (r) => result = r;

            var promise = new ProxyPromise<int>(startAction, revokeAction);
            promise.OnFinished += resolveAction;
            promise.OnProgress += progressAction;
            promise.OnFinishedResult += resultAction;

            Assert.AreEqual(0, result);
            Assert.AreEqual(0f, promise.Progress, Delta);
            Assert.AreEqual(0f, progress, Delta);
            Assert.IsFalse(started);
            Assert.IsFalse(revoked);
            Assert.IsFalse(finished);
            Assert.IsFalse(promise.IsFinished);

            promise.Start();
            Assert.IsTrue(started);
            Assert.IsFalse(revoked);
            Assert.IsFalse(finished);
            Assert.IsFalse(promise.IsFinished);

            promise.Revoke();
            Assert.IsTrue(started);
            Assert.IsTrue(revoked);
            Assert.IsFalse(finished);
            Assert.IsFalse(promise.IsFinished);

            promise.SetProgress(0.5f);
            Assert.AreEqual(0.5f, promise.Progress, Delta);
            Assert.AreEqual(0.5f, progress, Delta);

            promise.Resolve(1);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1, promise.Result);
            Assert.AreEqual(1, (promise as ProxyPromise).RawResult);
            Assert.IsTrue(finished);
            Assert.IsTrue(promise.IsFinished);

            promise.Resolve((object)3);
            Assert.AreEqual(3, result);
            Assert.AreEqual(3, promise.Result);
            Assert.AreEqual(3, (promise as ProxyPromise).RawResult);
            Assert.IsTrue(finished);
            Assert.IsTrue(promise.IsFinished);

            try
            {
                promise.Resolve(true);
                Assert.Fail();
            }
            catch (Exception e)
            {
            }
        }
    }
}