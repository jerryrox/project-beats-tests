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
    public class DatabaseEditorTest {
        
        [Test]
        public void TestWrite()
        {
            var processor = new DummyProcessor();
            var editor = new DatabaseEditor<TestEntity>(processor);

            var entity = new TestEntity()
            {
                Age = 100,
                FirstName = "FN100",
                LastName = "LN100",
                Id = Guid.NewGuid()
            };
            editor.Write(entity);
            Assert.AreEqual(1, editor.WriteCount);
            Assert.AreEqual(0, editor.RemoveCount);

            editor.Commit();
            Assert.AreEqual(0, editor.WriteCount);
            Assert.AreEqual(0, editor.RemoveCount);
            Assert.AreEqual(1, processor.Data.Count);
            Assert.AreEqual(1, processor.Index.Count);
            Assert.AreEqual(entity.Id.ToString(), processor.Data[0]["Id"].ToString());
            Assert.AreEqual(entity.Age, processor.Data[0]["Age"].Value<int>());
            Assert.AreEqual(entity.FirstName, processor.Data[0]["Name"].ToString());
            Assert.AreEqual(entity.LastName, processor.Data[0]["LastName"].ToString());
        }

        [Test]
        public void TestWriteRange()
        {
            var processor = new DummyProcessor();
            var editor = new DatabaseEditor<TestEntity>(processor);

            var entities = new List<TestEntity>()
            {
                new TestEntity()
                {
                    Age = 100,
                    FirstName = "FN100",
                    LastName = "LN100",
                    Id = Guid.NewGuid()
                },
                new TestEntity()
                {
                    Age = 101,
                    FirstName = "FN101",
                    LastName = "LN101",
                    Id = Guid.NewGuid()
                },
            };
            entities.Sort((x, y) => x.Id.CompareTo(y.Id));

            editor.WriteRange(entities);
            Assert.AreEqual(2, editor.WriteCount);
            Assert.AreEqual(0, editor.RemoveCount);

            editor.Commit();
            Assert.AreEqual(0, editor.WriteCount);
            Assert.AreEqual(0, editor.RemoveCount);
            Assert.AreEqual(2, processor.Data.Count);
            Assert.AreEqual(2, processor.Index.Count);
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                Assert.AreEqual(entity.Id.ToString(), processor.Data[i]["Id"].ToString());
                Assert.AreEqual(entity.Age, processor.Data[i]["Age"].Value<int>());
                Assert.AreEqual(entity.FirstName, processor.Data[i]["Name"].ToString());
                Assert.AreEqual(entity.LastName, processor.Data[i]["LastName"].ToString());
            }
        }

        [Test]
        public void TestRemove()
        {
            var processor = new DummyProcessor();
            var editor = new DatabaseEditor<TestEntity>(processor);

            editor.Remove(new TestEntity() {
                Age = 100,
                FirstName = "FN100",
                LastName = "LN100",
                Id = Guid.NewGuid()
            });
            Assert.AreEqual(0, editor.WriteCount);
            Assert.AreEqual(1, editor.RemoveCount);
        }

        [Test]
        public void TestRemoveRange()
        {
            var processor = new DummyProcessor();
            var editor = new DatabaseEditor<TestEntity>(processor);

            var entities = new List<TestEntity>()
            {
                new TestEntity()
                {
                    Age = 100,
                    FirstName = "FN100",
                    LastName = "LN100",
                    Id = Guid.NewGuid()
                },
                new TestEntity()
                {
                    Age = 101,
                    FirstName = "FN101",
                    LastName = "LN101",
                    Id = Guid.NewGuid()
                },
            };

            editor.RemoveRange(entities);
            Assert.AreEqual(0, editor.WriteCount);
            Assert.AreEqual(2, editor.RemoveCount);
        }


        private class DummyProcessor : IDatabaseProcessor<TestEntity>
        {
            public List<JObject> Data { get; private set; } = new List<JObject>();

            public List<JObject> Index { get; private set; } = new List<JObject>();


            IDatabaseIndex<TestEntity> IDatabaseProcessor<TestEntity>.Index => null;

            public void WhileLocked(Action action) => action?.Invoke();

            public FileInfo[] GetDataFiles() => null;

            public void RebuildIndex() { }

            public void SaveIndex() { }

            public void LoadIndex() { }

            public void WriteData(List<JObject> data, List<JObject> index)
            {
                SetData(data, Data);
                SetData(index, Index);
            }

            public void RemoveData(List<TestEntity> data)
            {
                foreach (var d in data)
                {
                    for (int i = 0; i < Data.Count; i++)
                    {
                        if (d.Id.ToString() == Data[i]["Id"].ToString())
                        {
                            Data.RemoveAt(i);
                            break;
                        }
                    }
                    for (int i = 0; i < Index.Count; i++)
                    {
                        if (d.Id.ToString() == Index[i]["Id"].ToString())
                        {
                            Index.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            public JObject LoadRaw(string key, bool requireLock = true) => null;

            public TestEntity LoadData(string key, bool requireLock = true) => null;

            public TestEntity ConvertToData(JObject raw) => null;

            public void Dispose() { }

            private void SetData(List<JObject> from, List<JObject> to)
            {
                foreach (var d in from)
                {
                    bool replaced = false;
                    for (int i = 0; i < to.Count; i++)
                    {
                        if (to[i]["Id"].ToString() == d["Id"].ToString())
                        {
                            to[i] = d;
                            replaced = true;
                            continue;
                        }
                    }
                    if(!replaced)
                        to.Add(d);
                }
            }
        }
    }
}