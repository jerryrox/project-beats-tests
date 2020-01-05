using System;
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
    public class UguiSpriteTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            var atlas = new ResourceSpriteAtlas();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(atlas);

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var sprite = root.CreateChild<UguiSprite>("sprite");
            sprite.SpriteName = "circle-32";

            Assert.AreEqual(atlas, sprite.Atlas);

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    sprite.ImageType = Image.Type.Simple;
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    sprite.ImageType = Image.Type.Sliced;
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    sprite.ImageType = Image.Type.Tiled;
                
                if(Input.GetKeyDown(KeyCode.Equals))
                    sprite.Height = sprite.Width += 10;
                else if(Input.GetKeyDown(KeyCode.Minus))
                    sprite.Height = sprite.Width -= 10;
                else if(Input.GetKeyDown(KeyCode.Alpha0))
                    sprite.ResetSize();
                yield return null;
            }
        }
    }
}