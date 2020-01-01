using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Storages.Tests
{
    public class PrefStorageTest {

        private const string PrefKey = "PrefStorageTest";


        [Test]
        public void TestInstantiate()
        {
            var storage = CreateStorage();
            Assert.IsFalse(PlayerPrefs.HasKey(PrefKey));
            Assert.AreEqual(0, storage.Count);

            storage.Save();
            Assert.IsTrue(PlayerPrefs.HasKey(PrefKey));
        }

        [Test]
        public void TestGetSet()
        {
            var storage = CreateStorage();

            storage.SetString("str", "asdf");
            Assert.AreEqual("asdf", storage.GetString("str"));

            storage.SetInt("i", 50);
            Assert.AreEqual(50, storage.GetInt("i"));

            storage.SetFloat("f", 15.5f);
            Assert.AreEqual(15.5f, storage.GetFloat("f"), 0.00000001f);

            storage.SetBool("b", true);
            Assert.AreEqual(true, storage.GetBool("b"));

            storage.SetObject("obj", new Dummy());
            Assert.AreEqual("dummy", storage.GetObject<Dummy>("obj").A);
            Assert.AreEqual(10, storage.GetObject<Dummy>("obj").B);

            storage.SetEnum("enum", DummyEnum.FDSA);
            Assert.AreEqual(DummyEnum.FDSA, storage.GetEnum<DummyEnum>("enum"));
        }

        [Test]
        public void TestExists()
        {
            var storage = CreateStorage();
            Assert.IsFalse(storage.Exists("lolB"));

            storage.SetBool("lolB", true);
            Assert.IsTrue(storage.Exists("lolB"));

            storage.Save();
            storage = CreateStorage(false);
            Assert.IsTrue(storage.Exists("lolB"));
        }

        [Test]
        public void TestDelete()
        {
            var storage = CreateStorage();

            for (int i = 0; i < 4; i++)
            {
                storage.SetInt($"i{i}", i);
                Assert.AreEqual(i, storage.GetInt($"i{i}"));
            }

            storage.Delete("i0");
            storage.Delete("i3");
            for (int i = 0; i < 4; i++)
            {
                if (i == 0 || i == 3)
                {
                    Assert.IsFalse(storage.Exists($"i{i}"));
                    Assert.AreEqual(-1, storage.GetInt($"i{i}", -1));
                }
                else
                {
                    Assert.IsTrue(storage.Exists($"i{i}"));
                    Assert.AreEqual(i, storage.GetInt($"i{i}"));
                }
            }

            storage.DeleteAll();
            for (int i = 0; i < 4; i++)
            {
                Assert.IsFalse(storage.Exists($"i{i}"));
                Assert.AreEqual(-1, storage.GetInt($"i{i}", -1));
            }
        }

        private PrefStorage CreateStorage(bool clearFirst = true)
        {
            if(clearFirst)
                PlayerPrefs.DeleteKey(PrefKey);
            return new PrefStorage(PrefKey);
        }


        private class Dummy
        {
            public string A = "dummy";
            public int B = 10;
        }

        private enum DummyEnum
        {
            Troll = 0,
            Lolz,
            ASDF,
            FDSA,
            LAZER
        }
    }
}