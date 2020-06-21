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
            TestOptions options = new TestOptions()
            {
                UseManualTesting = true,
                Actions = new TestAction[]
                {
                    new TestAction(() => InitBasic()),
                    new TestAction(true, KeyCode.Q, () => UpdateToggleAnchor(AnchorType.TopLeft), "Sets anchor to TopLeft"),
                    new TestAction(true, KeyCode.W, () => UpdateToggleAnchor(AnchorType.Top), "Sets anchor to Top"),
                    new TestAction(true, KeyCode.E, () => UpdateToggleAnchor(AnchorType.TopRight), "Sets anchor to TopRight"),
                    new TestAction(true, KeyCode.A, () => UpdateToggleAnchor(AnchorType.Left), "Sets anchor to Left"),
                    new TestAction(true, KeyCode.S, () => UpdateToggleAnchor(AnchorType.Center), "Sets anchor to Center"),
                    new TestAction(true, KeyCode.D, () => UpdateToggleAnchor(AnchorType.Right), "Sets anchor to Right"),
                    new TestAction(true, KeyCode.Z, () => UpdateToggleAnchor(AnchorType.BottomLeft), "Sets anchor to BottomLeft"),
                    new TestAction(true, KeyCode.X, () => UpdateToggleAnchor(AnchorType.Bottom), "Sets anchor to Bottom"),
                    new TestAction(true, KeyCode.C, () => UpdateToggleAnchor(AnchorType.BottomRight), "Sets anchor to BottomRight"),
                }
            };
            return TestGame.Setup(this, options).Run();
        }

        [UnityTest]
        public IEnumerator TestLabelled()
        {
            TestOptions options = new TestOptions()
            {
                UseManualTesting = true,
                Actions = new TestAction[]
                {
                    new TestAction(() => InitLabelled()),
                    new TestAction(true, KeyCode.Q, () => UpdateToggleAnchor(AnchorType.TopLeft), "Sets toggle anchor to TopLeft"),
                    new TestAction(true, KeyCode.W, () => UpdateToggleAnchor(AnchorType.Top), "Sets toggle anchor to Top"),
                    new TestAction(true, KeyCode.E, () => UpdateToggleAnchor(AnchorType.TopRight), "Sets toggle anchor to TopRight"),
                    new TestAction(true, KeyCode.A, () => UpdateToggleAnchor(AnchorType.Left), "Sets toggle anchor to Left"),
                    new TestAction(true, KeyCode.S, () => UpdateToggleAnchor(AnchorType.Center), "Sets toggle anchor to Center"),
                    new TestAction(true, KeyCode.D, () => UpdateToggleAnchor(AnchorType.Right), "Sets toggle anchor to Right"),
                    new TestAction(true, KeyCode.Z, () => UpdateToggleAnchor(AnchorType.BottomLeft), "Sets toggle anchor to BottomLeft"),
                    new TestAction(true, KeyCode.X, () => UpdateToggleAnchor(AnchorType.Bottom), "Sets toggle anchor to Bottom"),
                    new TestAction(true, KeyCode.C, () => UpdateToggleAnchor(AnchorType.BottomRight), "Sets toggle anchor to BottomRight"),

                    new TestAction(true, KeyCode.U, () => UpdateLabelAnchor(TextAnchor.UpperLeft), "Sets label anchor to UpperLeft"),
                    new TestAction(true, KeyCode.I, () => UpdateLabelAnchor(TextAnchor.UpperCenter), "Sets label anchor to UpperCenter"),
                    new TestAction(true, KeyCode.O, () => UpdateLabelAnchor(TextAnchor.UpperRight), "Sets label anchor to UpperRight"),
                    new TestAction(true, KeyCode.J, () => UpdateLabelAnchor(TextAnchor.MiddleLeft), "Sets label anchor to MiddleLeft"),
                    new TestAction(true, KeyCode.K, () => UpdateLabelAnchor(TextAnchor.MiddleCenter), "Sets label anchor to MiddleCenter"),
                    new TestAction(true, KeyCode.L, () => UpdateLabelAnchor(TextAnchor.MiddleRight), "Sets label anchor to MiddleRight"),
                    new TestAction(true, KeyCode.M, () => UpdateLabelAnchor(TextAnchor.LowerLeft), "Sets label anchor to LowerLeft"),
                    new TestAction(true, KeyCode.Comma, () => UpdateLabelAnchor(TextAnchor.LowerCenter), "Sets label anchor to LowerCenter"),
                    new TestAction(true, KeyCode.Period, () => UpdateLabelAnchor(TextAnchor.LowerRight), "Sets label anchor to LowerRight"),
                }
            };
            return TestGame.Setup(this, options).Run();
        }

        private IEnumerator InitBasic()
        {
            toggle = RootMain.CreateChild<BasicToggle>("toggle");
            toggle.Size = new Vector2(100f, 100f);
            yield break;
        }

        private IEnumerator InitLabelled()
        {
            toggle = labelledToggle = RootMain.CreateChild<LabelledToggle>("toggle");
            labelledToggle.Size = new Vector2(200f, 200f);
            labelledToggle.LabelText = "Toggle text";
            labelledToggle.Tint = new Color(1f, 0.5f, 0.5f);
            yield break;
        }

        private IEnumerator UpdateToggleAnchor(AnchorType type)
        {
            toggle.IconAnchor = type;
            yield break;
        }

        private IEnumerator UpdateLabelAnchor(TextAnchor type)
    {
            labelledToggle.LabelAnchor = type;
            yield break;
        }
    }
}