using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using PBFramework.Assets.Atlasing;
using PBFramework.Graphics.Tests;
using PBFramework.Dependencies;

namespace PBFramework.UI.Tests
{
    public class UguiInputBoxTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var inputBox = root.CreateChild<UguiInputBox>("inputBox");
            Assert.IsNotNull(inputBox.PlaceholderLabel);
            Assert.IsNotNull(inputBox.ValueLabel);
            Assert.IsNotNull(inputBox.Background);

            inputBox.OnChanged += (value) =>
            {
                Assert.AreEqual(value, inputBox.Text);
                Debug.Log("OnChanged: " + value);
            };
            inputBox.OnSubmitted += (value) =>
            {
                Assert.AreEqual(value, inputBox.Text);
                Debug.Log("OnSubmitted: " + value);
            };

            inputBox.Font = env.ArialFont.ToFont();
            inputBox.Background.Color = Color.black;

            while (env.IsRunning)
            {
                UguiLabelTest.DoUpdateTest(inputBox, KeyCode.LeftShift);
                DoUpdateTest(inputBox, KeyCode.LeftControl);
                yield return null;
            }
        }

        public static void DoUpdateTest(IInputBox inputBox, KeyCode holdKey = KeyCode.None)
        {
            if(holdKey != KeyCode.None && !Input.GetKey(holdKey))
                return;

            if(Input.GetKeyDown(KeyCode.Q))
            {
                inputBox.Placeholder = $"Placeholder {Random.Range(0, 1000)}";
            }
            if(Input.GetKeyDown(KeyCode.W))
            {
                if(inputBox.CharacterLimit > 0)
                    inputBox.CharacterLimit = 0;
                else
                    inputBox.CharacterLimit = 10;                    
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                inputBox.SelectionColor = new Color(Random.Range(0.25f, 0.75f), Random.Range(0.25f, 0.75f), Random.Range(0.25f, 0.75f));
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                inputBox.LineType += 1;
                if(inputBox.LineType > InputField.LineType.MultiLineNewline)
                    inputBox.LineType = 0;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                inputBox.InputType += 1;
                if(inputBox.InputType > InputField.InputType.Password)
                    inputBox.InputType = 0;
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                inputBox.KeyboardType += 1;
                if(inputBox.KeyboardType > TouchScreenKeyboardType.Search)
                    inputBox.KeyboardType = 0;
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                inputBox.ValidationType += 1;
                if(inputBox.ValidationType > InputField.CharacterValidation.EmailAddress)
                    inputBox.ValidationType = 0;
            }
        }
    }
}