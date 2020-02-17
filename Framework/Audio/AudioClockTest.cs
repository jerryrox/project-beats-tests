using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Audio.Tests
{
    public class AudioClockTest {

        private const float Delta = 100;


        [Test]
        public void TestEvents()
        {
            DummyController controller = new DummyController();
            var clock = controller.Clock;
            Assert.IsNotNull(clock);

            Assert.IsFalse(clock.IsRunning);
            controller.MountAudio(new DummyAudio());
            Assert.IsTrue(clock.IsRunning);
            controller.MountAudio(null);
            Assert.IsFalse(clock.IsRunning);

            Assert.IsFalse(clock.IsPlaying);
            controller.Play();
            Assert.AreEqual(0f, clock.CurrentTime, Delta);
            Assert.IsTrue(clock.IsPlaying);
            controller.Play(2000f);
            Assert.AreEqual(0f, clock.CurrentTime, Delta);

            controller.Stop();
            Assert.IsFalse(clock.IsPlaying);
            Assert.IsFalse(clock.IsPaused);
            Assert.IsTrue(clock.IsStopped);

            controller.Play(2000f);
            Assert.AreEqual(-2000f, clock.CurrentTime, Delta);
            Assert.IsTrue(clock.IsPlaying);
            Assert.IsFalse(clock.IsPaused);
            Assert.IsFalse(clock.IsStopped);

            controller.Pause();
            Assert.AreEqual(-2000f, clock.CurrentTime, Delta);
            Assert.IsFalse(clock.IsPlaying);
            Assert.IsTrue(clock.IsPaused);
            Assert.IsFalse(clock.IsStopped);

            controller.Stop();
            Assert.AreEqual(0f, clock.CurrentTime, Delta);

            Assert.AreEqual(1f, clock.Rate);
            controller.SetTempo(2f);
            Assert.AreEqual(2f, clock.Rate, Delta);

            Assert.AreEqual(0f, clock.CurrentTime, Delta);
            controller.Seek(5000f);
            Assert.AreEqual(10000f, clock.CurrentTime, Delta);
            controller.SetTempo(1f);
            Assert.AreEqual(5000, clock.CurrentTime, Delta);

            clock.Offset = 1000f;
            Assert.AreEqual(6000f, clock.CurrentTime, Delta);
        }

        private class DummyAudio : IAudio
        {
            public int Duration { get; set; } = 1000;

            public int Frequency { get; set; } = 22050;

            public int Channels { get; set; } = 2;

            public void Dispose() { }
        }

        private class DummyController : IMusicController
        {
            public event Action<IAudio> OnMounted;

            public event Action<float> OnPlay;

            public event Action<float> OnUnpause;

            public event Action OnPause;

            public event Action OnStop;

            public event Action<float> OnSeek;

            public event Action OnEnd;

            public event Action OnLoop;

            public event Action<float> OnTempo;

            public event Action<float> OnFaded;


            public AudioClock Clock { get; }

            public IAudio Audio { get; private set; }

            public bool IsPlaying => Clock.IsPlaying;

            public bool IsPaused => Clock.IsPaused;

            public bool IsStopped => Clock.IsStopped;

            public float Volume => 1f;

            public float LoopTime { get; set; } = 1f;

            public float CurrentTime { get; set; } = 0f;

            public bool IsLoop { get; set; } = false;


            public DummyController()
            {
                Clock = new AudioClock(this);
            }

            public void MountAudio(IAudio audio)
            {
                Audio = audio;
                OnMounted?.Invoke(audio);
            }

            public void Play() => OnPlay?.Invoke(0);

            public void Play(float delay) => OnPlay?.Invoke(-delay);

            public void Unpause() => OnUnpause?.Invoke(CurrentTime);

            public void Pause() => OnPause?.Invoke();

            public void Stop() => OnStop?.Invoke();

            public void Seek(float time) => OnSeek?.Invoke(time);

            public void SetVolume(float volume) { }

            public void Fade(float from, float to) { }

            public void Fade(float to) { }

            public void SetFade(float fade) { }

            public void SetTempo(float tempo) => OnTempo?.Invoke(tempo);

            public void Dispose() { }
        }
    }
}