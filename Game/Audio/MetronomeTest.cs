using System;
using System.Collections;
using System.Collections.Generic;
using PBGame.Rulesets.Maps.Timing;
using PBGame.Rulesets.Maps.ControlPoints;
using PBFramework.Audio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.Audio.Tests
{
    public class MetronomeTest {

        private const float Delta = 0.001f;


        [Test]
        public void TestBeat()
        {
            int beatCount = 0;

            DummyMusicController musicController = new DummyMusicController();
            Metronome metronome = new Metronome()
            {
                AudioController = musicController
            };
            metronome.OnBeat += () => beatCount++;

            Assert.AreEqual(TimingControlPoint.DefaultBeatLength, metronome.BeatLength.Value, Delta);
            Assert.AreEqual(0, beatCount);

            metronome.Update();
            Assert.AreEqual(1, beatCount);

            musicController.CurrentTime = metronome.BeatLength.Value * 0.5f;
            metronome.Update();
            Assert.AreEqual(1, beatCount);

            musicController.CurrentTime = metronome.BeatLength.Value;
            metronome.Update();
            Assert.AreEqual(2, beatCount);

            musicController.CurrentTime = metronome.BeatLength.Value * 1.5f;
            metronome.Update();
            Assert.AreEqual(2, beatCount);

            musicController.CurrentTime = metronome.BeatLength.Value * 2f;
            metronome.Update();
            Assert.AreEqual(3, beatCount);

            musicController.Stop();
            metronome.Update();
            Assert.AreEqual(4, beatCount);

            musicController.Seek(metronome.BeatLength.Value * 3f);
            metronome.Update();
            Assert.AreEqual(5, beatCount);

            musicController.CurrentTime = metronome.BeatLength.Value * 4f;
            metronome.Update();
            Assert.AreEqual(6, beatCount);
        }

        [Test]
        public void TestBeatIndex()
        {
            DummyMusicController musicController = new DummyMusicController();
            Metronome metronome = new Metronome()
            {
                AudioController = musicController
            };

            Assert.AreEqual(BeatFrequency.Full, metronome.Frequency.Value);
            Assert.AreEqual((int)TimeSignatureType.Quadruple * (int)BeatFrequency.Full, metronome.BeatsInInterval.Value);
            Assert.AreEqual(0, metronome.BeatIndex.Value);

            musicController.CurrentTime = metronome.BeatLength.Value * 0.5f;
            metronome.Update();
            Assert.AreEqual(0, metronome.BeatIndex.Value);

            for (int i = 1; i < 4; i++)
            {
                musicController.CurrentTime = metronome.BeatLength.Value * i;
                metronome.Update();
                Assert.AreEqual(i, metronome.BeatIndex.Value);
            }

            musicController.CurrentTime = metronome.BeatLength.Value * 4f;
            metronome.Update();
            Assert.AreEqual(0, metronome.BeatIndex.Value);

            metronome.Frequency.Value = BeatFrequency.Half;
            musicController.CurrentTime = metronome.BeatLength.Value * 8f;
            metronome.Update();
            Assert.AreEqual(0, metronome.BeatIndex.Value);

            for (int i = 1; i < 8; i++)
            {
                musicController.CurrentTime = metronome.BeatLength.Value * (i + 8f);
                metronome.Update();
                Assert.AreEqual(i, metronome.BeatIndex.Value);
            }

            musicController.CurrentTime = metronome.BeatLength.Value * 16f;
            metronome.Update();
            Assert.AreEqual(0, metronome.BeatIndex.Value);

            metronome.Frequency.Value = BeatFrequency.Quarter;
            musicController.CurrentTime = metronome.BeatLength.Value * 32f;
            metronome.Update();
            Assert.AreEqual(0, metronome.BeatIndex.Value);

            for (int i = 1; i < 16; i++)
            {
                musicController.CurrentTime = metronome.BeatLength.Value * (i + 32f);
                metronome.Update();
                Assert.AreEqual(i, metronome.BeatIndex.Value);
            }

            musicController.CurrentTime = metronome.BeatLength.Value * 48f;
            metronome.Update();
            Assert.AreEqual(0, metronome.BeatIndex.Value);

            musicController.CurrentTime = metronome.BeatLength.Value * 49f;
            metronome.Update();
            Assert.AreEqual(1, metronome.BeatIndex.Value);

            musicController.Stop();
            metronome.Update();
            Assert.AreEqual(0, metronome.BeatIndex.Value);

            musicController.Seek(metronome.BeatLength.Value * 8);
            metronome.Update();
            Assert.AreEqual(8, metronome.BeatIndex.Value);
        }

        [Test]
        public void TestChangeAudioController()
        {
            DummyMusicController controller1 = new DummyMusicController();
            DummyMusicController controller2 = new DummyMusicController();
            Metronome metronome = new Metronome();

            controller1.CurrentTime = metronome.BeatLength.Value * 1f;
            controller2.CurrentTime = metronome.BeatLength.Value * 3f;

            metronome.AudioController = controller1;
            Assert.AreEqual(1, metronome.BeatIndex.Value);

            metronome.AudioController = controller2;
            Assert.AreEqual(3, metronome.BeatIndex.Value);

            controller1.Seek(4);
            Assert.AreEqual(3, metronome.BeatIndex.Value);

            metronome.AudioController = controller1;
            Assert.AreEqual(0, metronome.BeatIndex.Value);

            controller1.Seek(metronome.BeatLength.Value * 6);
            Assert.AreEqual(2, metronome.BeatIndex.Value);
        }

        private class DummyMusicController : IAudioController
        {
            public event Action<IAudio> OnMounted;
            public event Action<float> OnPlay;
            public event Action<float> OnUnpause;
            public event Action OnPause;
            public event Action OnStop;
            public event Action<float> OnSeek;
            public event Action OnEnd;
            public event Action OnLoop;

            public IAudio Audio { get; }
            public bool IsPlaying { get; }
            public bool IsPaused { get; }
            public bool IsStopped { get; }
            public float Volume { get; }
            public float LoopTime { get; set; }
            public float CurrentTime { get; set; }
            public float Progress { get; }
            public bool IsLoop { get; set; }

            public void MountAudio(IAudio audio) {}
            public void Play() {}
            public void Play(float delay) {}
            public void Pause() {}
            public void Stop()
            {
                CurrentTime = 0f;
                OnStop?.Invoke();
            }
            public void Seek(float time)
            {
                CurrentTime = time;
                OnSeek?.Invoke(time);
            }
            public void SetVolume(float volume) {}
            public void Dispose() {}
        }
    }
}