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
    public class TextureRequestTest {
        
        [UnityTest]
        public IEnumerator TestReadable()
        {
            var request = new TextureRequest(TestConstants.RemoteImageUrl, false);
            request.Request();

            while (!request.IsFinished)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsTrue(request.Response.IsSuccess);

            // Save to file
            var path = Path.Combine(TestConstants.TestAssetPath, "TextureRequestResult.jpg");
            File.WriteAllBytes(path, request.Response.ByteData);

            Assert.IsTrue(request.Response.TextureData.isReadable);

            Debug.Log("Saved");
        }

        [UnityTest]
        public IEnumerator TestNonReadable()
        {
            var request = new TextureRequest(TestConstants.RemoteImageUrl);
            request.Request();

            while (!request.IsFinished)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsTrue(request.Response.IsSuccess);

            // Save to file
            var path = Path.Combine(TestConstants.TestAssetPath, "TextureRequestResult.jpg");
            File.WriteAllBytes(path, request.Response.ByteData);

            Assert.IsFalse(request.Response.TextureData.isReadable);

            Debug.Log("Saved");
        }

        [UnityTest]
        public IEnumerator TestTask()
        {
            var request = new TextureRequest(TestConstants.RemoteImageUrl, false);
            ITask<Texture2D> promise = request;
            Assert.AreEqual(request, promise);

            // Receive via callback
            Texture2D texture = null;
            var listener = new TaskListener<Texture2D>();
            listener.OnFinished += (value) => texture = value;

            // Request
            promise.StartTask(listener);
            Assert.IsFalse(promise.IsFinished);
            Assert.IsFalse(request.IsFinished);

            // Wait till finish
            while (!promise.IsFinished)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsTrue(promise.IsFinished);
            Assert.IsTrue(request.IsFinished);
            Assert.IsNotNull(request.Response);
            Assert.IsNotNull(texture);
            Assert.AreEqual(listener.Value, request.Response.TextureData);
            Assert.AreEqual(listener.Value, texture);
        }
    }
}