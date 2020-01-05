using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Assets.Atlasing;
using PBFramework.Graphics.Tests;
using PBFramework.Dependencies;

namespace PBFramework.UI.Tests
{
    public class UguiButtonTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var button = root.CreateChild<UguiButton>("button");
            button.Label.Font = env.ArialFont.ToFont();
            button.Background.Color = Color.green;

            button.Label.Text = "Press me!";

            int clickCount = 0;
            button.OnClick += () =>
            {
                clickCount++;
                Debug.Log("Clicked");
            };
            Assert.AreEqual(0, clickCount);

            button.InvokeClick();
            Assert.AreEqual(1, clickCount);

            while (env.IsRunning)
            {
                yield return null;
            }
        }
    }
}