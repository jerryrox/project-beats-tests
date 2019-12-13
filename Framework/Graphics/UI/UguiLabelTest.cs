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

            while (env.IsRunning)
            {
                DoUpdateTest(label);
                yield return null;
            }
        }

        public static void DoUpdateTest(ILabel label, KeyCode holdKey = KeyCode.None)
        {
            if(holdKey != KeyCode.None && !Input.GetKey(holdKey))
                return;

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
                label.Alignment += 3;
                if(label.Alignment > TextAnchor.LowerRight)
                    label.Alignment = (TextAnchor)((int)label.Alignment % ((int)TextAnchor.LowerRight + 1));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                label.Alignment -= 3;
                if(label.Alignment < TextAnchor.UpperLeft)
                    label.Alignment += (int)TextAnchor.LowerRight + 1;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                label.Alignment += 1;
                if(label.Alignment > TextAnchor.LowerRight)
                    label.Alignment = (TextAnchor)((int)label.Alignment % ((int)TextAnchor.LowerRight + 1));
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                label.Alignment -= 1;
                if(label.Alignment < TextAnchor.UpperLeft)
                    label.Alignment += (int)TextAnchor.LowerRight + 1;
            }
        }
    }
}