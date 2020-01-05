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
    public class UguiScrollBarTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var scrollbar = root.CreateChild<UguiScrollBar>("scrollbar");
            scrollbar.Foreground.SpriteName = "circle-16";
            scrollbar.Background.SpriteName = "circle-16";
            scrollbar.Background.Color = new Color(0.125f, 0.125f, 0.125f);

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    scrollbar.Direction = Scrollbar.Direction.LeftToRight;
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    scrollbar.Direction = Scrollbar.Direction.TopToBottom;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    scrollbar.Direction = Scrollbar.Direction.RightToLeft;
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    scrollbar.Direction = Scrollbar.Direction.BottomToTop;
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    scrollbar.Value = 0f;
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    scrollbar.Value = 0.5f;
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    scrollbar.Value = 1f;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    scrollbar.ForegroundSize = 0f;
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    scrollbar.ForegroundSize = 0.25f;
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    scrollbar.ForegroundSize = 0.5f;
                }

                if (Input.GetKeyDown(KeyCode.J))
                {
                    scrollbar.Steps = 0;
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    scrollbar.Steps = 5;
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    scrollbar.Steps = 10;
                }
                yield return null;
            }
        }
    }
}