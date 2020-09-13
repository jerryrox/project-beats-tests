using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBFramework.DB.Entities.Tests;

namespace PBFramework.DB.Tests
{
    public class DatabaseProcessorTest {

        private const string TestDbFolder = "DBProcessorTest";

        [Test]
        public void TestGetDataFiles()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());

            var files = processor.GetDataFiles();
            for (int i = 0; i < files.Length; i++)
            {
                Assert.AreEqual(
                    Path.Combine(TestConstants.TestAssetPath, $"{TestDbFolder}/data/00000000-0000-0000-0000-00000000000{i}.data"),
                    files[i].FullName
                );
            }
        }

        [Test]
        public void TestRebuildIndex()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());

            var index = processor.Index;
            Assert.IsNull(index);

            processor.RebuildIndex();
            index = processor.Index;
            Assert.IsNotNull(processor.Index);
            Assert.AreEqual(5, index.Raw.ToList().Count);

            processor.RebuildIndex();
            Assert.AreNotEqual(index, processor.Index);
            Assert.AreEqual(5, index.Raw.ToList().Count);
        }

        [Test]
        public void TestLoadIndex()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());
            processor.LoadIndex();

            foreach (var index in processor.Index.Raw)
            {
                Debug.LogWarning(index.ToString());
            }

            Assert.IsNotNull(processor.Index);
            Assert.AreEqual(5, processor.Index.Raw.Count());
        }

        [Test]
        public void TestSaveIndex()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());
            processor.LoadIndex();

            var indexPath = Path.Combine(TestConstants.TestAssetPath, $"{TestDbFolder}/index.dbi");
            File.Delete(indexPath);

            processor.SaveIndex();
            Assert.IsTrue(File.Exists(indexPath));
            Debug.Log("Index content: " + File.ReadAllText(indexPath));

            processor.LoadIndex();
            int index = 0;
            foreach (var json in processor.Index.Raw)
            {
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{index}", json["Id"].ToString());
                index++;
            }
            Assert.AreEqual(5, index);
        }

        [Test]
        public void TestWriteData()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());
            processor.LoadIndex();

            Assert.AreEqual(5, processor.Index.Raw.Count());

            List<JObject> data = new List<JObject>()
            {
                JsonConvert.DeserializeObject<JObject>(@"
                    {
                        'Id' : '00000000-0000-0000-0000-000000000005',
                        'Age' : 5,
                        'Name' : 'FN5',
                        'LastName' : 'LN5'
                    }
                ")
            };
            List<JObject> index = new List<JObject>()
            {
                JsonConvert.DeserializeObject<JObject>(@"
                    {'Id':'00000000-0000-0000-0000-000000000005','Age':5,'Name':'FN5'}
                ")
            };

            processor.WriteData(data, index);

            Assert.AreEqual(6, processor.Index.Raw.Count());
            Assert.AreEqual(6, processor.GetDataFiles().Length);
            for (int i = 0; i < 6; i++)
            {
                var d = processor.LoadData($"00000000-0000-0000-0000-00000000000{i}");
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{i}", d.Id.ToString());
                Assert.AreEqual(i, d.Age);
                Assert.AreEqual($"FN{i}", d.FirstName);
                Assert.AreEqual($"LN{i}", d.LastName);
            }
        }

        [Test]
        public void TestRemoveData()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());
            processor.LoadIndex();

            // Load all data first.
            List<TestEntity> data = new List<TestEntity>();
            for (int i = 0; i < 5; i++)
            {
                data.Add(processor.LoadData($"00000000-0000-0000-0000-00000000000{i}"));

                var d = data[i];
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{i}", d.Id.ToString());
                Assert.AreEqual(i, d.Age);
                Assert.AreEqual($"FN{i}", d.FirstName);
                Assert.AreEqual($"LN{i}", d.LastName);
            }

            // Get deleting targets
            List<TestEntity> targets = new List<TestEntity>()
            {
                data[3],
                data[4]
            };

            processor.RemoveData(targets);

            var index = processor.Index.GetAll();
            Assert.AreEqual(3, index.Count);
            Assert.AreEqual(processor.GetDataFiles().Length, index.Count);

            for (int i = 0; i < 3; i++)
            {
                var d = processor.LoadData($"00000000-0000-0000-0000-00000000000{i}");
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{i}", d.Id.ToString());
                Assert.AreEqual(i, d.Age);
                Assert.AreEqual($"FN{i}", d.FirstName);
                Assert.AreEqual($"LN{i}", d.LastName);
                Assert.AreEqual(d.Id.ToString(), index[i]["Id"].ToString());
            }
        }

        [Test]
        public void TestLoadRaw()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());
            processor.LoadIndex();

            var json = processor.LoadRaw("00000000-0000-0000-0000-000000000001");
            Assert.AreEqual("00000000-0000-0000-0000-000000000001", json["Id"].ToString());
            Assert.AreEqual(1, json["Age"].Value<int>());
            Assert.AreEqual("FN1", json["Name"].ToString());
            Assert.AreEqual("LN1", json["LastName"].ToString());
        }

        [Test]
        public void TestLoadData()
        {
            Debug.LogWarning(TestConstants.TestAssetPath);
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());
            processor.LoadIndex();

            var entity = processor.LoadData("00000000-0000-0000-0000-000000000001");
            Assert.AreEqual("00000000-0000-0000-0000-000000000001", entity.Id.ToString());
            Assert.AreEqual(1, entity.Age);
            Assert.AreEqual("FN1", entity.FirstName);
            Assert.AreEqual("LN1", entity.LastName);
        }

        [Test]
        public void TestConvertToData()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());
            processor.LoadIndex();

            JObject json = JsonConvert.DeserializeObject<JObject>(@"
                {
                    'Id' : '00000000-0000-0000-0000-000000000000',
                    'Age' : 0,
                    'Name' : 'FN0',
                    'LastName' : 'LN0'
                }
            ");
            var entity = processor.ConvertToData(json);
            Assert.AreEqual("00000000-0000-0000-0000-000000000000", entity.Id.ToString());
            Assert.AreEqual(0, entity.Age);
            Assert.AreEqual("FN0", entity.FirstName);
            Assert.AreEqual("LN0", entity.LastName);
        }

        [Test]
        public void TestWipe()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());
            processor.LoadIndex();

            Assert.AreEqual(5, processor.Index.GetAll().Count);
            processor.Wipe();
            Assert.AreEqual(0, processor.Index.GetAll().Count);
        }


        private class DummyDatabase : IDatabase<TestEntity>
        {
            public bool IsAlive => true;

            public DirectoryInfo Directory => new DirectoryInfo(Path.Combine(TestConstants.TestAssetPath, TestDbFolder));


            public DummyDatabase()
            {
                Assert.IsTrue(Initialize());
            }

            public bool Initialize()
            {
                if (Directory.Exists)
                {
                    Directory.Delete(true);
                    Directory.Refresh();
                }

                Directory.Create();
                Directory.Refresh();

                dynamic[] datas = new dynamic[5];
                dynamic[] indexes = new dynamic[5];
                for (int i = 0; i < 5; i++)
                {
                    datas[i] = new
                    {
                        Id = $"00000000-0000-0000-0000-00000000000{i}",
                        Age = i,
                        Name = $"FN{i}",
                        LastName = $"LN{i}",
                    };
                    indexes[i] = new
                    {
                        Id = $"00000000-0000-0000-0000-00000000000{i}",
                        Age = i,
                        Name = $"FN{i}",
                    };
                }

                File.WriteAllText(
                    Path.Combine(Directory.FullName, "index.dbi"),
                    JsonConvert.SerializeObject(indexes)
                );
                var dataFolder = Directory.CreateSubdirectory("data");
                for (int i = 0; i < datas.Length; i++)
                {
                    File.WriteAllText(
                        Path.Combine(dataFolder.FullName, $"{datas[i].Id}.data"),
                        JsonConvert.SerializeObject(datas[i])
                    );
                }

                return true;
            }

            public IDatabaseEditor<TestEntity> Edit() => null;

            public IDatabaseQuery<TestEntity> Query() => null;

            public void Wipe() { }

            public void Dispose() { }
        }
    }
}