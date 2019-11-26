using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Storages.Tests
{
    public class FileStorageTest {
        
        [Test]
        public void TestText()
        {
            var storage = CreateStorage();
            Assert.IsFalse(storage.Exists("test"));
            Assert.IsFalse(storage.GetFile("test").Exists);

            storage.Write("test", "trollol");
            Assert.IsTrue(storage.Exists("test"));
            Assert.IsTrue(storage.GetFile("test").Exists);
            Assert.AreEqual("trollol", storage.GetText("test"));
        }

        [Test]
        public void TestData()
        {
            var storage = CreateStorage();
            Assert.IsFalse(storage.Exists("test"));
            Assert.IsFalse(storage.GetFile("test").Exists);

            storage.Write("test", new byte[] { 0, 1, 2, 3 });
            Assert.IsTrue(storage.Exists("test"));
            Assert.IsTrue(storage.GetFile("test").Exists);
            var data = storage.GetData("test");
            Assert.AreEqual(4, data.Length);
            for (byte i = 0; i < 4; i++)
            {
                Assert.AreEqual(i, data[i]);
            }
        }

        [Test]
        public void TestDelete()
        {
            var storage = CreateStorage();
            for (int i = 0; i < 4; i++)
            {
                storage.Write($"test{i}", $"content{i}");
                Assert.IsTrue(storage.Exists($"test{i}"));
            }
            Assert.AreEqual(4, storage.Count);

            storage.Delete("test0");
            storage.Delete("test1");
            Assert.AreEqual(2, storage.Count);
            for (int i = 0; i < 4; i++)
            {
                if (i < 2)
                {
                    Assert.IsFalse(storage.Exists($"test{i}"));
                    Assert.IsNull(storage.GetText($"test{i}"));
                }
                else
                {
                    Assert.IsTrue(storage.Exists($"test{i}"));
                    Assert.AreEqual($"content{i}", storage.GetText($"test{i}"));
                }
            }

            storage.DeleteAll();
            Assert.AreEqual(0, storage.Count);
            for (int i = 0; i < 4; i++)
            {
                Assert.IsFalse(storage.Exists($"test{i}"));
                Assert.IsNull(storage.GetText($"test{i}"));
            }
        }

        private IFileStorage CreateStorage(bool clearFirst = true)
        {
            var storage = new FileStorage(GetDirectory());
            if (clearFirst)
            {
                storage.DeleteAll();
            }
            return storage;
        }

        private DirectoryInfo GetDirectory()
        {
            return new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "Storages/FileStorageTest"));
        }

    }
}