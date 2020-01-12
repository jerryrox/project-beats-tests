using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Networking.Tests
{
    public class CookieContainerTest {
        
        [Test]
        public void Test()
        {
            var container = new CookieContainer();
            container.SetCookie("__cfduid=ddb92f20371065d679c1530ea63bb4c671578798643; expires=Tue, 11-Feb-20 03:10:43 GMT; path=/; domain=.ppy.sh; HttpOnly; SameSite=Lax,osu_session=eyJpdiI6IkhtSnE1UHhwQXN4NStYZVJoZEVQS3c9PSIsInZhbHVlIjoiVXM0cmx6RW02d3BIWU9hMXRpeXBxMHIrUWx3XC9jM3FZbUtpdVJkMUdOMnVtNGIzOXdnTHhWYW14VEU3UVp5UmJXSEJWVHZyVkdERVFjbW9cL3VQWUhcL3c9PSIsIm1hYyI6ImQxZGY1NTQzOTBkYjU3ZjYyYjg3NWQzNmI4YjY4YzFjMTg5ZWUxNDc2Zjg4OGZkMjVlNDEzODVjOGNjYmRmMTkifQ%3D%3D; expires=Tue, 11-Feb-2020 03:10:44 GMT; Max-Age=2592000; path=/; domain=.ppy.sh; httponly,locale=deleted; expires=Sat, 12-Jan-2019 03:10:43 GMT; Max-Age=0; path=/; domain=osu.ppy.sh; httponly");

            Assert.IsTrue(container.HasName("__cfduid"));
            Assert.IsTrue(container.HasName("osu_session"));
            Assert.IsTrue(container.HasName("locale"));

            var cfduid = container["__cfduid"];
            var osusession = container["osu_session"];
            var locale = container["locale"];

            Assert.AreEqual("__cfduid", cfduid.Name);
            Assert.AreEqual("ddb92f20371065d679c1530ea63bb4c671578798643", cfduid.Value);
            Assert.AreEqual("/", cfduid.Path);
            Assert.AreEqual(".ppy.sh", cfduid.Domain);
            Assert.IsTrue(cfduid.HttpOnly);

            Assert.AreEqual("osu_session", osusession.Name);
            Assert.AreEqual("eyJpdiI6IkhtSnE1UHhwQXN4NStYZVJoZEVQS3c9PSIsInZhbHVlIjoiVXM0cmx6RW02d3BIWU9hMXRpeXBxMHIrUWx3XC9jM3FZbUtpdVJkMUdOMnVtNGIzOXdnTHhWYW14VEU3UVp5UmJXSEJWVHZyVkdERVFjbW9cL3VQWUhcL3c9PSIsIm1hYyI6ImQxZGY1NTQzOTBkYjU3ZjYyYjg3NWQzNmI4YjY4YzFjMTg5ZWUxNDc2Zjg4OGZkMjVlNDEzODVjOGNjYmRmMTkifQ%3D%3D", osusession.Value);
            Assert.AreEqual("/", osusession.Path);
            Assert.AreEqual(".ppy.sh", osusession.Domain);
            Assert.IsTrue(osusession.HttpOnly);

            Assert.AreEqual("locale", locale.Name);
            Assert.AreEqual("deleted", locale.Value);
            Assert.AreEqual("/", locale.Path);
            Assert.AreEqual("osu.ppy.sh", locale.Domain);
            Assert.IsTrue(locale.HttpOnly);

            Debug.Log(container.GetCookieString());
        }
    }
}