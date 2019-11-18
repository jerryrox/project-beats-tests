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
        
        [Test]
        public void TestWhileLocked()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());

            int val = 0;
            var tasks = new Task[8];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() => val++);
            }

            Task.WaitAll(tasks);
            Assert.AreEqual(tasks.Length, val);
        }

        [Test]
        public void TestGetDataFiles()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());

            var files = processor.GetDataFiles();
            for (int i = 0; i < files.Length; i++)
            {
                Assert.AreEqual(
                    Path.Combine(Application.streamingAssetsPath, $"DB/data/00000000-0000-0000-0000-00000000000{i}.data"),
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

            Assert.IsNotNull(processor.Index);
            Assert.AreEqual(5, processor.Index.Raw.Count());
        }

        [Test]
        public void TestSaveIndex()
        {
            var processor = new DatabaseProcessor<TestEntity>(new DummyDatabase());
            processor.LoadIndex();

            var indexPath = Path.Combine(Application.streamingAssetsPath, "DB/index.dbi");
            var backupPath = Path.Combine(Application.streamingAssetsPath, "DB/index2.dbi");
            try
            {
                if (File.Exists(indexPath))
                {
                    File.Move(indexPath, backupPath);
                }
                Assert.IsFalse(File.Exists(indexPath));

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

                if(File.Exists(backupPath))
                    File.Delete(backupPath);
            }
            catch (Exception e)
            {
                if (File.Exists(backupPath) && !File.Exists(indexPath))
                {
                    File.Move(backupPath, indexPath);
                }
                throw e;
            }
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

            try
            {
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
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                var newDataFile = Path.Combine(Application.streamingAssetsPath, "data/00000000-0000-0000-0000-000000000005.data");
                if (File.Exists(newDataFile))
                {
                    File.Delete(newDataFile);
                }
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

            // Backup original DB first.
            var sourcePath = Path.Combine(Application.streamingAssetsPath, "DB");
            var backupPath = Path.Combine(Application.streamingAssetsPath, "DB_Backup");
            if (Directory.Exists(backupPath))
            {
                Directory.Delete(backupPath, true);
            }
            Directory.CreateDirectory(backupPath);
            Directory.CreateDirectory(Path.Combine(backupPath, "data"));
            File.Copy(Path.Combine(sourcePath, "index.dbi"), Path.Combine(backupPath, "index.dbi"));
            for (int i = 0; i < 5; i++)
            {
                File.Copy(
                    Path.Combine(sourcePath, $"data/00000000-0000-0000-0000-00000000000{i}.data"),
                    Path.Combine(backupPath, $"data/00000000-0000-0000-0000-00000000000{i}.data")
                );
            }

            try
            {
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
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Directory.Delete(sourcePath, true);
                Directory.Move(backupPath, sourcePath);
                Debug.Log("Reverted test environment.");
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


        private class DummyDatabase : IDatabase<TestEntity>
        {
            public bool IsAlive => true;

            public DirectoryInfo Directory => new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "DB"));


            public bool Initialize() => true;

            public IDatabaseEditor<TestEntity> Edit() => null;

            public IDatabaseQuery<TestEntity> Query() => null;

            public void Dispose() { }
        }
    }
}