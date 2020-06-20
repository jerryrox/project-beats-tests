using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBFramework.UI;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Offsets.Tests
{
    public class MetronomeTickTest {

        private MetronomeTick ticker;


        [ReceivesDependency]
        private IRootMain RootMain { get; set; }
        
        
        [UnityTest]
        public IEnumerator Test()
        {
            yield return TestGame.Run(
                this,
                () => Init(),
                Update,
                keyBindings: new TestKeyBinding[] {
                    new TestKeyBinding(KeyCode.Q, DoTick, "Sets the metronome tick to its Ticked state.")
                }
            );
        }
        
        private IEnumerator Init()
        {
            ticker = RootMain.CreateChild<MetronomeTick>("ticker");
            ticker.Size = new Vector2(24f, 24f);
            ticker.Tint = new Color(1f, 0.2f, 0.2f);
            yield break;
        }

        private void Update()
        {
        }

        private void DoTick()
        {
            ticker.Tick();
        }
    }
}