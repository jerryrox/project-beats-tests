using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBGame.Rulesets;
using PBGame.Networking.Maps;
using PBFramework.UI;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Download.Tests
{
    public class ResultListTest {

        private DownloadState state;
        private ResultList list;


        [ReceivesDependency]
        private IRootMain RootMain { get; set; }
        
        
        [UnityTest]
        public IEnumerator Test()
        {
            yield return TestGame.Run(
                this,
                () => Init(),
                Update
            );
        }
        
        private IEnumerator Init()
        {
            state = new DownloadState();
            RootMain.Dependencies.Cache(state);

            list = RootMain.CreateChild<ResultList>("list", 0);
            {
                list.Size = new Vector2(1280f, 500f);
            }

            yield break;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Setting test results");
                SetTestResults();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("Setting empty result");
                SetEmptyResults();
            }
        }

        private void SetTestResults()
        {
            state.ModifyResults(results =>
            {
                results.Add(new OnlineMapset()
                {
                    Artist = "HoneyWorks meets YURiCa/Hanatan",
                    CoverImage = "https://assets.ppy.sh/beatmaps/1102807/covers/cover.jpg?1586745143",
                    CardImage = "https://assets.ppy.sh/beatmaps/1102807/covers/card.jpg?1586745143",
                    Creator = "William K",
                    FavoriteCount = 1,
                    Id = 1102807,
                    PlayCount = 198,
                    PreviewAudio = "https://b.ppy.sh/preview/1102807.mp3",
                    Source = "ずっと前から好きでした。～告白実行委員会～",
                    Status = MapStatus.Ranked,
                    Title = "Destiny ~Zutto Mae kara Kimi ga Suki Deshita~",
                    HasVideo = false,
                    HasStoryboard = false,
                    Bpm = 81,
                    DisabledInformation = null,
                    IsDisabled = false,
                    LastUpdate = DateTime.Parse("2020-04-13T02:32:02+00:00"),
                    Tags = "kokuhaku jikkou iinkai i've ive always liked you confession executive confess your love committee insert song",
                    Maps = new List<OnlineMap>() {
                        new OnlineMap() {
                            Accuracy = 7,
                            AR = 8,
                            Bpm = 81,
                            CircleCount = 412,
                            CS = 4,
                            Difficulty = 4.21f,
                            Drain = 5,
                            HitDuration = 312,
                            Id = 2303786,
                            Mode = GameModes.OsuStandard,
                            SliderCount = 474,
                            SpinnerCount = 4,
                            TotalCount = 1372,
                            TotalDuration = 344,
                            Version = "Fate"
                        }
                    }
                });
                results.Add(new OnlineMapset()
                {
                    Artist = "Kokuchou feat. Chata",
                    CoverImage = "https://assets.ppy.sh/beatmaps/1125030/covers/cover.jpg?1587142402",
                    CardImage = "https://assets.ppy.sh/beatmaps/1125030/covers/card.jpg?1587142402",
                    Creator = "kanpakyin",
                    FavoriteCount = 13236,
                    Id = 1102837,
                    PlayCount = 622252,
                    PreviewAudio = "https://b.ppy.sh/preview/1125030.mp3",
                    Source = "東方星蓮船　～ Undefined Fantastic Object.",
                    Status = MapStatus.Approved,
                    Title = "Lucid Dream",
                    HasVideo = true,
                    HasStoryboard = true,
                    Bpm = 200,
                    DisabledInformation = null,
                    IsDisabled = false,
                    LastUpdate = DateTime.Parse("2020-04-13T02:32:02+00:00"),
                    Tags = "eastnewsound touhou 東方project 法界の火 seirensen c77 ens sacred factor idumin izumin hijiri byakuren fires of hokkai video game japanese comiket 77 doujin circle",
                    Maps = new List<OnlineMap>() {
                        new OnlineMap() {
                            Accuracy = 7,
                            AR = 8,
                            Bpm = 81,
                            CircleCount = 412,
                            CS = 4,
                            Difficulty = 4.21f,
                            Drain = 5,
                            HitDuration = 312,
                            Id = 2303786,
                            Mode = GameModes.OsuStandard,
                            SliderCount = 474,
                            SpinnerCount = 4,
                            TotalCount = 1372,
                            TotalDuration = 344,
                            Version = "Fate 1"
                        },
                        new OnlineMap() {
                            Accuracy = 7,
                            AR = 8,
                            Bpm = 81,
                            CircleCount = 412,
                            CS = 4,
                            Difficulty = 4.21f,
                            Drain = 5,
                            HitDuration = 312,
                            Id = 2303786,
                            Mode = GameModes.OsuStandard,
                            SliderCount = 474,
                            SpinnerCount = 4,
                            TotalCount = 1372,
                            TotalDuration = 344,
                            Version = "Fate 2"
                        },
                        new OnlineMap() {
                            Accuracy = 7,
                            AR = 8,
                            Bpm = 81,
                            CircleCount = 412,
                            CS = 4,
                            Difficulty = 4.21f,
                            Drain = 5,
                            HitDuration = 312,
                            Id = 2303786,
                            Mode = GameModes.OsuStandard,
                            SliderCount = 474,
                            SpinnerCount = 4,
                            TotalCount = 1372,
                            TotalDuration = 344,
                            Version = "Fate 3"
                        },
                    }
                });
            });
        }

        private void SetEmptyResults()
        {
            state.ModifyResults(results => results.Clear());
        }
    }
}