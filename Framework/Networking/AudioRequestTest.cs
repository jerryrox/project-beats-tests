using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;
using PBFramework.Threading.Futures;

namespace PBFramework.Networking.Tests
{
    public class AudioRequestTest {
        
        [UnityTest]
        public IEnumerator TestNonStream()
        {
            var request = new AudioRequest(TestConstants.RemoteMp3Url, false);
            var progress = new ReturnableProgress<IWebRequest>();
            request.Request(progress);

            while (!request.IsCompleted.Value)
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

            var request = new AudioRequest(TestConstants.RemoteMp3Url, true);
            var progress = new ReturnableProgress<IWebRequest>();
            request.Request(progress);

            while (!request.IsCompleted.Value)
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
            var request = new AudioRequest(TestConstants.RemoteMp3Url, false);
            IControlledFuture<AudioClip> promise = request;
            Assert.AreEqual(request, promise);
            Assert.IsNull(promise.Output.Value);

            // Receive via callback
            AudioClip clip = null;
            promise.Output.OnNewValue += (c) => clip = c;

            // Request
            promise.Start();
            Assert.IsFalse(promise.IsCompleted.Value);
            Assert.IsFalse(request.IsCompleted.Value);

            // Wait till finish
            while (!promise.IsCompleted.Value)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsTrue(promise.IsCompleted.Value);
            Assert.IsTrue(request.IsCompleted.Value);
            Assert.IsNotNull(request.Response);
            Assert.IsNotNull(promise.Output.Value);
            Assert.IsNotNull(clip);
            Assert.AreEqual(promise.Output.Value, request.Response.AudioData);
            Assert.AreEqual(promise.Output.Value, clip);
        }
    }
}