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
            var request = new WebRequest("file://" + Path.Combine(Application.streamingAssetsPath, "TestText.txt"));
            var progress = new ReturnableProgress<IWebRequest>();
            request.Request(progress);

            // Max threshold
            int time = 2;
            while (time > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                time--;
            }

            Assert.AreEqual(1f, progress.Progress);
            Assert.AreEqual(1f, request.Progress);
            Assert.IsTrue(request.IsDone);
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
            var request = new WebRequest("http://23.237.126.42/ost/touhou-youyoumu-perfect-cherry-blossom/vrdyenmp/%5B01%5D%20Youyoumu%20~%20Snow%20or%20Cherry%20Petal.mp3");
            var progress = new ReturnableProgress<IWebRequest>();
            request.Request(progress);

            Debug.Log("Requesting to: " + request.Url);

            // Wait until half the progress
            while (progress.Progress < 0.5)
            {
                Debug.Log("First progress: " + progress.Progress);
                yield return null;
            }

            // Retry now
            request.Retry();

            // Try wait for a frame.
            yield return null;

            // Check progress
            Debug.Log("Retried new progress: " + progress.Progress);
            while (!request.IsDone)
            {
                Debug.Log("Second progress: " + progress.Progress);
                yield return null;
            }

            Assert.AreEqual(1f, progress.Progress, 0.00000001f);
            Assert.IsNotNull(request.Response);

            Debug.Log("Content: " + request.Response.TextData);
            Debug.Log("Content length: " + request.Response.ContentLength);
            Debug.Log("Content type: " + request.Response.ContentType);

            // Save to streaming assets just to check.
            string savePath = Path.Combine(Application.streamingAssetsPath, "WebRequestTestDownload.mp3");
            File.WriteAllBytes(savePath, request.Response.ByteData);

            Assert.IsTrue(File.Exists(savePath));
        }

        [UnityTest]
        public IEnumerator TestTimeout()
        {
            var request = new WebRequest("http://23.237.126.42/ost/touhou-youyoumu-perfect-cherry-blossom/vrdyenmp/%5B01%5D%20Youyoumu%20~%20Snow%20or%20Cherry%20Petal.mp3", timeout: 2);
            var progress = new ReturnableProgress<IWebRequest>();
            request.Request(progress);

            while(!request.IsDone)
                yield return null;

            Debug.Log("Stopped at progress: " + progress.Progress);
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
        public IEnumerator TestPromiseStart()
        {
            var request = new WebRequest("file://" + Path.Combine(Application.streamingAssetsPath, "TestText.txt"));
            IPromise promise = request;
            Assert.IsNotNull(promise);

            // Register callback on finish.
            bool onFinishedCalled = false;
            promise.OnFinished += () =>
            {
                onFinishedCalled = true;
            };
            Assert.IsFalse(onFinishedCalled);

            // Start the request.
            promise.Start();

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
            Assert.IsTrue(request.IsDone);
            Assert.IsTrue(onFinishedCalled);

            var response = request.Response;
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("My random text for testing.", response.TextData);
            Assert.AreEqual(request, promise.Result);
        }

        [UnityTest]
        public IEnumerator TestPromiseRevoke()
        {
            var request = new WebRequest("http://23.237.126.42/ost/touhou-youyoumu-perfect-cherry-blossom/vrdyenmp/%5B01%5D%20Youyoumu%20~%20Snow%20or%20Cherry%20Petal.mp3");
            IPromise promise = request;
            Assert.IsNotNull(promise);

            // Register callback
            bool onFinishedCalled = false;
            promise.OnFinished += () =>
            {
                onFinishedCalled = true;
            };
            Assert.IsFalse(onFinishedCalled);

            // Start request
            promise.Start();

            // Wait till certain progress level
            while (request.Progress < 0.4f)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsFalse(request.IsDone);
            Assert.IsFalse(promise.IsFinished);

            // Revoke the request.
            promise.Revoke();
            Assert.IsFalse(request.IsDone);
            Assert.IsFalse(promise.IsFinished);

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

            Assert.IsNotNull(request.Response);
            Assert.IsFalse(request.IsDone);
            Assert.IsFalse(promise.IsFinished);
            Assert.AreEqual(request, promise.Result);
            Assert.IsFalse(onFinishedCalled);
        }
    }
}