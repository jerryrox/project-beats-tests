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
    public class UguiSliderTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var slider = root.CreateChild<UguiSlider>("slider");

            slider.Background.SpriteName = "circle-16";
            slider.Background.Color = Color.black;

            slider.Foreground.SpriteName = "circle-16";
            slider.Foreground.Color = Color.green;

            slider.Thumb.SpriteName = "circle-32";
            slider.Thumb.Size = new Vector2(48f, 48f);

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    slider.Direction = Slider.Direction.LeftToRight;
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    slider.Direction = Slider.Direction.TopToBottom;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    slider.Direction = Slider.Direction.RightToLeft;
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    slider.Direction = Slider.Direction.BottomToTop;
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    slider.Value = 0f;
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    slider.Value = 0.5f;
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    slider.Value = 1f;
                }

                yield return null;
            }
        }
    }
}