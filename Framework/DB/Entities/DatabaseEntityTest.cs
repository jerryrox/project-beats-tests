using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json.Linq;

namespace PBFramework.DB.Entities.Tests
{
    public class DatabaseEntityTest {

        [Test]
        public void TestSerialize()
        {
            Guid id = Guid.NewGuid();

            var entity = new TestEntity()
            {
                Id = id,
                Age = 10,
                FirstName = "Troll",
                LastName = "Lol",
            };
            Assert.AreEqual(id, entity.Id);
            Assert.AreEqual(10, entity.Age);
            Assert.AreEqual("Troll", entity.FirstName);
            Assert.AreEqual("Lol", entity.LastName);
            Assert.AreEqual(90, entity.YearsLeft);

            var json = entity.Serialize();
            Debug.Log(json.ToString());
            Assert.AreEqual(entity.Id.ToString(), json["Id"].ToString());
            Assert.AreEqual(entity.Age, json["Age"].Value<int>());
            Assert.AreEqual(entity.FirstName, json["Name"].ToString());
            Assert.AreEqual(entity.LastName, json["LastName"].ToString());
            Assert.IsFalse(json.ContainsKey("YearsLeft"));
        }

        [Test]
        public void TestSerializeIndex()
        {
            Guid id = Guid.NewGuid();

            var entity = new TestEntity()
            {
                Id = id,
                Age = 10,
                FirstName = "Troll",
                LastName = "Lol",
            };
            Assert.AreEqual(id, entity.Id);
            Assert.AreEqual(10, entity.Age);
            Assert.AreEqual("Troll", entity.FirstName);
            Assert.AreEqual("Lol", entity.LastName);
            Assert.AreEqual(90, entity.YearsLeft);

            var json = entity.SerializeIndex();
            Debug.Log(json.ToString());

            Assert.IsFalse(json.ContainsKey("YearsLeft"));
            Assert.IsFalse(json.ContainsKey("FirstName"));
            Assert.IsFalse(json.ContainsKey("LastName"));
            Assert.IsTrue(json.ContainsKey("Id"));
            Assert.IsTrue(json.ContainsKey("Age"));
            Assert.IsTrue(json.ContainsKey("Name"));

            Assert.AreEqual(entity.Id.ToString(), json["Id"].ToString());
            Assert.AreEqual(entity.Age, json["Age"].Value<int>());
            Assert.AreEqual(entity.FirstName, json["Name"].ToString());
        }
    }
}