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
            var progress = new ReturnableProgress<Texture2D>();

            var key = "https://cdn-www.bluestacks.com/bs-images/Banner_com.sunborn.girlsfrontline.en-1.jpg";
            var id = cacher.Request(key, progress);
            while (!cacher.IsCached(key))
            {
                Debug.Log("Progress: " + progress.Progress);
                yield return null;
            }

            Assert.IsTrue(cacher.IsCached(key));
            Assert.IsNotNull(progress.Value);
        }
    }
}