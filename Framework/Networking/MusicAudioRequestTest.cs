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
            var request = new MusicAudioRequest(TestConstants.RemoteMp3Url, false);
            request.StartTask();

            while (!request.IsFinished)
            {
                Debug.Log($"Progress: {request.Progress}");
                yield return null;
            }

            Assert.IsNotNull(request.Output);

            var unityAudio = request.Output as UnityAudio;
            Assert.IsNotNull(unityAudio);

            Debug.Log($"Dur: {unityAudio.Duration}, Channels: {unityAudio.Channels}");
        }
    }
}