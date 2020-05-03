using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;

namespace PBFramework.Networking.Tests
{
    public class AudioRequestTest {
        
        [UnityTest]
        public IEnumerator TestNonStream()
        {
            var request = new AudioRequest("http://23.237.126.42/ost/touhou-youyoumu-perfect-cherry-blossom/vrdyenmp/%5B01%5D%20Youyoumu%20~%20Snow%20or%20Cherry%20Petal.mp3", false);
            var progress = new ReturnableProgress<IWebRequest>();
            request.Request(progress);

            while (!request.IsDone)
            {
                Debug.Log("Progress: " + progress.Progress);
                yield return null;
            }

            Assert.IsNotNull(request.Response);

            var clip = request.Response.AudioData;
            Assert.IsNotNull(clip);

            Debug.Log($"Content: {request.Response.ContentLength}, response: {request.Response.BytesLoaded}");
            Assert.AreEqual(request.Response.ContentLength, request.Response.BytesLoaded);
        }

        [UnityTest]
        public IEnumerator TestStream()
        {
            // Current limitation:
            // There is no way to check whether the loaded audio is truly a streaming audio or not.

            var request = new AudioRequest("http://23.237.126.42/ost/touhou-youyoumu-perfect-cherry-blossom/vrdyenmp/%5B01%5D%20Youyoumu%20~%20Snow%20or%20Cherry%20Petal.mp3", true);
            var progress = new ReturnableProgress<IWebRequest>();
            request.Request(progress);

            while (!request.IsDone)
            {
                Debug.Log("Progress: " + progress.Progress);
                yield return null;
            }

            Assert.IsNotNull(request.Response);

            var clip = request.Response.AudioData;
            Assert.IsNotNull(clip);

            Debug.Log($"Content: {request.Response.ContentLength}, response: {request.Response.BytesLoaded}");
            Assert.LessOrEqual((double)request.Response.BytesLoaded, (double)request.Response.ContentLength);
        }

        [UnityTest]
        public IEnumerator TestPromise()
        {
            var request = new AudioRequest("http://23.237.126.42/ost/touhou-youyoumu-perfect-cherry-blossom/vrdyenmp/%5B01%5D%20Youyoumu%20~%20Snow%20or%20Cherry%20Petal.mp3", false);
            IExplicitPromise<AudioClip> promise = request;
            Assert.AreEqual(request, promise);
            Assert.IsNull(promise.Result);

            // Receive via callback
            AudioClip clip = null;
            promise.OnFinishedResult += (c) => clip = c;

            // Request
            promise.Start();
            Assert.IsFalse(promise.IsFinished);
            Assert.IsFalse(request.IsDone);

            // Wait till finish
            while (!promise.IsFinished)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsTrue(promise.IsFinished);
            Assert.IsTrue(request.IsDone);
            Assert.IsNotNull(request.Response);
            Assert.IsNotNull(promise.Result);
            Assert.IsNotNull(clip);
            Assert.AreEqual(promise.Result, request.Response.AudioData);
            Assert.AreEqual(promise.Result, clip);
        }
    }
}