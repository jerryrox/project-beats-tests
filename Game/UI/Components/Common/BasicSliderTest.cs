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

namespace PBGame.UI.Components.Common.Tests
{
    public class BasicSliderTest
    {

        private BasicSlider slider;

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
                    new TestAction(true, KeyCode.Q, () => SetTint(Color.red), "Sets slider tint to red"),
                    new TestAction(true, KeyCode.W, () => SetTint(Color.green), "Sets slider tint to green"),
                    new TestAction(true, KeyCode.E, () => SetTint(Color.blue), "Sets slider tint to blue"),
                }
            };
            return TestGame.Setup(this, options).Run();
        }

        [InitWithDependency]
        private void Init()
        {
            slider = RootMain.CreateChild<BasicSlider>();
            {
                slider.Size = new Vector2(300f, 64f);
                slider.OnChange += value => Debug.Log("Value changed to: " + value);
            }
        }

        private IEnumerator SetTint(Color color)
        {
            slider.Tint = color;
            yield break;
        }
    }
}