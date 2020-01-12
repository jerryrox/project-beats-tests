using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Networking.Tests
{
    public class CookieTest {
        
        [Test]
        public void TestParse()
        {
            Cookie cookie = Cookie.Parse("__cfduid=ddb92f20371065d679c1530ea63bb4c671578798643; expires=Tue, 11-Feb-20 03:10:43 GMT; path=/; domain=.ppy.sh; HttpOnly; SameSite=Lax");
            Assert.AreEqual("__cfduid", cookie.Name);
            Assert.AreEqual("ddb92f20371065d679c1530ea63bb4c671578798643", cookie.Value);
            Debug.Log(cookie.Expires);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(".ppy.sh", cookie.Domain);
            Assert.IsTrue(cookie.HttpOnly);
        }
    }
}