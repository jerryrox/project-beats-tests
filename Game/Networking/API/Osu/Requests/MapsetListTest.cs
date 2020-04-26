using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Networking.Maps;
using PBGame.Networking.API.Osu.Responses;
using PBGame.Networking.API.Responses;
using Newtonsoft.Json;

namespace PBGame.Networking.API.Osu.Requests.Tests
{
    public class MapsetListTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var api = new OsuApi();

            var request = new MapsetListRequest();
            IMapsetListResponse response = null;
            request.OnRequestEnd += (r) => response = r;
            api.Request(request);

            while(response == null) yield return null;

            Assert.IsTrue(response.Mapsets.All(m => m != null));
        }

        [UnityTest]
        public IEnumerator TestSorts()
        {
            var api = new OsuApi();

            foreach (var sortType in (MapSortType[])Enum.GetValues(typeof(MapSortType)))
            {
                Debug.LogWarning("Requesting with sort type: " + sortType);
                var request = new MapsetListRequest()
                {
                    Sort = sortType,
                    IsDescending = true,
                };

                IMapsetListResponse response = null;
                request.OnRequestEnd += (r) => response = r;
                api.Request(request);

                while (response == null) yield return null;

                Assert.IsTrue(response.Mapsets.All(m => m != null));
            }
        }
    }
}