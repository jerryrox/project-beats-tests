using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Graphics.Tests;
using PBFramework.Threading;
using PBFramework.Networking;

namespace PBFramework.UI.Tests
{
    public class UguiTextureTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            // Load test texture
            TextureRequest req = new TextureRequest(Path.Combine(TestConstants.TestAssetPath, "Graphics/UI/texture0.jpg"));//0.jpg"));
            var future = req.Request();
            while (!req.IsCompleted.Value)
                yield return null;

            Assert.IsNotNull(future.Output.Value);
            Assert.IsNotNull(req.Response);

            var loadedTexture = future.Output.Value;
            Assert.IsNotNull(loadedTexture);

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(null);

            var texture = root.CreateChild<UguiTexture>("texture");
            texture.Texture = loadedTexture;
            texture.Width = 400;
            texture.Height = 400;
            texture.FillTexture();

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Minus))
                {
                    texture.Width -= 100;
                    texture.Height += 100;
                    texture.FillTexture();
                }
                else if (Input.GetKeyDown(KeyCode.Equals))
                {
                    texture.Width += 100;
                    texture.Height -= 100;
                    texture.FillTexture();
                }
                yield return null;
            }
        }
    }
}