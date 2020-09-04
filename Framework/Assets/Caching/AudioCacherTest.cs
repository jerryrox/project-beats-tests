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

            var key = TestConstants.RemoteMp3Url;
            var listener = cacher.Request(key);
            while (!cacher.IsCached(key))
            {
                Debug.Log("Progress: " + listener.Progress.Value);
                yield return null;
            }

            Assert.IsTrue(cacher.IsCached(key));
            Assert.IsNotNull(listener.Output.Value);
        }
    }
}