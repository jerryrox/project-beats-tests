using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using PBFramework.Assets.Atlasing;
using PBFramework.Assets.Fonts;
using PBFramework.Graphics;
using PBFramework.Graphics.Tests;
using PBFramework.Dependencies;

namespace PBFramework.UI.Tests
{
    public class UguiListViewTest {

        private IGraphicObject listViewContainer;
        private IFont font;


        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            font = env.ArialFont.ToFont();

            var listView = root.CreateChild<UguiListView>("listview");
            listViewContainer = listView.Container;
            listView.Background.Color = new Color();

            // Setup properties
            listView.Limit = 0;
            Assert.AreEqual(1, listView.Limit);
            listView.CellSize = new Vector2(150f, 30f);
            listView.Size = new Vector2(600f, 600f);

            listView.Initialize(OnCreateItem, OnUpdateItem);

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Equals))
                {
                    listView.TotalItems += 5;
                }
                else if (Input.GetKeyDown(KeyCode.Minus))
                {
                    listView.TotalItems -= 5;
                }

                if (Input.GetKeyDown(KeyCode.Q))
                    listView.ForceUpdate();
                if (Input.GetKeyDown(KeyCode.W))
                    listView.Recalibrate();

                if(Input.GetKeyDown(KeyCode.A))
                    listView.CellWidth += 10;
                else if(Input.GetKeyDown(KeyCode.S))
                    listView.CellWidth -= 10;

                if(Input.GetKeyDown(KeyCode.D))
                    listView.CellHeight += 10;
                else if(Input.GetKeyDown(KeyCode.F))
                    listView.CellHeight -= 10;

                if(Input.GetKeyDown(KeyCode.Z))
                    listView.Corner = GridLayoutGroup.Corner.UpperLeft;
                if(Input.GetKeyDown(KeyCode.X))
                    listView.Corner = GridLayoutGroup.Corner.UpperRight;
                if(Input.GetKeyDown(KeyCode.C))
                    listView.Corner = GridLayoutGroup.Corner.LowerLeft;
                if(Input.GetKeyDown(KeyCode.V))
                    listView.Corner = GridLayoutGroup.Corner.LowerRight;

                if(Input.GetKeyDown(KeyCode.B))
                    listView.Axis = GridLayoutGroup.Axis.Horizontal;
                if(Input.GetKeyDown(KeyCode.N))
                    listView.Axis = GridLayoutGroup.Axis.Vertical;

                if(Input.GetKeyDown(KeyCode.O))
                    listView.Limit ++;
                if(Input.GetKeyDown(KeyCode.P))
                    listView.Limit --;

                yield return null;
            }
        }

        private IListItem OnCreateItem()
        {
            var item = listViewContainer.CreateChild<DummyItem>("cell");
            item.MyLabel.Font = font;
            return item;
        }

        private void OnUpdateItem(IListItem item)
        {
            if (item is DummyItem dummy)
            {
                dummy.Refresh();
            }
        }

        private class DummyItem : UguiObject, IListItem
        {
            public UguiSprite MySprite { get; set; }

            public UguiLabel MyLabel { get; set; }

            public int ItemIndex { get; set; }


            [InitWithDependency]
            private void Init()
            {
                MySprite = CreateChild<UguiSprite>("sprite");
                {
                    MySprite.Anchor = Anchors.Fill;
                    MySprite.RawSize = Vector2.zero;
                    MySprite.Color = Color.black;
                }
                MyLabel = CreateChild<UguiLabel>("label", 1);
                {
                    MyLabel.Anchor = Anchors.Fill;
                    MyLabel.RawSize = Vector2.zero;
                    MyLabel.Color = Color.white;
                }

                Size = new Vector2(150f, 30f);
            }

            public void Refresh()
            {
                MyLabel.Text = ItemIndex.ToString();
            }
        }
    }
}