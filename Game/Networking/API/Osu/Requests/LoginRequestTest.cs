using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Networking.API.Osu.Tests;
using PBFramework.Networking;
using PBFramework.Networking.API;
using Newtonsoft.Json;

namespace PBGame.Networking.API.Osu.Requests.Tests
{
    public class LoginRequestTest {
        
        [UnityTest]
        public IEnumerator TestPlain()
        {
            var request = new HttpPostRequest("https://osu.ppy.sh/session");
            var form = new FormPostData();
            form.AddField("username", OsuCredentials.Username);
            form.AddField("password", OsuCredentials.Password);
            request.SetPostData(form);

            request.Request();
            while(!request.IsFinished) yield return null;

            var response = request.Response;
            Debug.Log($"isSuccess: {response.IsSuccess}");
            Debug.Log($"errorMessage: {response.ErrorMessage}");
            Debug.Log($"textData: {response.TextData}");
            Debug.Log($"headers: {JsonConvert.SerializeObject(response.Headers)}");
        }

        [UnityTest]
        public IEnumerator TestApi()
        {
            var api = new OsuApi();
            bool finished = false;
            bool isOnline = false;
            IOnlineUser user = api.User.Value;
            IOnlineUser offlineUser = user;

            api.IsOnline.OnValueChanged += (online, _) => isOnline = online;
            api.User.OnValueChanged += (u, _) => user = u;

            var request = new LoginRequest(OsuCredentials.Username, OsuCredentials.Password);
            request.OnRequestEnd += (_) => finished = true;
            api.Request(request);
            
            while(!finished) yield return null;

            var onlineUser = api.User.Value;
            Assert.IsTrue(isOnline);
            Assert.AreEqual(user, onlineUser);
            Assert.AreNotEqual(offlineUser, user);
            Assert.IsTrue(onlineUser is OsuUser);

            Debug.Log("Online user data: " + JsonConvert.SerializeObject(onlineUser));
        }
    }
}