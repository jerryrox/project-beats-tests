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
    public class UnityAudioControllerTest {

        private const float Delta = 100;


        [UnityTest]
        public IEnumerator TestMount()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = CreateController();

            Assert.IsNull(controller.Audio);
            controller.MountAudio(audio);
            Assert.IsNotNull(controller.Audio);
            Assert.AreEqual(audio, controller.Audio);
        }

        [UnityTest]
        public IEnumerator TestPlay()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = CreateController();
            controller.MountAudio(audio);
            controller.Play();
            Assert.IsTrue(controller.IsPlaying);
            Assert.IsFalse(controller.IsPaused);
            Assert.IsFalse(controller.IsStopped);
            while (controller.IsPlaying)
            {
                // Debug.Log("Time: " + controller.CurrentTime);
                yield return null;
            }
            yield return new WaitForSeconds(audio.Duration / 1000f);
            Assert.AreEqual(0f, controller.CurrentTime, Delta);
            Assert.IsFalse(controller.IsPlaying);
            Assert.IsFalse(controller.IsPaused);
            Assert.IsTrue(controller.IsStopped);
        }

        [UnityTest]
        public IEnumerator TestPlayDelay()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = CreateController();
            controller.MountAudio(audio);
            controller.Play(2000);
            Assert.IsTrue(controller.IsPlaying);
            Assert.IsFalse(controller.IsPaused);
            Assert.IsFalse(controller.IsStopped);
            yield return new WaitForSeconds(audio.Duration / 1000f);
            Assert.Less(controller.CurrentTime, audio.Duration);
            Assert.IsTrue(controller.IsPlaying);
            Assert.IsFalse(controller.IsPaused);
            Assert.IsFalse(controller.IsStopped);
            yield return new WaitForSeconds(2f);
            Assert.AreEqual(0f, controller.CurrentTime, Delta);
            Assert.IsFalse(controller.IsPlaying);
            Assert.IsFalse(controller.IsPaused);
            Assert.IsTrue(controller.IsStopped);
        }

        [UnityTest]
        public IEnumerator TestPause()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = CreateController();
            controller.MountAudio(audio);
            controller.Play();
            Assert.IsTrue(controller.IsPlaying);
            Assert.IsFalse(controller.IsPaused);
            Assert.IsFalse(controller.IsStopped);
            yield return new WaitForSeconds(audio.Duration / 1000f / 2f);
            controller.Pause();
            Assert.AreEqual(audio.Duration / 2f, controller.CurrentTime, Delta);
            Assert.IsFalse(controller.IsPlaying);
            Assert.IsTrue(controller.IsPaused);
            Assert.IsFalse(controller.IsStopped);
        }

        [UnityTest]
        public IEnumerator TestStop()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = CreateController();
            controller.MountAudio(audio);
            controller.Play();
            Assert.IsTrue(controller.IsPlaying);
            Assert.IsFalse(controller.IsPaused);
            Assert.IsFalse(controller.IsStopped);
            yield return new WaitForSeconds(audio.Duration / 1000f / 2f);
            controller.Stop();
            Assert.AreEqual(0f, controller.CurrentTime, Delta);
            Assert.IsFalse(controller.IsPlaying);
            Assert.IsFalse(controller.IsPaused);
            Assert.IsTrue(controller.IsStopped);
        }

        [UnityTest]
        public IEnumerator TestSeek()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = CreateController();
            controller.MountAudio(audio);
            controller.Play();
            Assert.AreEqual(0f, controller.CurrentTime, Delta);
            controller.Seek(500);
            Assert.AreEqual(500f, controller.CurrentTime, Delta);
        }

        [UnityTest]
        public IEnumerator TestSetVolume()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = CreateController();
            controller.MountAudio(audio);
            controller.SetVolume(0.25f);
            controller.Play();
            yield return new WaitForSeconds(audio.Duration / 1000f);
        }

        [UnityTest]
        public IEnumerator TestDispose()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = CreateController();
            controller.MountAudio(audio);
            controller.Dispose();
            try
            {
                controller.Play();
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [UnityTest]
        public IEnumerator TestLoop()
        {
            IAudio audio = null;
            yield return LoadAudio(a => audio = a);

            var controller = CreateController();

            int loopCount = 0;
            controller.OnLoop += () => loopCount++;
            
            controller.MountAudio(audio);
            controller.LoopTime = 0f;
            controller.IsLoop = true;

            controller.Play();
            yield return new WaitForSeconds(audio.Duration / 1000f * 2.5f);
            controller.Stop();
            Assert.AreEqual(2, loopCount);

            controller.LoopTime = audio.Duration / 2f;
            loopCount = 0;
            controller.Seek(audio.Duration / 2f);
            controller.Play();
            yield return new WaitForSeconds(audio.Duration / 1000f / 2f * 2.5f);
            controller.Stop();
            Assert.AreEqual(2, loopCount);
        }

        private DummyController CreateController()
        {
            return new GameObject("controller").AddComponent<DummyController>();
        }

        private IEnumerator LoadAudio(Action<IAudio> onFinished)
        {
            new GameObject("Listener").AddComponent<AudioListener>();

            Assert.IsNotNull(onFinished);
            var request = new AudioRequest("file://" + Path.Combine(Application.streamingAssetsPath, "Audio/effect.mp3"));
            request.OnFinishedResult += (audio) => onFinished(new UnityAudio(audio));
            request.Start();
            while(!request.IsFinished)
                yield return null;
            Assert.IsNotNull(request.Result);
            AudioClip clip = request.Result as AudioClip;
            Assert.IsNotNull(clip);
            Assert.Greater(clip.length, 0f);
        }

        private class DummyController : UnityAudioController
        {
            private bool isPlaying;
            private bool isPaused;

            public override bool IsPlaying => isPlaying;

            public override bool IsPaused => isPaused;

            public override bool IsStopped => !isPlaying && !isPaused;


            public override void Play(float delay)
            {
                base.Play(delay);
                if (CanPlay())
                {
                    isPlaying = true;
                    isPaused = false;
                }
            }

            public override void Pause()
            {
                base.Pause();
                if (CanPause())
                {
                    isPlaying = false;
                    isPaused = true;
                }
            }

            public override void Stop()
            {
                base.Stop();
                if (CanStop())
                {
                    isPlaying = false;
                    isPaused = false;
                }
            }
        }
    }
}