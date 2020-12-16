using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.IO;
using PBGame.Data.Users;
using PBGame.Data.Records;
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
using PBFramework.Threading;

namespace PBGame.Stores.Tests
{
    public class RecordStoreTest {

        private const string UserId = "00000000-0000-0000-0000-000000000001";
        private const float Delta = 0.001f;


        [Test]
        public void TestSave()
        {
            var recordStore = new RecordStore();
            List<DummyMap> maps = new List<DummyMap>()
            {
                new DummyMap()
                {
                    Detail = new MapDetail()
                    {
                        Hash = "0001",
                    },
                    PlayableMode = GameModeType.BeatsStandard,
                },
            };

            Initialize(recordStore, maps);

            var curDate = DateTime.Now;
            recordStore.SaveRecord(CreateRecord(
                maps[0], 0.9f, RankType.A, 100, 1234,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                }
            ));
            var listener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(maps[0], listener: listener).Wait();
            Assert.AreEqual(1, listener.Value.Count);
            var record = listener.Value[0];
            Assert.AreEqual(0.9f, record.Accuracy, Delta);
            Assert.AreEqual((0 + 1 + 1 + 1 + 1) / 5f, record.AverageOffset, Delta);
            Assert.IsTrue(record.Date >= curDate);
            Assert.AreEqual(maps[0].PlayableMode, record.GameMode);
            Assert.AreEqual(5, record.HitCount);
            Assert.AreEqual(2, record.HitResultCounts[HitResultType.Perfect]);
            Assert.AreEqual(1, record.HitResultCounts[HitResultType.Great]);
            Assert.AreEqual(2, record.HitResultCounts[HitResultType.Good]);
            Assert.IsFalse(record.HitResultCounts.ContainsKey(HitResultType.Ok));
            Assert.IsFalse(record.HitResultCounts.ContainsKey(HitResultType.Bad));
            Assert.IsFalse(record.HitResultCounts.ContainsKey(HitResultType.Miss));
            Assert.IsFalse(record.HitResultCounts.ContainsKey(HitResultType.None));
            Assert.AreEqual(true, record.IsClear);
            Assert.AreEqual(5, record.Judgements.Count);
            Assert.AreEqual(maps[0].Detail.Hash, record.MapHash);
            Assert.AreEqual(100, record.MaxCombo);
            Assert.AreEqual(RankType.A, record.Rank);
            Assert.AreEqual(1234, record.Score);
            Assert.AreEqual(100, record.Time);
            Assert.AreEqual(UserId, record.UserId.ToString());
            Assert.AreEqual("Offline user", record.Username);
            Assert.AreEqual("", record.AvatarUrl);
        }

