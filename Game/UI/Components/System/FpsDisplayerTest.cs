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

namespace PBGame.UI.Components.System.Tests
{
    public class FpsDisplayerTest {

        private FpsDisplayer fpsDisplayer;

        [ReceivesDependency]
        private IRootMain RootMain { get; set; }
        
        
        [UnityTest]
        public IEnumerator Test()
        {
            yield return TestGame.Run(
                this,
                () => Init(),
                Update
            );
        }
        
        private IEnumerator Init()
        {

            fpsDisplayer = RootMain.CreateChild<FpsDisplayer>("fps-displayer");
            {
                fpsDisplayer.Size = new Vector2(180f, 30f);
            }
            yield break;
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                SetFps(120);
            if (Input.GetKeyDown(KeyCode.W))
                SetFps(60);
            if (Input.GetKeyDown(KeyCode.E))
                SetFps(50);
            if (Input.GetKeyDown(KeyCode.R))
                SetFps(40);
            if (Input.GetKeyDown(KeyCode.T))
                SetFps(30);
        }

        private void SetFps(float fps)
        {
            Time.maximumDeltaTime = 1f / fps;
            Application.targetFrameRate = (int)fps;
        }
    }
}