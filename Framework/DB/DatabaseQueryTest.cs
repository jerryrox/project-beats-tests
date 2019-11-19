using System;
using System.IO;
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
    public class DatabaseQueryTest {
        
        [Test]
        public void TestPreload()
        {
            var query = new DatabaseQuery<TestEntity>(new DummyProcessor());
            query.Preload();

            var result = query.GetResult();
            Assert.AreEqual(5, result.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(result.MoveNext());
                Assert.IsNotNull(result.Current);
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{i}", result.Current.Id.ToString());
                Assert.AreEqual(i, result.Current.Age);
                Assert.AreEqual($"FN{i}", result.Current.FirstName);
                Assert.AreEqual($"LN{i}", result.Current.LastName);
            }
        }

        [Test]
        public void TestGetAll()
        {
            var query = new DatabaseQuery<TestEntity>(new DummyProcessor());

            var result = query.GetResult();
            Assert.AreEqual(5, result.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(result.MoveNext());
                AssertEntity(result.Current, i);
            }
        }

        [Test]
        public void TestWhere()
        {
            var query = new DatabaseQuery<TestEntity>(new DummyProcessor());
            query.Where(e => e["Age"].Value<int>() < 4 && e["Age"].Value<int>() > 0);

            var result = query.GetResult();
            Assert.AreEqual(3, result.Count);
            for (int i = 1; i < 4; i++)
            {
                Assert.IsTrue(result.MoveNext());
                AssertEntity(result.Current, i);
            }

            // Test with non-indexed key but using Where() method.
            query = new DatabaseQuery<TestEntity>(new DummyProcessor());
            query.Where(e => e["LastName"].ToString().Equals("LN0"));

            result = query.GetResult();
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void TestWhereNonIndexed()
        {
            var query = new DatabaseQuery<TestEntity>(new DummyProcessor());
            query.WhereNonIndexed(e => e["LastName"].ToString().Equals("LN0"));

            var result = query.GetResult();
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.MoveNext());
            AssertEntity(result.Current, 0);
        }

        [Test]
        public void TestSort()
        {
            var query = new DatabaseQuery<TestEntity>(new DummyProcessor());
            query.Sort((x, y) => y["Age"].Value<int>() - x["Age"].Value<int>());

            var result = query.GetResult();
            Assert.AreEqual(5, result.Count);
            for (int i = 4; i >= 0; i--)
            {
                Assert.IsTrue(result.MoveNext());
                AssertEntity(result.Current, i);
            }
        }

        [Test]
        public void TestNonIndexed()
        {
            var query = new DatabaseQuery<TestEntity>(new DummyProcessor());
            query.SortNonIndexed((x, y) => y["LastName"].ToString().CompareTo(x["LastName"].ToString()));

            var result = query.GetResult();
            Assert.AreEqual(5, result.Count);
            for (int i = 4; i >= 0; i--)
            {
                Assert.IsTrue(result.MoveNext());
                AssertEntity(result.Current, i);
            }
        }

        [Test]
        public void TestOffsetAndSize()
        {
            var query = new DatabaseQuery<TestEntity>(new DummyProcessor());
            query.Offset(2).Size(2);

            var result = query.GetResult();
            Assert.AreEqual(2, result.Count);
            for (int i = 2; i < 4; i++)
            {
                Assert.IsTrue(result.MoveNext());
                AssertEntity(result.Current, i);
            }
        }

        private void AssertEntity(TestEntity entity, int index)
        {
            Assert.IsNotNull(entity);
            Assert.AreEqual($"00000000-0000-0000-0000-00000000000{index}", entity.Id.ToString());
            Assert.AreEqual(index, entity.Age);
            Assert.AreEqual($"FN{index}", entity.FirstName);
            Assert.AreEqual($"LN{index}", entity.LastName);
        }


        private class DummyIndex : IDatabaseIndex<TestEntity>
        {
            private List<JObject> index;


            public IEnumerable<JObject> Raw => index;


            public DummyIndex(List<JObject> index)
            {
                this.index = index;
            }

            public void Set(JObject index) { }

            public void Remove(string key) { }

            public void Remove(JObject index) { }

            public List<JObject> GetAll() => index;
        }

        private class DummyProcessor : IDatabaseProcessor<TestEntity>
        {
            private DirectoryInfo directory;
            private DirectoryInfo dataDirectory;
            private FileInfo indexFile;


            public IDatabaseIndex<TestEntity> Index { get; private set; }


            public DummyProcessor()
            {
                directory = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "DB"));
                dataDirectory = new DirectoryInfo(Path.Combine(directory.FullName, "data"));
                indexFile = new FileInfo(Path.Combine(directory.FullName, "index.dbi"));

                LoadIndex();
            }

            public void WhileLocked(Action action) => action?.Invoke();

            public FileInfo[] GetDataFiles() => dataDirectory.GetFiles("*.data");

            public void RebuildIndex() => LoadIndex();

            public void SaveIndex() { }

            public void LoadIndex()
            {
                var list = JsonConvert.DeserializeObject<List<JObject>>(File.ReadAllText(indexFile.FullName));
                Index = new DummyIndex(list);
            }

            public void WriteData(List<JObject> data, List<JObject> index) { }

            public void RemoveData(List<TestEntity> data) { }

            public JObject LoadRaw(string key, bool requireLock = true)
            {
                var path = Path.Combine(dataDirectory.FullName, $"{key}.data");
                return JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
            }

            public TestEntity LoadData(string key, bool requireLock = true)
            {
                return ConvertToData(LoadRaw(key, requireLock));
            }

            public TestEntity ConvertToData(JObject raw) => raw.ToObject<TestEntity>();

            public void Dispose() { }
        }
    }
}