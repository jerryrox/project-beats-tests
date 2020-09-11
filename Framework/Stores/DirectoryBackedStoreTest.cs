using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBFramework.DB;
using PBFramework.DB.Entities;
using PBFramework.Storages;
using PBFramework.Threading;

namespace PBFramework.Stores.Test
{
    public class DirectoryBackedStoreTest
    {

        [UnityTest]
        public IEnumerator TestInitialize()
        {
            var store = new DummyStore();
            yield return InitStore(store);
        }

        [UnityTest]
        public IEnumerator TestImport()
        {
            var store = new DummyStore();
            yield return InitStore(store);

            var listeners = new List<TaskListener<DummyIndex>>()
            {
                new TaskListener<DummyIndex>(),
                new TaskListener<DummyIndex>(),
                new TaskListener<DummyIndex>()
            };
            for (int i = 0; i < listeners.Count; i++)
            {
                yield return WaitImport(store, i, listeners[i]);

                var value = listeners[i].Value;
                Assert.IsNotNull(value);
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{i}", value.Id.ToString());
                Assert.AreEqual($"Name{i}", value.Name);
                Assert.AreEqual(i, value.Age);
                Assert.AreEqual(i % 2 == 1, value.IsVerified);
            }
            Assert.AreEqual(3, store.Count);
        }

        [UnityTest]
        public IEnumerator TestReload()
        {
            var store = new DummyStore();
            yield return InitStore(store);

            var listeners = new List<TaskListener<DummyIndex>>()
            {
                new TaskListener<DummyIndex>(),
                new TaskListener<DummyIndex>(),
                new TaskListener<DummyIndex>()
            };
            for (int i = 0; i < listeners.Count; i++)
            {
                yield return WaitImport(store, i, listeners[i]);

                var value = listeners[i].Value;
                Assert.IsNotNull(value);
                Assert.AreEqual(GetDataDirectory(i).FullName, value.Directory.FullName);
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{i}", value.Id.ToString());
                Assert.AreEqual($"Name{i}", value.Name);
                Assert.AreEqual(i, value.Age);
                Assert.AreEqual(i % 2 == 1, value.IsVerified);
            }
            Assert.AreEqual(3, store.Count);

            store = new DummyStore(false);
            yield return InitStore(store, 3);
            Assert.AreEqual(3, store.Count);
            int count = 0;
            foreach (var value in store.GetAll())
            {
                Assert.IsNotNull(value);
                Assert.AreEqual(GetDataDirectory(count).FullName, value.Directory.FullName);
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{count}", value.Id.ToString());
                Assert.AreEqual($"Name{count}", value.Name);
                Assert.AreEqual(count, value.Age);
                Assert.AreEqual(count % 2 == 1, value.IsVerified);
                count++;
            }
        }

        [UnityTest]
        public IEnumerator TestGet()
        {
            var store = new DummyStore();
            yield return InitStore(store);

            var listeners = new List<TaskListener<DummyIndex>>()
            {
                new TaskListener<DummyIndex>(),
                new TaskListener<DummyIndex>(),
                new TaskListener<DummyIndex>()
            };
            for (int i = 0; i < listeners.Count; i++)
            {
                yield return WaitImport(store, i, listeners[i]);

                var value = listeners[i].Value;
                Assert.IsNotNull(value);
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{i}", value.Id.ToString());
                Assert.AreEqual($"Name{i}", value.Name);
                Assert.AreEqual(i, value.Age);
                Assert.AreEqual(i % 2 == 1, value.IsVerified);
            }
            Assert.AreEqual(3, store.Count);

            int index = 1;
            foreach (var data in store.Get(query => query.WhereNonIndexed(d => d["Age"].Value<int>() > 0)))
            {
                Assert.IsNotNull(data);
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{index}", data.Id.ToString());
                Assert.AreEqual($"Name{index}", data.Name);
                Assert.AreEqual(index, data.Age);
                Assert.AreEqual(index % 2 == 1, data.IsVerified);
                index++;
            }
        }

