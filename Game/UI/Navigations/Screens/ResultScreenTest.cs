using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.UI.Models;
using PBGame.UI.Components.Result;
using PBGame.UI.Components.Common;
using PBGame.Maps;
using PBGame.Data.Users;
using PBGame.Data.Records;
using PBGame.Tests;
using PBGame.Rulesets;
using PBGame.Rulesets.Maps;
using PBGame.Rulesets.Scoring;
using PBGame.Rulesets.Judgements;
using PBGame.Graphics;
using PBGame.Networking.API;
using PBFramework;
using PBFramework.UI;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Threading;
using PBFramework.Dependencies;

using Random = UnityEngine.Random;

namespace PBGame.UI.Navigations.Screens.Tests
{
    public class ResultScreenTest {

        private const float Delta = 0.001f;

        private readonly string MapId = "3f997e58-157d-425b-9d57-712c2827d250";

        private readonly float[] Accuracies = new float[] { 0.35f, 0.75f, 0.85f, 0.925f, 0.975f, 1f };

        private ResultScreen resultScreen;
        private ResultModel resultModel;

        private IMapset mapset;
        private IRecord[] records;

        [ReceivesDependency]
        private IColorPreset ColorPreset { get; set; }

        [ReceivesDependency]
        private IMapSelection MapSelection { get; set; }

        [ReceivesDependency]
        private IMapManager MapManager { get; set; }

        [ReceivesDependency]
        private IRootMain RootMain { get; set; }

        [ReceivesDependency]
        private IModeManager ModeManager { get; set; }


        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                UseManualTesting = true,
                Actions = new TestAction[]
                {
                    new TestAction(() => InitTest(), "Initializes the test environment"),
                    new TestAction(KeyCode.Q, () => SetupState(0), "Selects map 0"),
                    new TestAction(KeyCode.W, () => SetupState(1), "Selects map 1"),
                    new TestAction(KeyCode.E, () => SetupState(2), "Selects map 2"),
                    new TestAction(KeyCode.R, () => SetupState(3), "Selects map 3"),
                    new TestAction(KeyCode.T, () => SetupState(4), "Selects map 4"),
                    new TestAction(KeyCode.Y, () => SetupState(5), "Selects map 5"),
                    new TestAction(KeyCode.A, () => ResetState(), "Resets screen state."),
        },
            };
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init()
        {
            resultScreen = RootMain.CreateChild<ResultScreen>("result");
            {
                resultScreen.Anchor = AnchorType.Fill;
                resultScreen.Offset = Offset.Zero;

                resultModel = resultScreen.Model;
            }
        }

        private IEnumerator InitTest()
        {
            // Load mapsets.
            var loadListener = new TaskListener<IMapset>();
            MapManager.Load(new Guid(MapId), loadListener);
            while(!loadListener.IsFinished)
                yield return null;

            Assert.IsTrue(loadListener.IsFinished);
            Assert.IsNotNull(loadListener.Value);
            Assert.AreEqual(6, loadListener.Value.Maps.Count);
            mapset = loadListener.Value;

            // Generate test records for the maps.
            int index = 0;
            records = mapset.Maps.Select((map) =>
            {
                var scoreProcessor = ModeManager.GetService(GameModeType.BeatsStandard).CreateScoreProcessor();
                scoreProcessor.Accuracy.Value = Accuracies[index];
                scoreProcessor.Ranking.Value = scoreProcessor.CalculateRank(Accuracies[index]);
                scoreProcessor.HighestCombo.Value = map.HitObjects.Count();
                scoreProcessor.Score.Value = map.HitObjects.Count();

                index++;
                return new Record(
                    map.GetPlayable(GameModeType.BeatsStandard),
                    new User(new OfflineUser()),
                    scoreProcessor,
                    map.Duration
                );
            }).ToArray();

            yield break;
        }

        private IEnumerator SetupState(int index)
        {
            resultModel.Setup(mapset.Maps[index].GetPlayable(GameModeType.BeatsStandard), records[index]);

            yield return new WaitForSeconds(0.1f);
            // TODO: Assertion
            yield break;
        }

        private IEnumerator ResetState()
        {
            resultModel.Setup(null, null);

            InfoBlock infoBlock = resultScreen.GetComponentInChildren<InfoBlock>(true);
            {
                Assert.IsTrue(string.IsNullOrEmpty(infoBlock.FindWithName<Label>("title").Text));
                Assert.IsTrue(string.IsNullOrEmpty(infoBlock.FindWithName<Label>("artist").Text));
                Assert.IsTrue(string.IsNullOrEmpty(infoBlock.FindWithName<Label>("version").Text));
                Assert.AreEqual("mapped by", infoBlock.FindWithName<Label>("mapper").Text.Trim());
            }
            InfoStrip infoStrip = resultScreen.GetComponentInChildren<InfoStrip>(true);
            {
                Assert.AreEqual("0", infoStrip.FindWithName<Label>("score").Text);
                Assert.AreEqual("x0", infoStrip.FindWithName<Label>("combo").Text);
                Assert.IsNull(infoStrip.FindWithName<UguiTexture>("texture").Texture);
                Assert.AreEqual("", infoStrip.FindWithName<Label>("name").Text);
                Assert.AreEqual("", infoStrip.FindWithName<Label>("date").Text);
            }
            RankCircle rankCircle = resultScreen.GetComponentInChildren<RankCircle>(true);
            {
                Assert.AreEqual(0f, rankCircle.FindWithName<UguiSprite>("meter").FillAmount);
                Assert.IsTrue(rankCircle.FindWithName<MapImageDisplay>("thumb").GetComponentsInChildren<ITexture>(true).Any(t => t.Active == false));
                Assert.AreNotEqual(1f, rankCircle.FindWithName<UguiSprite>("rank-glow").Alpha);
                Assert.AreNotEqual(0f, rankCircle.FindWithName<UguiSprite>("rank-glow").Alpha);
                Assert.AreEqual(ColorPreset.GetRankColor(RankType.D).Base.WithAlpha(0.625f), rankCircle.FindWithName<UguiSprite>("rank-glow").Tint);
                Assert.AreEqual(ColorPreset.GetRankColor(RankType.D).Base, rankCircle.FindWithName<Label>("rank").Color);
                Assert.AreEqual(RankType.D.ToString(), rankCircle.FindWithName<Label>("rank").Text);
                Assert.AreEqual("0.00%", rankCircle.FindWithName<Label>("accuracy").Text);

                RankCircleRange range = rankCircle.GetComponentInChildren<RankCircleRange>(true);
                {
                    Assert.IsFalse(range.Active);
                }
            }
            yield break;
        }
    }
}