        [Test]
        public void TestSaveSameMultiple()
        {
            var recordStore = new RecordStore();
            List<DummyMap> maps = new List<DummyMap>()
            {
                new DummyMap()
                {
                    Detail = new MapDetail()
                    {
                        Hash = "0001",
                    },
                    PlayableMode = GameModeType.BeatsStandard,
                },
            };

            Initialize(recordStore, maps);

            recordStore.SaveRecord(CreateRecord(
                maps[0], 0.9f, RankType.X, 99, 1234,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                }
            ));
            recordStore.SaveRecord(CreateRecord(
                maps[0], 0.95f, RankType.SH, 88, 4321,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                }
            ));
            recordStore.SaveRecord(CreateRecord(
                maps[0], 0.85f, RankType.S, 77, 321,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                }
            ));

            var listener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(maps[0], listener: listener).Wait();
            Assert.AreEqual(3, listener.Value.Count);
            Assert.AreEqual(3, recordStore.GetRecordCount(maps[0], CreateUser()));
            Assert.AreEqual(0, recordStore.GetRecordCount(maps[0], CreateUser(Guid.Empty.ToString())));

            var bestRecord = listener.Value[0];
            Assert.AreEqual(4321, bestRecord.Score);

            var limitedListener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(maps[0], limit: 1, listener: limitedListener).Wait();
            Assert.AreEqual(1, limitedListener.Value.Count);
            Assert.AreEqual(bestRecord.Score, limitedListener.Value[0].Score);

            recordStore.SaveRecord(CreateRecord(
                maps[0], 0.98f, RankType.SH, 88, 4321,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                }
            ));

            listener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(maps[0], listener: listener).Wait();
            Assert.AreEqual(4, listener.Value.Count);
            bestRecord = listener.Value[0];
            Assert.AreEqual(4321, bestRecord.Score);
            Assert.AreEqual(0.98f, bestRecord.Accuracy, Delta);
        }

        [Test]
        public void TestSaveDifferentMultiple()
        {
            var recordStore = new RecordStore();
            List<DummyMap> maps = new List<DummyMap>()
            {
                new DummyMap()
                {
                    Detail = new MapDetail()
                    {
                        Hash = "0001",
                    },
                    PlayableMode = GameModeType.BeatsStandard,
                },
                new DummyMap()
                {
                    Detail = new MapDetail()
                    {
                        Hash = "0002",
                    },
                    PlayableMode = GameModeType.BeatsStandard,
                },
                new DummyMap()
                {
                    Detail = new MapDetail()
                    {
                        Hash = "0003",
                    },
                    PlayableMode = GameModeType.BeatsStandard,
                },
            };

            Initialize(recordStore, maps);

            recordStore.SaveRecord(CreateRecord(
                maps[0], 0.9f, RankType.X, 99, 1234,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                }
            ));
            recordStore.SaveRecord(CreateRecord(
                maps[1], 0.95f, RankType.SH, 88, 4321,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                }
            ));
            recordStore.SaveRecord(CreateRecord(
                maps[1], 0.85f, RankType.S, 77, 321,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                }
            ));
            recordStore.SaveRecord(CreateRecord(
                maps[2], 0.85f, RankType.S, 77, 321,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                }
            ));

            var listener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(maps[0], listener: listener).Wait();
            Assert.AreEqual(1, listener.Value.Count);
            Assert.AreEqual(1, recordStore.GetRecordCount(maps[0], CreateUser()));

            listener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(maps[1], listener: listener).Wait();
            Assert.AreEqual(2, listener.Value.Count);
            Assert.AreEqual(2, recordStore.GetRecordCount(maps[1], CreateUser()));

            listener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(maps[2], listener: listener).Wait();
            Assert.AreEqual(1, listener.Value.Count);
            Assert.AreEqual(1, recordStore.GetRecordCount(maps[2], CreateUser()));
        }

        [Test]
        public void TestGetRecordsDifferentUser()
        {
            var recordStore = new RecordStore();
            DummyMap map = new DummyMap()
            {
                Detail = new MapDetail()
                {
                    Hash = "0001",
                },
                PlayableMode = GameModeType.BeatsStandard,
            };

            IUser userA = CreateUser("00000000-0000-0000-0000-000000000000");
            IUser userB = CreateUser("00000000-0000-0000-0000-000000000001");

            Initialize(recordStore, new List<DummyMap>()
            {
                map
            });

            recordStore.SaveRecord(CreateRecord(
                map, 0.9f, RankType.X, 99, 1234,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                },
                user: userA
            ));
            recordStore.SaveRecord(CreateRecord(
                map, 0.8f, RankType.A, 98, 123,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                },
                user: userA
            ));
            recordStore.SaveRecord(CreateRecord(
                map, 0.95f, RankType.SH, 88, 4321,
                new List<JudgementResult>()
                {
                    CreateJudgementResult(0, 0, HitResultType.Perfect),
                    CreateJudgementResult(1, 1, HitResultType.Perfect),
                    CreateJudgementResult(2, 1, HitResultType.Great),
                    CreateJudgementResult(3, 1, HitResultType.Good),
                    CreateJudgementResult(4, 1, HitResultType.Good),
                },
                user: userB
            ));

            var listener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(map, userA, listener: listener).Wait();
            Assert.AreEqual(2, listener.Value.Count);
            Assert.IsTrue(listener.Value.Where((r) => r.Accuracy == 0.9f && r.UserId == userA.Id).Count() == 1);
            Assert.IsTrue(listener.Value.Where((r) => r.Accuracy == 0.8f && r.UserId == userA.Id).Count() == 1);

            listener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(map, userB, listener: listener).Wait();
            Assert.AreEqual(1, listener.Value.Count);
            Assert.IsTrue(listener.Value.Where((r) => r.Accuracy == 0.95f && r.UserId == userB.Id).Count() == 1);

            var limitedListener = new TaskListener<List<IRecord>>();
            recordStore.GetTopRecords(map, userA, limit: 1, listener: limitedListener).Wait();
            Assert.AreEqual(1, limitedListener.Value.Count);
            Assert.AreEqual(1234, limitedListener.Value[0].Score);
        }

        private void Initialize(RecordStore manager, List<DummyMap> dummyMaps)
        {
            UnityThread.Initialize();
            GameDirectory.Records.Refresh();

            var reloadListener = new TaskListener();
            manager.Reload(reloadListener).Wait();
            Assert.IsTrue(reloadListener.IsFinished);

            // Remove trash data from last test.
            dummyMaps.ForEach(map =>
            {
                manager.DeleteRecords(map);

                var listener = new TaskListener<List<IRecord>>();
                manager.GetTopRecords(map, listener: listener).Wait();
                Assert.IsTrue(listener.IsFinished);
                Assert.IsNotNull(listener.Value);
                Assert.AreEqual(0, listener.Value.Count);
            });

            GameDirectory.Records.Refresh();
        }

        private Record CreateRecord(
            DummyMap map,
            float accuracy = 1, RankType ranking = RankType.SH, int highestCombo = 1,
            int score = 1000, List<JudgementResult> judgements = null, IUser user = null
        )
        {
            return new Record(
                map,
                user ?? CreateUser(),
                new DummyScoreProcessor()
                {
                    Accuracy = new BindableFloat(accuracy),
                    Ranking = new Bindable<RankType>(ranking),
                    HighestCombo = new BindableInt(highestCombo),
                    Score = new BindableInt(score),
                    Judgements = judgements,
                },
                100
            );
        }

        private IUser CreateUser(string overrideId = null)
        {
            return new User(new OfflineUser())
            {
                Id = new Guid(overrideId ?? UserId)
            };
        }

        private JudgementResult CreateJudgementResult(int combo, int offset, HitResultType hitResult)
        {
            return new JudgementResult(new JudgementInfo())
            {
                ComboAtJudgement = combo,
                HighestComboAtJudgement = combo,
                HitOffset = offset,
                HitResult = hitResult
            };
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
    }
}