using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Networking.Tests
{
    public class TextureRequestTest {
        
        [UnityTest]
        public IEnumerator TestReadable()
        {
            var request = new TextureRequest("https://cdn-www.bluestacks.com/bs-images/Banner_com.sunborn.girlsfrontline.en-1.jpg", false);
            request.Request();

            while (!request.IsDone)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsTrue(request.Response.IsSuccess);

            // Save to file
            var path = Path.Combine(Application.streamingAssetsPath, "TextureRequestResult.jpg");
            File.WriteAllBytes(path, request.Response.ByteData);

            Assert.IsTrue(request.Response.TextureData.isReadable);

            Debug.Log("Saved");
        }

        [UnityTest]
        public IEnumerator TestNonReadable()
        {
            var request = new TextureRequest("https://cdn-www.bluestacks.com/bs-images/Banner_com.sunborn.girlsfrontline.en-1.jpg");
            request.Request();

            while (!request.IsDone)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsTrue(request.Response.IsSuccess);

            // Save to file
            var path = Path.Combine(Application.streamingAssetsPath, "TextureRequestResult.jpg");
            File.WriteAllBytes(path, request.Response.ByteData);

            Assert.IsFalse(request.Response.TextureData.isReadable);

            Debug.Log("Saved");
        }
    }
}