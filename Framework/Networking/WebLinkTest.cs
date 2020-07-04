using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Networking.Tests
{
    public class WebLinkTest {

        [Test]
        public void TestParse()
        {
            WebLink webPath = new WebLink();
            Assert.AreEqual("", webPath.Scheme);
            Assert.AreEqual("", webPath.Path);
            Assert.AreEqual(0, webPath.Parameters.Count);

            webPath = new WebLink("pbgame");
            Assert.AreEqual("", webPath.Scheme);
            Assert.AreEqual("pbgame", webPath.Path);
            Assert.AreEqual(0, webPath.Parameters.Count);

            webPath = new WebLink("pbgame://api");
            Assert.AreEqual("pbgame", webPath.Scheme);
            Assert.AreEqual("api", webPath.Path);
            Assert.AreEqual(0, webPath.Parameters.Count);

            webPath = new WebLink("://///asdf");
            Assert.AreEqual("", webPath.Scheme);
            Assert.AreEqual("asdf", webPath.Path);
            Assert.AreEqual(0, webPath.Parameters.Count);

            webPath = new WebLink("api?");
            Assert.AreEqual("", webPath.Scheme);
            Assert.AreEqual("api", webPath.Path);
            Assert.AreEqual(0, webPath.Parameters.Count);

            webPath = new WebLink("api?=&=&=&");
            Assert.AreEqual("", webPath.Scheme);
            Assert.AreEqual("api", webPath.Path);
            Assert.AreEqual(0, webPath.Parameters.Count);

            webPath = new WebLink("api?a=b&c=");
            Assert.AreEqual("", webPath.Scheme);
            Assert.AreEqual("api", webPath.Path);
            Assert.AreEqual(2, webPath.Parameters.Count);
            Assert.AreEqual("b", webPath.Parameters["a"]);
            Assert.AreEqual("", webPath.Parameters["c"]);
        }

        [Test]
        public void TestSet()
        {
            WebLink webPath = new WebLink();
            webPath.SetScheme("lol://");
            Assert.AreEqual("lol", webPath.Scheme);
            webPath.SetScheme("asdf");
            Assert.AreEqual("asdf", webPath.Scheme);
            // At this point, the developer is intentionally trolling so we don't care about parsing it.
            webPath.SetScheme("asdf?:ab");
            Assert.AreEqual("asdf?:ab", webPath.Scheme);
            
            Assert.AreEqual("asdf?:ab://", webPath.Url);

            webPath.SetPath("a");
            Assert.AreEqual("a", webPath.Path);
            webPath.SetPath("a/b/c");
            Assert.AreEqual("a/b/c", webPath.Path);
            webPath.SetPath("asdf://a/b/c");
            Assert.AreEqual("a/b/c", webPath.Path);
            webPath.SetPath("a/b/");
            Assert.AreEqual("a/b", webPath.Path);
            webPath.SetPath("a/b?d=3");
            Assert.AreEqual("a/b", webPath.Path);

            Assert.AreEqual("asdf?:ab://a/b", webPath.Url);

            webPath.SetParam("asdf", "fdsa");
            Assert.AreEqual(1, webPath.Parameters.Count);
            Assert.AreEqual("fdsa", webPath.Parameters["asdf"]);
            webPath.SetParam("as[a]", "asd");
            Assert.AreEqual(2, webPath.Parameters.Count);
            Assert.AreEqual("asd", webPath.Parameters["as[a]"]);
            webPath.SetParam("asdf", "ffddssaa");
            Assert.AreEqual(2, webPath.Parameters.Count);
            Assert.AreEqual("ffddssaa", webPath.Parameters["asdf"]);

            Assert.AreEqual("asdf?:ab://a/b?asdf=ffddssaa&as%5ba%5d=asd", webPath.Url);
        }
    }
}