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
            var listener = new TaskListener<AudioClip>();

            var key = TestConstants.RemoteMp3Url;
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