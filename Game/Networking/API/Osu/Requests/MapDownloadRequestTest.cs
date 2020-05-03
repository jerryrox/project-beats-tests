using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Stores;
using PBGame.Networking.API.Osu.Tests;
using PBGame.Networking.API.Osu.Responses;
using PBGame.Networking.API.Responses;
using PBGame.Networking.Maps;

namespace PBGame.Networking.API.Osu.Requests.Tests
{
    public class MapDownloadRequestTest {

        [UnityTest]
        public IEnumerator TestNoLogin()
        {
            var api = new OsuApi();

            var request = new MapDownloadRequest()
            {
                DownloadStore = GetStore(),
                Mapset = GetMapset()
            };
            IMapDownloadResponse response = null;
            request.OnRequestEnd += (r) => response = r;
            api.Request(request);

            while (response == null)
            {
                yield return null;
            }

            Assert.IsFalse(response.IsSuccess);
            Debug.Log($"MapDownloadRequestTest.TestNoLogin - Error: {response.ErrorMessage}");
        }

        [UnityTest]
        public IEnumerator Test()
        {
            var api = new OsuApi();

            yield return WaitLogin(api);

            int mapsetId = 61080;
            string fileName = $"{mapsetId}.osz";

            var store = GetStore();
            store.MapStorage.DeleteAll();
            Assert.IsFalse(store.MapStorage.Exists(fileName));

            var request = new MapDownloadRequest()
            {
                DownloadStore = store,
                Mapset = GetMapset()
            };
            IMapDownloadResponse response = null;
            request.OnRequestEnd += (r) => response = r;
            api.Request(request);

            while (response == null)
            {
                Debug.Log($"Progress: {request.Promise.Progress}");
                yield return null;
            }

            Assert.IsTrue(store.MapStorage.Exists(fileName));
        }

        private IEnumerator WaitLogin(IApi api)
        {
            var loginRequest = new LoginRequest()
            {
                Username = OsuCredentials.Username,
                Password = OsuCredentials.Password
            };
            api.Request(loginRequest);

            while(!loginRequest.Promise.IsFinished) yield return null;

            Assert.IsTrue(api.IsOnline.Value);
        }

        /// <summary>
        /// Creates a new dummy store for testing.
        /// </summary>
        private IDownloadStore GetStore() => new DummyDownloadStore();

        private OnlineMapset GetMapset()
        {
            return new OnlineMapset()
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
                Status = MapStateType.Ranked,
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
                        Mode = Rulesets.GameModeType.OsuStandard,
                        SliderCount = 474,
                        SpinnerCount = 4,
                        TotalCount = 1372,
                        TotalDuration = 344,
                        Version = "Fate"
                    }
                }
            };
        }
    }
}