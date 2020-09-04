using System.Collections;
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

            var key = TestConstants.RemoteMp3Url;
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