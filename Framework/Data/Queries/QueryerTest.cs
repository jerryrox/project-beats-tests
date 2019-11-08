using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Data.Queries.Tests
{
    public class QueryerTest {
        
        [Test]
        public void TestQuery()
        {
            var objects = new List<Dummy> {
                new Dummy() { Name = "Lol", Age = 15 },
                new Dummy() { Name = "Lolz", Age = 18 },
                new Dummy() { Name = "AOL", Age = 20 },
                new Dummy() { Name = "lawMaS", Age = 22 },
                new Dummy() { Name = "LAERO", Age = 15 }
            };

            var queryer = new Queryer<Dummy>();

            // Null query
            var results = queryer.Query(objects, null).ToList();
            Assert.AreEqual(objects.Count, results.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
            }
            // Empty query
            results = queryer.Query(objects, "").ToList();
            Assert.AreEqual(objects.Count, results.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
            }
            // Space query
            results = queryer.Query(objects, "      ").ToList();
            Assert.AreEqual(objects.Count, results.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
            }

            results = queryer.Query(objects, "ol").ToList();
            Assert.AreEqual(3, results.Count);
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
                Debug.Log(results[i].ToString());
            }

            results = queryer.Query(objects, "15").ToList();
            Assert.AreEqual(2, results.Count);
            for (int i = 0; i < 2; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
                Debug.Log(results[i].ToString());
            }
        }

        [Test]
        public void TestSpecialHandler()
        {
            var objects = new List<Dummy> {
                new Dummy() { Name = "Lol", Age = 15 },
                new Dummy() { Name = "Lolz", Age = 18 },
                new Dummy() { Name = "AOL", Age = 20 },
                new Dummy() { Name = "lawMaS", Age = 22 },
                new Dummy() { Name = "LAERO", Age = 15 }
            };

            var queryer = new Queryer<Dummy>();
            queryer.SetSpecialHandler("Name:", (list, token) => {
                return list.Where(item => item.Name.StartsWith(token, StringComparison.OrdinalIgnoreCase));
            });

            // Null query
            var results = queryer.Query(objects, null).ToList();
            Assert.AreEqual(objects.Count, results.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
            }
            // Empty query
            results = queryer.Query(objects, "").ToList();
            Assert.AreEqual(objects.Count, results.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
            }
            // Space query
            results = queryer.Query(objects, "      ").ToList();
            Assert.AreEqual(objects.Count, results.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
            }


            results = queryer.Query(objects, "name: l").ToList();
            Assert.AreEqual(4, results.Count);
            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
                Debug.Log(results[i].ToString());
            }

            results = queryer.Query(objects, "name:lo").ToList();
            Assert.AreEqual(2, results.Count);
            for (int i = 0; i < 2; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
                Debug.Log(results[i].ToString());
            }

            results = queryer.Query(objects, "aol").ToList();
            Assert.AreEqual(1, results.Count);
            for (int i = 0; i < 1; i++)
            {
                Assert.IsTrue(objects.Contains(results[i]));
                Debug.Log(results[i].ToString());
            }
        }


        private class Dummy : IQueryableData
        {
            public string Name { get; set; }
            public int Age { get; set; }

            public IEnumerable<string> GetQueryables()
            {
                yield return Name;
                yield return Age.ToString();
            }

            public override string ToString() => $"Name: {Name}, Age: {Age}";
        }
    }
}