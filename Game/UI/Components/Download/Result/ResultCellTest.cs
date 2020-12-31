using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.UI.Models;
using PBGame.UI.Components.Common.MetaTags;
using PBGame.Tests;
using PBGame.Rulesets;
using PBGame.Graphics;
using PBGame.Networking.Maps;
using PBFramework.UI;
using PBFramework.Testing;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Download.Result.Tests
{
    public class ResultCellTest {

        private ResultCell cell;

        [ReceivesDependency]
        private IRootMain RootMain { get; set; }

        [ReceivesDependency]
        private IModeManager ModeManager { get; set; }


        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                Actions = new TestAction[]
                {
                    new TestAction(KeyCode.Q, () => AssignMapset(), "Assigns mapset to the result cell"),
                    new TestAction(KeyCode.W, () => RemoveMapset(), "Removes mapset from the result cell"),
                    new TestAction(true, KeyCode.Z, () => AssignMapset(), "Performs manual test."),
                },
            };
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init(IDependencyContainer dependency)
        {
            dependency.Cache(new DownloadModel());

            cell = RootMain.CreateChild<ResultCell>("Cell");
            {
                cell.Size = new Vector2(300f, 180f);
            }
        }

        private IEnumerator AssignMapset()
        {
            OnlineMapset mapset = new OnlineMapset()
            {
                Artist = "Test artist",
                Bpm = 200,
                Creator = "Test creator",
                FavoriteCount = 50,
                Id = 15342,
                LastUpdate = DateTime.Now,
                Maps = new OnlineMap[]
                {
                    new OnlineMap()
                    {
                        Accuracy = 5.5f,
                        AR = 9,
                        Bpm = 200,
                        CircleCount = 1363,
                        CS = 4,
                        Difficulty = 2.23f,
                        Drain = 6,
                        HitDuration = 132,
                        Id = 2362,
                        ModeIndex = (int)GameProviderType.Osu + 0,
                        SliderCount = 25,
                        SpinnerCount = 1,
                        TotalCount = 3523,
                        TotalDuration = 13,
                        Version = "Test version",
                    }
                },
                PlayCount = 1366,
                Source = "Test source",
                Tags = "test tags",
                Title = "Test title",
                Status = "Test status",
                CardImage = "",
                CoverImage = "",
                HasStoryboard = false,
                HasVideo = false,
                IsDisabled = false,
                PreviewAudio = "",
            };
            cell.Setup(mapset);
            {
                Assert.AreEqual("Test title", cell.FindFromPath<Label>("container/title").Text);
                Assert.AreEqual("Test artist", cell.FindFromPath<Label>("container/artist").Text);
                Assert.AreEqual("Test creator", cell.FindFromPath<Label>("container/mapper").Text);

                var metaDisplayer = cell.FindFromPath<MetaDisplayer>("container/meta");
                {
                    Assert.AreEqual("Test status", metaDisplayer.FindFromPath<RankMetaTag>("rank").LabelText);
                    Assert.AreEqual("1,366", metaDisplayer.FindFromPath<StatMetaTag>("stat").LabelText);
                    Assert.AreEqual("50", metaDisplayer.FindFromPath<StatMetaTag>("favorite").LabelText);
                    Assert.AreEqual("icon-play", metaDisplayer.FindWithName<UguiSprite>("icon").SpriteName);
                    Assert.AreEqual(1, metaDisplayer.GetComponentsInChildren<MapMetaTag>(false).Length);

                    var tag = metaDisplayer.GetComponentInChildren<MapMetaTag>(false);
                    Assert.AreEqual(
                        ModeManager.GetService(GameModeType.OsuStandard).GetIconName(32),
                        tag.FindWithName<UguiSprite>("icon").SpriteName
                    );
                    Assert.AreEqual("1", tag.LabelText);
                }
            }
            yield return null;
        }

        private IEnumerator RemoveMapset()
        {
            cell.Setup(null);
            yield return null;
        }
    }
}