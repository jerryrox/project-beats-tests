using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;

namespace PBFramework.Assets.Caching.Tests
{
    public class AudioCacherTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var cacher = new AudioCacher(true);
            var progress = new ReturnableProgress<AudioClip>();

            var key = "http://23.237.126.42/ost/touhou-youyoumu-perfect-cherry-blossom/vrdyenmp/%5B01%5D%20Youyoumu%20~%20Snow%20or%20Cherry%20Petal.mp3";
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