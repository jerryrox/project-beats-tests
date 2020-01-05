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
    public class UguiToggleTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var toggle = root.CreateChild<UguiToggle>("toggle");
            toggle.Background.SpriteName = "circle-32";
            toggle.Tick.SpriteName = "circle-16";
            toggle.Tick.Color = Color.black;
            toggle.Tick.Size = new Vector2(16, 16);

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    toggle.UseFade = !toggle.UseFade;
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    toggle.Value = !toggle.Value;
                }
                yield return null;
            }
        }
    }
}