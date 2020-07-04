using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Networking.Linking.Tests
{
    public class DeepLinkerTest {
        
        [Test]
        public void TestLink()
        {
            DeepLinker.HookHandler handler = new DeepLinker.HookHandler((link) => {});

            DeepLinker linker = new DeepLinker();
            linker.LinkPath("api/", handler);
            Assert.IsTrue(linker.IsLinked("api"));
            Assert.IsTrue(linker.IsLinked("api/"));
            Assert.IsTrue(linker.IsLinked("/api"));

            linker = new DeepLinker();
            linker.LinkPath("/api", handler);
            Assert.IsTrue(linker.IsLinked("api"));
            Assert.IsTrue(linker.IsLinked("api/"));
            Assert.IsTrue(linker.IsLinked("/api"));

            linker = new DeepLinker();
            linker.LinkPath(null, handler);
            Assert.IsFalse(linker.IsLinked("api"));

            linker = new DeepLinker();
            linker.LinkPath("api", null);
            Assert.IsFalse(linker.IsLinked("api"));

            linker = new DeepLinker();
            linker.LinkPath("api?ab=ba", handler);
            Assert.IsTrue(linker.IsLinked("api"));

            linker = new DeepLinker();
            linker.LinkPath("api/?ab=ba", handler);
            Assert.IsTrue(linker.IsLinked("api"));

            linker = new DeepLinker();
            linker.LinkPath("pbtest://api", handler);
            Assert.IsTrue(linker.IsLinked("api"));
        }

        [Test]
        public void TestEmit()
        {
            int calledCount = 0;

            DeepLinker linker = new DeepLinker();
            linker.LinkPath("api", (link) =>
            {
                calledCount++;
                Assert.AreEqual("api", link.Path);
                Assert.AreEqual(0, link.Parameters.Count);
            });
            linker.Emit("api");
            Assert.AreEqual(1, calledCount);
            linker.Emit("api/a");
            linker.Emit("api/s");
            linker.Emit("a/api");
            linker.Emit("nb/api");
            Assert.AreEqual(1, calledCount);

            linker = new DeepLinker();
            linker.LinkPath("api/lolz/", (link) =>
            {
                calledCount++;
                Assert.AreEqual("api/lolz", link.Path);
                Assert.AreEqual(0, link.Parameters.Count);
            });
            linker.Emit("api/lolz");
            Assert.AreEqual(2, calledCount);

            linker = new DeepLinker();
            linker.LinkPath("api", (link) =>
            {
                calledCount++;
                Assert.AreEqual("api", link.Path);
                Assert.AreEqual(1, link.Parameters.Count);
                Assert.IsTrue(link.Parameters.ContainsKey("asdf"));
                Assert.AreEqual("fdsa", link.Parameters["asdf"]);
            });
            linker.Emit("api?asdf=fdsa");
            Assert.AreEqual(3, calledCount);

            linker = new DeepLinker();
            linker.LinkPath("api", (link) =>
            {
                calledCount++;
                Assert.AreEqual("api", link.Path);
                Assert.AreEqual(1, link.Parameters.Count);
                Assert.IsTrue(link.Parameters.ContainsKey("asd[f]"));
                Assert.AreEqual("fdsa", link.Parameters["asd[f]"]);
            });
            linker.Emit("api?asd%5Bf%5D=fdsa");
            Assert.AreEqual(4, calledCount);
        }
    }
}