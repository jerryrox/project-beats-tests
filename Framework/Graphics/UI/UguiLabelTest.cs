using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Assets.Fonts;
using PBFramework.Graphics.Tests;

namespace PBFramework.Graphics.UI.Tests
{
    public class UguiLabelTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(null);

            var fontInfo = SystemFontProvider.Fonts.Where(f => f.Name.Equals("Arial", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            Assert.IsNotNull(fontInfo);
            var font = new SystemFont(fontInfo);
            Assert.IsNotNull(font.Normal);

            var label = root.CreateChild<UguiLabel>("label");
            label.Font = font;
            label.Text = "This is my test text!";

            int alignmentFloor = 0;
            int alignmentIndex = 0;
            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    label.IsBold = !label.IsBold;
                    Debug.Log("Bold: " + label.IsBold);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    label.IsItalic = !label.IsItalic;
                    Debug.Log("Italic: " + label.IsItalic);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    label.WrapText = !label.WrapText;
                    Debug.Log("Wrap text: " + label.WrapText);
                }

                if (Input.GetKeyDown(KeyCode.Minus))
                {
                    label.FontSize -= 2;
                }
                else if(Input.GetKeyDown(KeyCode.Equals))
                {
                    label.FontSize += 2;
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    alignmentFloor++;
                    if(alignmentFloor >= 3)
                        alignmentFloor = 0;
                    label.Alignment = (TextAnchor)(alignmentFloor * 3 + alignmentIndex);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    alignmentFloor--;
                    if(alignmentFloor < 0)
                        alignmentFloor = 2;
                    label.Alignment = (TextAnchor)(alignmentFloor * 3 + alignmentIndex);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    alignmentIndex++;
                    if(alignmentIndex >= 3)
                        alignmentIndex = 0;
                    label.Alignment = (TextAnchor)(alignmentFloor * 3 + alignmentIndex);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    alignmentIndex--;
                    if(alignmentIndex < 0)
                        alignmentIndex = 2;
                    label.Alignment = (TextAnchor)(alignmentFloor * 3 + alignmentIndex);
                }
                yield return null;
            }
        }
    }
}