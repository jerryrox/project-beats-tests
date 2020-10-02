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

namespace PBGame.UI.Components.System.Tests
{
    public class FpsDisplayerTest {

        private FpsDisplayer fpsDisplayer;


        [ReceivesDependency]
        private IRootMain RootMain { get; set; }
        
        
        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                Actions = new TestAction[]
                {
                    new TestAction(true, KeyCode.Q, () => SetFps(120), "Sets to 120 fps"),
                    new TestAction(true, KeyCode.W, () => SetFps(60), "Sets to 60 fps"),
                    new TestAction(true, KeyCode.E, () => SetFps(50), "Sets to 50 fps"),
                    new TestAction(true, KeyCode.R, () => SetFps(45), "Sets to 45 fps"),
                    new TestAction(true, KeyCode.T, () => SetFps(30), "Sets to 30 fps"),
                }
            };
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init()
        {
            fpsDisplayer = RootMain.CreateChild<FpsDisplayer>("fps-displayer");
            {
                fpsDisplayer.Size = new Vector2(180f, 30f);
            }
        }
        
        private IEnumerator SetFps(float fps)
        {
            Time.maximumDeltaTime = 1f / fps;
            Application.targetFrameRate = (int)fps;
            yield break;
        }
    }
}