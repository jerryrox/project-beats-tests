using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBFramework.DB.Entities.Tests;

namespace PBFramework.DB.Tests
{
    public class DatabaseIndexTest {
        
        [Test]
        public void TestRaw()
        {
            var index = new DatabaseIndex<TestEntity>(CreateList());
            int i = 0;
            foreach (var r in index.Raw)
            {
                AssertObject(r, i);
                i++;
            }
        }

        [Test]
        public void TestGetAll()
        {
            var index = new DatabaseIndex<TestEntity>(CreateList());
            var list = index.GetAll();
            Assert.AreEqual(5, list.Count);
            for (int i = 0; i < 5; i++)
            {
                AssertObject(list[i], i);
            }
        }

        [Test]
        public void TestSet()
        {
            var index = new DatabaseIndex<TestEntity>(CreateList());
            index.Set(CreateObject(5));
            var list = index.GetAll();
            Assert.AreEqual(6, list.Count);
            for (int i = 0; i < 6; i++)
            {
                AssertObject(list[i], i);
            }
        }

        [Test]
        public void TestSetReplace()
        {
            var index = new DatabaseIndex<TestEntity>(CreateList());
            index.Set(CreateObject(0));
            var list = index.GetAll();
            Assert.AreEqual(5, list.Count);
            for (int i = 0; i < 5; i++)
            {
                AssertObject(list[i], i);
            }
        }

        [Test]
        public void TestRemoveKey()
        {
            var index = new DatabaseIndex<TestEntity>(CreateList());
            index.Remove("00000000-0000-0000-0000-000000000004");
            var list = index.GetAll();
            Assert.AreEqual(4, list.Count);
            for (int i = 0; i < 4; i++)
            {
                AssertObject(list[i], i);
            }
        }

        [Test]
        public void TestRemoveObject()
        {
            var index = new DatabaseIndex<TestEntity>(CreateList());
            index.Remove(CreateObject(0));
            var list = index.GetAll();
            Assert.AreEqual(4, list.Count);
            for (int i = 0; i < 4; i++)
            {
                AssertObject(list[i], i+1);
            }
        }

        private void AssertObject(JObject obj, int index)
        {
            Assert.AreEqual($"00000000-0000-0000-0000-00000000000{index}", obj["Id"].ToString());
            Assert.AreEqual(index, obj["Age"].Value<int>());
            Assert.AreEqual($"FN{index}", obj["Name"].ToString());
        }

        private JObject CreateObject(int index)
        {
            JObject json = new JObject();
            json["Id"] = $"00000000-0000-0000-0000-00000000000{index}";
            json["Age"] = index;
            json["Name"] = $"FN{index}";
            return json;
        }

        private List<JObject> CreateList()
        {
            return JsonConvert.DeserializeObject<List<JObject>>(@"
            [
                {
                    'Id' : '00000000-0000-0000-0000-000000000000',
                    'Age' : 0,
                    'Name' : 'FN0',
                },
                {
                    'Id' : '00000000-0000-0000-0000-000000000001',
                    'Age' : 1,
                    'Name' : 'FN1',
                },
                {
                    'Id' : '00000000-0000-0000-0000-000000000002',
                    'Age' : 2,
                    'Name' : 'FN2',
                },
                {
                    'Id' : '00000000-0000-0000-0000-000000000003',
                    'Age' : 3,
                    'Name' : 'FN3',
                },
                {
                    'Id' : '00000000-0000-0000-0000-000000000004',
                    'Age' : 4,
                    'Name' : 'FN4',
                }
            ]
            ");
        }
    }
}