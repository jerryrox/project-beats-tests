using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;

namespace PBFramework.Assets.Caching.Tests
{
    public class TextureCacherTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var cacher = new TextureCacher(true);
            var listener = new TaskListener<Texture2D>();

            var key = TestConstants.RemoteImageUrl;
            var id = cacher.Request(key, listener);
            while (!cacher.IsCached(key))
            {
                Debug.Log("Progress: " + listener.Progress);
                yield return null;
            }

            Assert.IsTrue(cacher.IsCached(key));
            Assert.IsNotNull(listener.Value);
        }
    }
}