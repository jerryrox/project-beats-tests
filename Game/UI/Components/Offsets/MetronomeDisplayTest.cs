using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Audio;
using PBGame.Tests;
using PBGame.Rulesets.Maps;
using PBGame.Graphics;
using PBFramework.Data.Bindables;
using PBFramework.Audio;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Offsets.Tests
{
    public class MetronomeDisplayTest {

        private MetronomeDisplay metronomeDisplay;


        [ReceivesDependency]
        private IRootMain RootMain { get; set; }
        
        
        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                UseManualTesting = true,
                Actions = new TestAction[]
                {
                    new TestAction(() => AutoTest()),
                    new TestAction(true, KeyCode.Q, () => SetupTicks(4), "Sets up 4 ticks"),
                    new TestAction(true, KeyCode.W, () => ClearTicks(), "Removes all ticks"),
                    new TestAction(true, KeyCode.E, () => TriggerTick(), "Triggers all ticks consecutively"),
                },
            };
            return TestGame.Setup(this, options).Run();
        }

        [UnityTest]
        public IEnumerator TestWithMetronome()
        {
            TestOptions options = new TestOptions()
            {
                UseManualTesting = true,
                Actions = new TestAction[]
                {
                    new TestAction(false, KeyCode.Q, () => AutoTestWithMetronome(), "Runs auto test with metronome"),
                },
            };
            return TestGame.Setup(this, options).Run();
        }

        [InitWithDependency]
        private void Init()
        {
            metronomeDisplay = RootMain.CreateChild<MetronomeDisplay>();
            {
                metronomeDisplay.Size = new Vector2(600f, 24f);
            }
        }

        private IEnumerator AutoTest()
        {
            yield return SetupTicks(2);
            yield return SetupTicks(1);
            yield return ClearTicks();
            yield return SetupTicks(3);
            yield return TriggerTick();
            yield return ClearTicks();
        }

        private IEnumerator AutoTestWithMetronome()
        {
            DummyMetronome metronome = new DummyMetronome();
            yield return SetMetronome(metronome);
            yield return RemoveMetronome();
            yield return SetMetronome(metronome);

            metronome.TestBeatsInInterval.Value = 8;
            Assert.AreEqual(8, metronomeDisplay.TickCount);
            yield return TriggerTick(8);

            metronome.TestBeatsInInterval.Value = 6;
            Assert.AreEqual(6, metronomeDisplay.TickCount);
            yield return TriggerTick(6);

            yield return RemoveMetronome();
        }

        private IEnumerator SetupTicks(int count)
        {
            metronomeDisplay.SetupTicks(count);
            Assert.AreEqual(count, metronomeDisplay.TickCount);
            yield break;
        }

        private IEnumerator ClearTicks()
        {
            metronomeDisplay.ClearTicks();
            Assert.AreEqual(0, metronomeDisplay.TickCount);
            yield break;
        }

        private IEnumerator TriggerTick(int? overrideCount = null)
        {
            int loops = overrideCount.HasValue ? overrideCount.Value : metronomeDisplay.TickCount;
            for (int i = 0; i < loops; i++)
            {
                Assert.IsTrue(metronomeDisplay.TriggerTick(i));
                yield return new WaitForSeconds(0.25f);
            }
            yield break;
        }

        private IEnumerator SetMetronome(IMetronome metronome)
        {
            metronomeDisplay.SetMetronome(metronome);
            Assert.AreEqual(metronome, metronomeDisplay.CurMetronome);
            Assert.AreEqual(metronome.BeatsInInterval.Value, metronomeDisplay.TickCount);
            yield break;
        }

        private IEnumerator RemoveMetronome()
        {
            metronomeDisplay.RemoveMetronome();
            Assert.IsNull(metronomeDisplay.CurMetronome);
            yield break;
        }


        private class DummyMetronome : IMetronome
        {
            public event Action OnBeat;

            public BindableInt TestBeatsInInterval = new BindableInt(4);
            public BindableInt TestBeatIndex = new BindableInt(0);

            public IPlayableMap CurrentMap { get; set; }
            public IAudioController AudioController { get; set; }
            public IReadOnlyBindable<int> BeatIndex => TestBeatIndex;
            public IReadOnlyBindable<int> BeatsInInterval => TestBeatsInInterval;
            public Bindable<BeatFrequency> Frequency { get; set; }
            public IReadOnlyBindable<float> BeatLength { get; }

            public void Update() { }
        }
    }
}