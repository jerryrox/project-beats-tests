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
            var progress = new SimpleProgress();
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
            var progress = new SimpleProgress();
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
            var progress = new SimpleProgress();
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
    }
}