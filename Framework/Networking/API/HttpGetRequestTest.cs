using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Networking.API.Tests
{
    public class HttpGetRequestTest {
        
        [UnityTest]
        public IEnumerator TestPlain()
        {
            var request = new HttpGetRequest("https://osu.ppy.sh/beatmapsets/search");
            Assert.AreEqual("https://osu.ppy.sh/beatmapsets/search", request.Url);
            request.Request();

            while(!request.IsFinished) yield return null;

            Debug.Log($"TestPlain result:\n{request.Response.TextData}");
        }

        [UnityTest]
        public IEnumerator TestQueryParam()
        {
            var request = new HttpGetRequest("https://osu.ppy.sh/beatmapsets/search?cursor%5Bapproved_date%5D=1562342428000&cursor%5B_id%5D=942269");
            Assert.IsTrue("https://osu.ppy.sh/beatmapsets/search?cursor%5Bapproved_date%5D=1562342428000&cursor%5B_id%5D=942269".Equals(request.Url, StringComparison.OrdinalIgnoreCase));

            request = new HttpGetRequest("https://osu.ppy.sh/beatmapsets/search");
            request.AddQueryParam("cursor[approved_date]", "1562342428000");
            request.AddQueryParam("cursor[_id]", "942269");
            Assert.IsTrue("https://osu.ppy.sh/beatmapsets/search?cursor%5Bapproved_date%5D=1562342428000&cursor%5B_id%5D=942269".Equals(request.Url, StringComparison.OrdinalIgnoreCase));

            request.Request();
            while (!request.IsFinished) yield return null;

            Debug.Log($"TestQueryParam result:\n{request.Response.TextData}");
        }
    }
}