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

namespace PBGame.UI.Components.Common.Tests
{
    public class BasicSliderTest {

        private BasicSlider slider;

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
            slider = RootMain.CreateChild<BasicSlider>();
            {
                slider.Size = new Vector2(300f, 64f);
                slider.OnChange += value => Debug.Log("Value changed to: " + value);
            }
            yield break;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                slider.Tint = Color.red;
            else if (Input.GetKeyDown(KeyCode.W))
                slider.Tint = Color.green;
            else if (Input.GetKeyDown(KeyCode.E))
                slider.Tint = Color.blue;
        }
    }
}