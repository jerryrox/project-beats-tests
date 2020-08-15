using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json.Linq;

namespace PBFramework.Storages.Tests
{
    public class JsonStorageTest {
        
        [Test]
        public void TestObject()
        {
            var obj = new JObject();
            obj["i"] = 1;
            obj["s"] = "a";

            var storage = CreateStorage();
            Assert.IsFalse(storage.Exists("test"));
            Assert.IsNull(storage.GetObject("test"));

            storage.Write("test", obj);
            Assert.IsTrue(storage.Exists("test"));

            obj = storage.GetObject("test");
            Assert.AreEqual(1, obj["i"].Value<int>());
            Assert.AreEqual("a", obj["s"].ToString());
        }

        [Test]
        public void TestArray()
        {
            var arr = new JArray();
            arr.Add(true);
            arr.Add("b");

            var storage = CreateStorage();
            Assert.IsFalse(storage.Exists("test"));
            Assert.IsNull(storage.GetArray("test"));

            storage.Write("test", arr);
            Assert.IsTrue(storage.Exists("test"));

            arr = storage.GetArray("test");
            Assert.AreEqual(true, arr[0].Value<bool>());
            Assert.AreEqual("b", arr[1].ToString());
        }

        [Test]
        public void TestText()
        {
            string testObj = "{ \"i\": 1, \"s\": \"a\" }";
            string testArr = "[ true, \"b\" ]";

            var storage = CreateStorage();
            Assert.IsFalse(storage.Exists("testObj"));
            Assert.IsFalse(storage.Exists("testArr"));
            Assert.IsNull(storage.GetObject("testObj"));
            Assert.IsNull(storage.GetArray("testArr"));

            storage.Write("testObj", testObj);
            storage.Write("testArr", testArr);
            try
            {
                storage.Write("testError", "{[34y34[eg]g");
                Assert.Fail("This should've failed!");
            }
            catch (Exception) { }

            Assert.IsTrue(storage.Exists("testObj"));
            Assert.IsTrue(storage.Exists("testArr"));

            var obj = storage.GetObject("testObj");
            var arr = storage.GetArray("testArr");
            Assert.IsNotNull(obj);
            Assert.IsNotNull(arr);
            Assert.AreEqual(1, obj["i"].Value<int>());
            Assert.AreEqual("a", obj["s"].ToString());
            Assert.AreEqual(true, arr[0].Value<bool>());
            Assert.AreEqual("b", arr[1].ToString());
        }

        [Test]
        public void TestData()
        {
            byte[] testObj = System.Text.Encoding.UTF8.GetBytes("{ \"i\": 1, \"s\": \"a\" }");
            byte[] testArr = System.Text.Encoding.UTF8.GetBytes("[ true, \"b\" ]");

            var storage = CreateStorage();
            Assert.IsFalse(storage.Exists("testObj"));
            Assert.IsFalse(storage.Exists("testArr"));
            Assert.IsNull(storage.GetObject("testObj"));
            Assert.IsNull(storage.GetArray("testArr"));

            storage.Write("testObj", testObj);
            storage.Write("testArr", testArr);
            try
            {
                storage.Write("testError", System.Text.Encoding.UTF8.GetBytes("{[34y34[eg]g"));
                Assert.Fail("This should've failed!");
            }
            catch (Exception) { }

            Assert.IsTrue(storage.Exists("testObj"));
            Assert.IsTrue(storage.Exists("testArr"));

            var obj = storage.GetObject("testObj");
            var arr = storage.GetArray("testArr");
            Assert.IsNotNull(obj);
            Assert.IsNotNull(arr);
            Assert.AreEqual(1, obj["i"].Value<int>());
            Assert.AreEqual("a", obj["s"].ToString());
            Assert.AreEqual(true, arr[0].Value<bool>());
            Assert.AreEqual("b", arr[1].ToString());
        }



        private DirectoryInfo GetDirectory()
        {
            return new DirectoryInfo(Path.Combine(TestConstants.TestAssetPath, "Storages/JsonStorageTest"));
        }

        private IJsonStorage CreateStorage(bool clearFirst = true)
        {
            var storage = new JsonStorage(GetDirectory());
            if(clearFirst)
                storage.DeleteAll();
            return storage;
        }
    }
}