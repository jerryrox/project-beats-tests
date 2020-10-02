using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Data.Users;
using PBGame.Rulesets;
using PBGame.Rulesets.Objects;
using PBGame.Rulesets.Maps;
using PBGame.Rulesets.Maps.Timing;
using PBGame.Rulesets.Maps.ControlPoints;
using PBGame.Rulesets.Scoring;
using PBGame.Rulesets.Judgements;
using PBGame.Rulesets.Difficulty;
using PBGame.Networking.API;
using PBFramework.Data.Queries;
using PBFramework.Data.Bindables;

namespace PBGame.Data.Records.Tests
{
    public class RecordTest {
 
        private const float Delta = 0.001f;

        [Test]
        public void TestInitialize()
        {
            var curDate = DateTime.Now;

            var record = new Record(
                new DummyMap(),
                new User(new OfflineUser())
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000001")
                },
                new DummyScoreProcessor()
                {
                    Accuracy = new BindableFloat(0.55f),
                    Ranking = new Bindable<RankType>(RankType.B),
                    HighestCombo = new BindableInt(1000),
                    Score = new BindableInt(12345678),
                    Judgements = new List<JudgementResult>()
                    {
                        new JudgementResult(new JudgementInfo())
                        {
                            ComboAtJudgement = 0,
                            HitOffset = 1,
                            HitResult = HitResultType.Perfect,
                            HighestComboAtJudgement = 0,
                        },
                        new JudgementResult(new JudgementInfo())
                        {
                            ComboAtJudgement = 1,
                            HitOffset = 2,
                            HitResult = HitResultType.Great,
                            HighestComboAtJudgement = 1,
                        },
                        new JudgementResult(new JudgementInfo())
                        {
                            ComboAtJudgement = 2,
                            HitOffset = 5,
                            HitResult = HitResultType.Miss,
                            HighestComboAtJudgement = 2,
                        },
                    },
                },
                100
            );
            Assert.AreEqual("00000000-0000-0000-0000-000000000001", record.UserId.ToString());
            Assert.AreEqual("0x0", record.MapHash);
            Assert.AreEqual(GameModeType.BeatsStandard, record.GameMode);
            Assert.AreEqual("Offline user", record.Username);
            Assert.AreEqual("", record.AvatarUrl);
            Assert.AreEqual(RankType.B, record.Rank);
            Assert.AreEqual(12345678, record.Score);
            Assert.AreEqual(1000, record.MaxCombo);
            Assert.AreEqual(0.55f, record.Accuracy, Delta);
            Assert.AreEqual(3, record.Judgements.Count);

            Assert.AreEqual(0, record.Judgements[0].Combo);
            Assert.AreEqual(1, record.Judgements[0].HitOffset);
            Assert.AreEqual(HitResultType.Perfect, record.Judgements[0].Result);
            Assert.AreEqual(true, record.Judgements[0].IsHit);
            Assert.AreEqual(1, record.Judgements[1].Combo);
            Assert.AreEqual(2, record.Judgements[1].HitOffset);
            Assert.AreEqual(HitResultType.Great, record.Judgements[1].Result);
            Assert.AreEqual(true, record.Judgements[1].IsHit);
            Assert.AreEqual(2, record.Judgements[2].Combo);
            Assert.AreEqual(5, record.Judgements[2].HitOffset);
            Assert.AreEqual(HitResultType.Miss, record.Judgements[2].Result);
            Assert.AreEqual(false, record.Judgements[2].IsHit);

            Assert.IsFalse(record.HitResultCounts.ContainsKey(HitResultType.Good));
            Assert.IsFalse(record.HitResultCounts.ContainsKey(HitResultType.Bad));
            Assert.IsFalse(record.HitResultCounts.ContainsKey(HitResultType.None));
            Assert.IsFalse(record.HitResultCounts.ContainsKey(HitResultType.Ok));
            Assert.AreEqual(1, record.HitResultCounts[HitResultType.Perfect]);
            Assert.AreEqual(1, record.HitResultCounts[HitResultType.Great]);
            Assert.AreEqual(1, record.HitResultCounts[HitResultType.Miss]);
            Assert.AreEqual(2, record.HitCount);
            Assert.AreEqual(100, record.Time);
            Assert.AreEqual((1f + 2f + 5f) / 3f, record.AverageOffset, Delta);
            Assert.IsTrue(record.Date >= curDate);
            Assert.IsTrue(record.IsClear);
        }

        private class DummyMap : IPlayableMap
        {
            public MapDetail Detail { get; set; } = new MapDetail()
            {
                Hash = "0x0",
            };

            public GameModeType PlayableMode { get; set; } = GameModeType.BeatsStandard;

            IOriginalMap IPlayableMap.OriginalMap { get; }
            DifficultyInfo IPlayableMap.Difficulty { get; set; }
            MapMetadata IMap.Metadata { get; }
            ControlPointGroup IMap.ControlPoints { get; }
            List<BreakPoint> IMap.BreakPoints { get; }
            IEnumerable<BaseHitObject> IMap.HitObjects { get; }
            int IMap.ObjectCount { get; }
            int IMap.Duration { get; }
            float IMap.BreakDuration { get; }
            bool IMap.IsPlayable { get; }
            List<Color> IComboColorable.ComboColors { get; }
            IEnumerable<string> IQueryableData.GetQueryables()
            {
                yield break;
            }
        }

        private class DummyScoreProcessor : IScoreProcessor
        {
            /// <summary>
            /// Event called when the last judgement has been made.
            /// </summary>
            public event Action OnLastJudgement;

            /// <summary>
            /// Event called when a new judgement has been made.
            /// </summary>
            public event Action<JudgementResult> OnNewJudgement;

            /// <summary>
            /// Returns the map which the score has been processed for.
            /// </summary>
            public IPlayableMap Map { get; }

            /// <summary>
            /// Returns the list of all judgements currently made.
            /// </summary>
            public List<JudgementResult> Judgements { get; set; }

            /// <summary>
            /// Returns the bindable combo value.
            /// </summary>
            public BindableInt Combo { get; }

            /// <summary>
            /// Returns the bindable highest combo value.
            /// </summary>
            public BindableInt HighestCombo { get; set; }

            /// <summary>
            /// Returns the bindable highest score value.
            /// </summary>
            public BindableInt Score { get; set; }

            /// <summary>
            /// Returns the bindable health value.
            /// </summary>
            public BindableFloat Health { get; }

            /// <summary>
            /// Returns the bindable accuracy value.
            /// </summary>
            public BindableFloat Accuracy { get; set; }

            /// <summary>
            /// Returns the bindable rank value.
            /// </summary>
            public Bindable<RankType> Ranking { get; set; }

            /// <summary>
            /// Returns whether all judgements have been made on all hit objects.
            /// </summary>
            public bool IsFinished { get; }

            /// <summary>
            /// Returns the whether the player has failed to achieve the success criteria.
            /// </summary>
            public bool IsFailed { get; }

            /// <summary>
            /// Returns the current number of judgements made.
            /// </summary>
            public int JudgeCount { get; }


            public float GetRankAccuracy(RankType type) => 0f;

            /// <summary>
            /// Apply any changes to the score processing from specified map.
            /// </summary>
            public void ApplyMap(IPlayableMap map) { }

            /// <summary>
            /// Processes the specified judgement for scoring.
            /// </summary>
            public void ProcessJudgement(JudgementResult result) { }

            public RankType CalculateRank(float acc) => RankType.D;

            public void Dispose() { }
        }
    }
}