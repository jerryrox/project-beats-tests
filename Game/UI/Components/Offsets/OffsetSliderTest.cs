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
            yield return TestGame.Run(
                this,
                () => Init(),
                Update
            );
        }
        
        private IEnumerator Init()
        {
            slider = RootMain.CreateChild<OffsetSlider>("slider", 0);
            {
                slider.Size = new Vector2(400f, 100f);
                slider.LabelText = "Offset test";
            }
            yield break;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                TestOffset newOffset = new TestOffset() { Offset = new BindableInt(Random.Range(-100, 101)) };
                slider.SetSource(newOffset);
                Debug.Log("Created new offset with value: " + newOffset.Offset);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                slider.SetSource(null);
                Debug.Log("Removed offset");
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log(slider.CurOffset == null ? "Null source" : "Cur offset value: " + slider.CurOffset.Offset);
            }
        }

        private class TestOffset : IMusicOffset
        {
            public BindableInt Offset { get; set; } = new BindableInt(0);
        }
    }
}