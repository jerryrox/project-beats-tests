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
    public class BasicInputTest {

        private BasicInput input;


        [ReceivesDependency]
        private IRootMain RootMain { get; set; }
        
        
        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                Actions = new TestAction[]
                {
                    new TestAction(true, KeyCode.PageUp, () => ToggleFocus(), "Toggles input focus state."),
                    new TestAction(true, KeyCode.PageDown, () => CreateIcon(), "Creates icon on the input.")
                }
            };
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init()
        {
            input = RootMain.CreateChild<BasicInput>("input", 0);
            {
                input.Size = new Vector2(200f, 40f);
                input.OnFocused += (isFocused) =>
                {
                    Debug.Log("Focus state changed to: " + isFocused);
                };
                input.UseDefaultFocusAni();
                input.UseDefaultHoverAni();
            }
        }

        private IEnumerator ToggleFocus()
        {
            input.IsFocused = !input.IsFocused;
            yield break;
        }

        private IEnumerator CreateIcon()
        {
            input.CreateIconSprite(spriteName: "icon-search");
            yield break;
        }
    }
}