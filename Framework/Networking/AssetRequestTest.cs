using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Networking.Tests
{
    public class AssetRequestTest {
        
        [Test]
        public void TestUrl()
        {
            var request = new DummyRequest("asdf");
            Assert.AreEqual("file:///asdf", request.Url);

            request = new DummyRequest("file://asd");
            Assert.AreEqual("file:///asd", request.Url);

            request = new DummyRequest("http://asdfg");
            Assert.AreEqual("http://asdfg", request.Url);

            request = new DummyRequest("HTtpS://bbb");
            Assert.AreEqual("HTtpS://bbb", request.Url);
        }

        private class DummyRequest : AssetRequest<GameObject>
        {
            public DummyRequest(string url) : base(url)
            {
            }
        }
    }
}