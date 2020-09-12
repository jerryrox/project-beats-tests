using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;

namespace PBFramework.Networking.Tests
{
    public class WebRequestTest {

        [UnityTest]
        public IEnumerator TestRequest()
        {
            var request = new WebRequest("file://" + Path.Combine(TestConstants.TestAssetPath, "TestText.txt"));
            var listener = new TaskListener<IWebRequest>();
            request.Request(listener);

            // Max threshold
            int time = 2;
            while (time > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                time--;
            }

            Assert.AreEqual(1f, listener.Progress);
            Assert.AreEqual(1f, request.Progress);
            Assert.IsTrue(request.IsFinished);
            Assert.IsTrue(request.IsAlive);

            var response = request.Response;
            Assert.IsNotNull(response);

            if (response.IsSuccess)
            {
                Assert.AreEqual("My random text for testing.", response.TextData);
                Debug.Log("Success: " + response.TextData);
            }
            else
            {
                Assert.IsNotNull(response.ErrorMessage);
                Debug.Log("Fail: " + response.ErrorMessage);
            }
        }

        [UnityTest]
        public IEnumerator TestRetry()
        {
            var request = new WebRequest(TestConstants.RemoteMp3Url);
            var listener = new TaskListener<IWebRequest>();
            request.Request(listener);

            Debug.Log("Requesting to: " + request.Url);

            // Wait until half the progress
            while (listener.Progress < 0.5)
            {
                Debug.Log("First progress: " + listener.Progress);
                yield return null;
            }

            // Retry now
            request.Retry();

            // Try wait for a frame.
            yield return null;

            // Check progress
            Debug.Log("Retried new progress: " + listener.Progress);
            while (!request.IsFinished)
            {
                Debug.Log("Second progress: " + listener.Progress);
                yield return null;
            }

            Assert.AreEqual(1f, listener.Progress, 0.00000001f);
            Assert.IsNotNull(request.Response);

            Debug.Log("Content: " + request.Response.TextData);
            Debug.Log("Content length: " + request.Response.ContentLength);
            Debug.Log("Content type: " + request.Response.ContentType);

            // Save to streaming assets just to check.
            string savePath = Path.Combine(TestConstants.TestAssetPath, "WebRequestTestDownload.mp3");
            File.WriteAllBytes(savePath, request.Response.ByteData);

            Assert.IsTrue(File.Exists(savePath));
        }

        [UnityTest]
        public IEnumerator TestTimeout()
        {
            var request = new WebRequest(TestConstants.RemoteMp3Url, timeout: 2);
            var listener = new TaskListener<IWebRequest>();
            request.Request(listener);

            while(!request.IsFinished)
                yield return null;

            Debug.Log("Stopped at progress: " + listener.Progress);
            Debug.Log("Error message: " + request.Response.ErrorMessage);

            Assert.IsNotNull(request.Response);

            if (request.Response.IsSuccess)
            {
                Assert.IsNull(request.Response.ErrorMessage);
                Debug.Log("Result was true.");
            }
            else
            {
                Assert.IsNotNull(request.Response.ErrorMessage);
                Debug.Log("Result was false");
            }
        }

        [UnityTest]
        public IEnumerator TestTaskStart()
        {
            var request = new WebRequest("file://" + Path.Combine(TestConstants.TestAssetPath, "TestText.txt"));
            ITask promise = request;
            Assert.IsNotNull(promise);

            // Register callback on finish.
            bool onFinishedCalled = false;
            var listener = new TaskListener();
            listener.OnFinished += () => onFinishedCalled = true;
            Assert.IsFalse(onFinishedCalled);

            // Start the request.
            promise.StartTask(listener);

            // Setup time limit.
            float limit = 4;
            while (!promise.IsFinished)
            {
                limit -= Time.deltaTime;
                if(limit <= 0)
                    Assert.Fail("Request took too long! perhaps it's not running at all?");
                yield return null;
            }

            Assert.IsTrue(promise.IsFinished);
            Assert.IsTrue(request.IsFinished);
            Assert.IsTrue(onFinishedCalled);

            var response = request.Response;
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("My random text for testing.", response.TextData);
        }

        [UnityTest]
        public IEnumerator TestTaskRevoke()
        {
            var request = new WebRequest(TestConstants.RemoteMp3Url);
            ITask task = request;
            Assert.IsNotNull(task);

            // Register callback
            bool onFinishedCalled = false;
            var listener = new TaskListener();
            listener.OnFinished += () => onFinishedCalled = true;
            Assert.IsFalse(onFinishedCalled);

            // Start request
            task.StartTask(listener);

            // Wait till certain progress level
            while (request.Progress < 0.4f)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsFalse(request.IsFinished);
            Assert.IsFalse(task.IsFinished);

            // Revoke the request.
            task.RevokeTask(true);
            Assert.IsFalse(request.IsFinished);
            Assert.IsFalse(task.IsFinished);

            // See if any progress is changing even after revoke.
            float lastProgress = request.Progress;
            float limit = 3;
            while (true)
            {
                limit -= Time.deltaTime;
                if (request.Progress != lastProgress)
                {
                    Debug.Log("New progress detected! " + request.Progress);
                    Assert.Fail("There mustn't be any progressing going on!");
                }
                if(limit <= 0)
                    break;
                yield return null;
            }

            Assert.IsNull(request.Response);
            Assert.IsFalse(request.IsFinished);
            Assert.IsFalse(task.IsFinished);
            Assert.IsFalse(onFinishedCalled);
        }
    }
}