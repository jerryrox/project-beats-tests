using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
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

        private IEnumerator TriggerTick()
        {
            for (int i = 0; i < metronomeDisplay.TickCount; i++)
            {
                Assert.IsTrue(metronomeDisplay.TriggerTick(i));
                yield return new WaitForSeconds(0.25f);
            }
            yield break;
        }
    }
}