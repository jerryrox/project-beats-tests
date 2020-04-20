using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Common.Tests
{
    public class ToggleTest {

        private BasicToggle toggle;
        private LabelledToggle labelledToggle;


        [ReceivesDependency]
        private IRootMain RootMain { get; set; }


        [UnityTest]
        public IEnumerator TestBasic()
        {
            yield return TestGame.Run(
                this,
                () => InitBasic(),
                () =>
                {
                    UpdateToggleAnchor();
                }
            );
        }

        [UnityTest]
        public IEnumerator TestLabelled()
        {
            yield return TestGame.Run(
                this,
                () => InitLabelled(),
                () =>
                {
                    UpdateToggleAnchor();
                    UpdateLabelAnchor();
                }
            );
        }

        private IEnumerator InitBasic()
        {
            toggle = RootMain.CreateChild<BasicToggle>("toggle");
            toggle.Size = new Vector2(100f, 100f);
            toggle.OnTriggered += () => toggle.IsFocused = !toggle.IsFocused;
            yield break;
        }

        private IEnumerator InitLabelled()
        {
            toggle = labelledToggle = RootMain.CreateChild<LabelledToggle>("toggle");
            labelledToggle.Size = new Vector2(200f, 200f);
            labelledToggle.LabelText = "Toggle text";
            labelledToggle.OnTriggered += () => toggle.IsFocused = !toggle.IsFocused;
            labelledToggle.Tint = new Color(1f, 0.5f, 0.5f);
            yield break;
        }

        private void UpdateToggleAnchor()
        {
            if(Input.GetKeyDown(KeyCode.Q))
                toggle.IconAnchor = Anchors.TopLeft;
            if (Input.GetKeyDown(KeyCode.W))
                toggle.IconAnchor = Anchors.Top;
            if (Input.GetKeyDown(KeyCode.E))
                toggle.IconAnchor = Anchors.TopRight;
            if (Input.GetKeyDown(KeyCode.A))
                toggle.IconAnchor = Anchors.Left;
            if (Input.GetKeyDown(KeyCode.S))
                toggle.IconAnchor = Anchors.Center;
            if (Input.GetKeyDown(KeyCode.D))
                toggle.IconAnchor = Anchors.Right;
            if (Input.GetKeyDown(KeyCode.Z))
                toggle.IconAnchor = Anchors.BottomLeft;
            if (Input.GetKeyDown(KeyCode.X))
                toggle.IconAnchor = Anchors.Bottom;
            if (Input.GetKeyDown(KeyCode.C))
                toggle.IconAnchor = Anchors.BottomRight;
        }

        private void UpdateLabelAnchor()
        {
            if (Input.GetKeyDown(KeyCode.U))
                labelledToggle.LabelAnchor = TextAnchor.UpperLeft;
            if (Input.GetKeyDown(KeyCode.I))
                labelledToggle.LabelAnchor = TextAnchor.UpperCenter;
            if (Input.GetKeyDown(KeyCode.O))
                labelledToggle.LabelAnchor = TextAnchor.UpperRight;
            if (Input.GetKeyDown(KeyCode.J))
                labelledToggle.LabelAnchor = TextAnchor.MiddleLeft;
            if (Input.GetKeyDown(KeyCode.K))
                labelledToggle.LabelAnchor = TextAnchor.MiddleCenter;
            if (Input.GetKeyDown(KeyCode.L))
                labelledToggle.LabelAnchor = TextAnchor.MiddleRight;
            if (Input.GetKeyDown(KeyCode.M))
                labelledToggle.LabelAnchor = TextAnchor.LowerLeft;
            if (Input.GetKeyDown(KeyCode.Comma))
                labelledToggle.LabelAnchor = TextAnchor.LowerCenter;
            if (Input.GetKeyDown(KeyCode.Period))
                labelledToggle.LabelAnchor = TextAnchor.LowerRight;
        }
    }
}