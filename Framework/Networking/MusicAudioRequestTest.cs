using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Audio;
using PBFramework.Threading;

namespace PBFramework.Networking.Tests
{
    public class MusicAudioRequestTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var request = new MusicAudioRequest("http://23.237.126.42/ost/touhou-youyoumu-perfect-cherry-blossom/vrdyenmp/%5B01%5D%20Youyoumu%20~%20Snow%20or%20Cherry%20Petal.mp3", false);
            request.Start();

            while (!request.IsFinished)
            {
                Debug.Log($"Progress: {request.Progress}");
                yield return null;
            }

            Assert.IsNotNull(request.Result);

            var unityAudio = request.Result as UnityAudio;
            Assert.IsNotNull(unityAudio);

            Debug.Log($"Dur: {unityAudio.Duration}, Channels: {unityAudio.Channels}");
        }
    }
}