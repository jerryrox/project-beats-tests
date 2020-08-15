using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading.Futures;

namespace PBFramework.Networking.Tests
{
    public class TextureRequestTest {
        
        [UnityTest]
        public IEnumerator TestReadable()
        {
            var request = new TextureRequest(TestConstants.RemoteImageUrl, false);
            request.Request();

            while (!request.IsCompleted.Value)
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

            while (!request.IsCompleted.Value)
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
        public IEnumerator TestPromise()
        {
            var request = new TextureRequest(TestConstants.RemoteImageUrl, false);
            IControlledFuture<Texture2D> promise = request;
            Assert.AreEqual(request, promise);
            Assert.IsNull(promise.Output.Value);

            // Receive via callback
            Texture2D texture = null;
            promise.Output.OnNewValue += (c) => texture = c;

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
            Assert.IsNotNull(texture);
            Assert.AreEqual(promise.Output.Value, request.Response.TextureData);
            Assert.AreEqual(promise.Output.Value, texture);
        }
    }
}