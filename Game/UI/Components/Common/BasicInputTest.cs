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
    public class BasicInputTest {

        private BasicInput input;


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
            yield break;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                input.IsFocused = !input.IsFocused;
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                input.CreateIconSprite(spriteName: "icon-search");
            }
        }
    }
}