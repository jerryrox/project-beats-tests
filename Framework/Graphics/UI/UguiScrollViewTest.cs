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

namespace PBFramework.Graphics.UI.Tests
{
    public class UguiScrollViewTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var scrollview = root.CreateChild<UguiScrollView>("scrollview");

            var horizontalScrollbar = root.CreateChild<UguiScrollBar>("horizontal-bar", 1);
            var verticalScrollbar = root.CreateChild<UguiScrollBar>("vertical-bar", 2);

            scrollview.Size = new Vector2(300, 600);
            scrollview.Container.Size = new Vector2(500, 900);
            scrollview.Background.Alpha = 0.25f;
            scrollview.HorizontalScrollbar = horizontalScrollbar;
            scrollview.VerticalScrollbar = verticalScrollbar;

            horizontalScrollbar.Position = new Vector2(0f, -302f);
            horizontalScrollbar.Size = new Vector2(300f, 4f);
            horizontalScrollbar.Background.Color = Color.grey;

            verticalScrollbar.Position = new Vector2(152f, 0f);
            verticalScrollbar.Direction = Scrollbar.Direction.BottomToTop;
            verticalScrollbar.Size = new Vector2(4f, 600f);
            verticalScrollbar.Background.Color = Color.grey;

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    scrollview.IsHorizontal = !scrollview.IsHorizontal;
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    scrollview.IsVertical = !scrollview.IsVertical;
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    scrollview.Movement++;
                    if(scrollview.Movement > ScrollRect.MovementType.Clamped)
                        scrollview.Movement = 0;
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    scrollview.Alpha = 1f;
                }
                if (Input.GetKeyDown(KeyCode.T))
                {
                    scrollview.Alpha = 0.5f;
                }
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    scrollview.Alpha = 0f;
                }

                if (Input.GetKeyDown(KeyCode.U))
                {
                    scrollview.UseMask = !scrollview.UseMask;
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    scrollview.ShowMaskingSprite = !scrollview.ShowMaskingSprite;
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    scrollview.ScrollTo(Vector2.zero);
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    scrollview.ScrollTo(new Vector2(-100, 150));
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    scrollview.ScrollTo(new Vector2(-200, 300));
                }

                yield return null;
            }
        }
    }
}