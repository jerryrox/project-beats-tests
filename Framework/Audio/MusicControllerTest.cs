using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Networking;

namespace PBFramework.Audio.Tests
{
    public class MusicControllerTest {

        private const float Delta = 100;


        [UnityTest]
        public IEnumerator TestSetVolume()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = MusicController.Create();
            controller.MountAudio(audio);
            controller.Play();
            yield return new WaitForSeconds(1f);
            controller.SetVolume(0.25f);
            yield return new WaitForSeconds(1f);
        }

        [UnityTest]
        public IEnumerator TestFade()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = MusicController.Create();
            controller.MountAudio(audio);

            int fadeEndCount = 0;
            controller.OnFaded += (f) => fadeEndCount++;

            controller.Play();
            yield return new WaitForSeconds(1f);
            controller.Fade(1f, 0f);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(1, fadeEndCount);
            controller.Fade(1f);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(2, fadeEndCount);
            controller.SetFade(0.5f);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(3, fadeEndCount);
        }

        [UnityTest]
        public IEnumerator TestTempo()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = MusicController.Create();
            controller.MountAudio(audio);
            controller.SetTempo(1.5f);
            controller.Play();
            yield return new WaitForSeconds(1f);
            Assert.Greater(controller.CurrentTime, 1.5f);
        }

        [UnityTest]
        public IEnumerator TestPlay()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = MusicController.Create();
            var clock = controller.Clock;
            controller.MountAudio(audio);
            Assert.AreEqual(0f, clock.CurrentTime, Delta);

            Debug.Log("A");
            controller.Play(500);
            Assert.AreEqual(-500f, clock.CurrentTime, Delta);

            Debug.Log("B");
            controller.Pause();
            Assert.AreEqual(-500, clock.CurrentTime, Delta);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(-500, clock.CurrentTime, Delta);

            Debug.Log("C");
            controller.Play();
            Assert.AreEqual(-500, clock.CurrentTime, Delta);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(500, clock.CurrentTime, Delta);

            Debug.Log("D");
            controller.Stop();
            Assert.AreEqual(0, clock.CurrentTime, Delta);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(0, clock.CurrentTime, Delta);

            Debug.Log("E");
            controller.Seek(1000);
            Assert.AreEqual(950, clock.CurrentTime, Delta);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(950, clock.CurrentTime, Delta);

            Debug.Log("F");
            controller.Play(0);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(1000, clock.CurrentTime, Delta);

            Debug.Log("G");
            controller.Seek(5000);
            Assert.AreEqual(4950, clock.CurrentTime, Delta);
            yield return new WaitForSeconds(2f);
        }

        private IEnumerator LoadAudio(Action<IAudio> onFinished)
        {
            new GameObject("Listener").AddComponent<AudioListener>();

            Assert.IsNotNull(onFinished);
            var request = new AudioRequest("file://" + Path.Combine(Application.streamingAssetsPath, "Audio/music.mp3"));
            request.OnFinishedResult += (audio) => onFinished(new UnityAudio(audio));
            request.Start();
            while(!request.IsFinished)
                yield return null;
            Assert.IsNotNull(request.RawResult);
            AudioClip clip = request.RawResult as AudioClip;
            Assert.IsNotNull(clip);
            Assert.Greater(clip.length, 0f);
        }
    }
}