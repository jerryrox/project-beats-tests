using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Stores;
using PBGame.Networking.API.Osu.Tests;
using PBGame.Networking.API.Osu.Responses;
using PBFramework.Storages;

namespace PBGame.Networking.API.Osu.Requests.Tests
{
    public class MapDownloadRequestTest {

        [UnityTest]
        public IEnumerator TestNoLogin()
        {
            var api = new OsuApi();

            var request = new MapDownloadRequest(GetStore(), 61080);
            MapDownloadResponse response = null;
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

            var request = new MapDownloadRequest(store, mapsetId);
            MapDownloadResponse response = null;
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
            var loginRequest = new LoginRequest(OsuCredentials.Username, OsuCredentials.Password);
            api.Request(loginRequest);

            while(!loginRequest.Promise.IsFinished) yield return null;

            Assert.IsTrue(api.IsOnline.Value);
        }

        /// <summary>
        /// Creates a new dummy store for testing.
        /// </summary>
        private IDownloadStore GetStore() => new DummyDownloadStore();
    }
}