        [UnityTest]
        public IEnumerator TestDelete()
        {
            var store = new DummyStore();
            yield return InitStore(store);

            var listeners = new List<TaskListener<DummyIndex>>()
            {
                new TaskListener<DummyIndex>(),
                new TaskListener<DummyIndex>(),
                new TaskListener<DummyIndex>()
            };
            for (int i = 0; i < listeners.Count; i++)
            {
                yield return WaitImport(store, i, listeners[i]);

                var value = listeners[i].Value;
                Assert.IsNotNull(value);
                Assert.AreEqual($"00000000-0000-0000-0000-00000000000{i}", value.Id.ToString());
                Assert.AreEqual($"Name{i}", value.Name);
                Assert.AreEqual(i, value.Age);
                Assert.AreEqual(i % 2 == 1, value.IsVerified);
            }
            Assert.AreEqual(3, store.Count);

            var removeList = new List<DummyIndex>();
            store.OnRemoveData += (removed) => removeList.Add(removed);

            store.Delete(listeners[2].Value);
            Assert.AreEqual(2, store.Count);
            Assert.IsNotNull(removeList[0]);
            Assert.AreEqual($"00000000-0000-0000-0000-000000000002", removeList[0].Id.ToString());
            Assert.AreEqual($"Name2", removeList[0].Name);
            Assert.AreEqual(2, removeList[0].Age);
            Assert.AreEqual(false, removeList[0].IsVerified);

            store.DeleteAll();
            Assert.AreEqual(1, removeList.Count);
            Assert.AreEqual(0, store.Count);
            for (int i = 0; i < 3; i++)
            {
                Assert.IsFalse(GetDataDirectory(i).Exists);
            }
        }

        private IEnumerator InitStore(DummyStore store, int expectedCount = 0)
        {
            var listener = new TaskListener();
            store.Reload(listener).Wait();
            yield return null;
            Assert.AreEqual(1f, listener.Progress, 0.00001f);
            Assert.AreEqual(expectedCount, store.Count);
        }

        private IEnumerator WaitImport(DummyStore store, int index, TaskListener<DummyIndex> listener)
        {
            // New data callback
            DummyIndex loadedData = null;
            Action<DummyIndex> onNewData = d => loadedData = d;

            // Load completion callback
            bool loaded = false;
            listener.OnFinished += delegate { loaded = true; };

            // Start importing
            store.OnNewData += onNewData;
            store.Import(GetDataFile(index), false, listener);
            while (!loaded)
                yield return null;
            store.OnNewData -= onNewData;

            // Checking
            Assert.AreEqual(1f, listener.Progress, 0.0000001f);
            Assert.AreEqual(loadedData, listener.Value);
        }

        private FileInfo GetDataFile(int index)
        {
            return new FileInfo(Path.Combine(GetPath(), $"Data{index}.zip"));
        }

        private DirectoryInfo GetDataDirectory(int index)
        {
            return new DirectoryInfo(Path.Combine(GetPath(), $"Storage/00000000-0000-0000-0000-00000000000{index}"));
        }

        private static DirectoryInfo GetDatabaseDirectory()
        {
            return new DirectoryInfo(Path.Combine(GetPath(), "DB"));
        }

        private static DirectoryInfo GetStorageDirectory()
        {
            return new DirectoryInfo(Path.Combine(GetPath(), "Storage"));
        }

        private static string GetPath()
        {
            return Path.Combine(TestConstants.TestAssetPath, "Stores/DirectoryBackedStoreTest");
        }

        private class DummyStore : DirectoryBackedStore<DummyIndex>
        {
            public DummyStore(bool deleteFirst = true) : base()
            {
                var dir = GetDatabaseDirectory();
                if(dir.Exists && deleteFirst)
                    dir.Delete(true);
                dir = GetStorageDirectory();
                if(dir.Exists && deleteFirst)
                    dir.Delete(true);
            }

            protected override IDatabase<DummyIndex> CreateDatabase()
            {
                return new Database<DummyIndex>(GetDatabaseDirectory());
            }

            protected override IDirectoryStorage CreateStorage()
            {
                return new DirectoryStorage(GetStorageDirectory());
            }

            protected override DummyIndex ParseData(DirectoryInfo directory, DummyIndex data)
            {
                if (data == null)
                {
                    var dataFile = directory.GetFiles("*.data");
                    if(dataFile.Length == 0) return null;

                    var json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(dataFile[0].FullName));
                    data = new DummyIndex()
                    {
                        Id = new Guid(json["Id"].ToString()),
                        Name = json["Name"].ToString(),
                        Age = json["Age"].Value<int>(),
                        IsVerified = json["IsVerified"].Value<bool>(),
                    };
                }
                return data;
            }

            protected override void PostProcessData(DummyIndex data, Guid? id = null)
            {
                id = data.Id;
                base.PostProcessData(data, id);
            }
        }

        private class DummyIndex : DatabaseEntity, IDirectoryIndex
        {
            [Indexed]
            public int HashCode { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }

            public bool IsVerified { get; set; }

            [JsonIgnore]
            public DirectoryInfo Directory { get; set; }


            public IEnumerable GetHashParams()
            {
                yield return Name;
                yield return Age;
            }
        }
    }
}