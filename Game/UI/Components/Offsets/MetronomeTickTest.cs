using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBFramework.UI;
using PBFramework.Testing;
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
            TestOptions options = new TestOptions()
            {
                Actions = new TestAction[]
                {
                    new TestAction(false, KeyCode.Q, () => DoTick(), "Sets the metronome tick to its Ticked state.")
                }
            };
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init()
        {
            ticker = RootMain.CreateChild<MetronomeTick>("ticker");
            ticker.Size = new Vector2(24f, 24f);
            ticker.Tint = new Color(1f, 0.2f, 0.2f);
        }

        private IEnumerator DoTick()
        {
            ticker.Tick();
            yield break;
        }
    }
}