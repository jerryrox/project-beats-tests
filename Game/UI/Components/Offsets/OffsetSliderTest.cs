using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Audio;
using PBGame.Graphics;
using PBFramework.UI;
using PBFramework.Data.Bindables;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Offsets.Tests
{
    public class OffsetSliderTest {

        private OffsetSlider slider;


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
                    new TestAction(true, KeyCode.Q, () => CreateOffset(), "Creates a new offset instance to modify with slider."),
                    new TestAction(true, KeyCode.W, () => CreateOffset(), "Removes current offset attached to the slider."),
                    new TestAction(true, KeyCode.E, () => CreateOffset(), "Logs current offset value to the console."),
                }
            };
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init()
        {
            slider = RootMain.CreateChild<OffsetSlider>("slider", 0);
            {
                slider.Size = new Vector2(400f, 100f);
                slider.LabelText = "Offset test";
            }
        }

        private IEnumerator CreateOffset()
        {
            TestOffset newOffset = new TestOffset() { Offset = new BindableInt(Random.Range(-100, 101)) };
            slider.SetSource(newOffset);
            Debug.Log("Created new offset with value: " + newOffset.Offset);
            yield break;
        }

        private IEnumerator RemoveOffset()
        {
            slider.SetSource(null);
            Debug.Log("Removed offset");
            yield break;
        }

        private IEnumerator LogOffset()
        {
                Debug.Log(slider.CurOffset == null ? "Null source" : "Cur offset value: " + slider.CurOffset.Offset);
            yield break;
        }

        private class TestOffset : IMusicOffset
        {
            public BindableInt Offset { get; set; } = new BindableInt(0);
        }
    }
}