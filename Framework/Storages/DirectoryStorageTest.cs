using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Storages.Tests
{
    public class DirectoryStorageTest {
        
        [Test]
        public void TestMove()
        {
            // Preparation
            DirectoryInfo to = new DirectoryInfo(Path.Combine(GetPath(), "MoveTo"));
            if(to.Exists)
                to.Delete(true);
            DirectoryInfo from = new DirectoryInfo(Path.Combine(GetPath(), "MoveFrom"));
            from.Create();

            from.Refresh();
            to.Refresh();

            try
            {
                Assert.IsTrue(from.Exists);
                Assert.IsFalse(to.Exists);

                var storage = new DirectoryStorage(GetDirectory());
                storage.Move("MoveTo", from);

                from.Refresh();
                to.Refresh();
                Assert.IsFalse(from.Exists);
                Assert.IsTrue(to.Exists);
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                throw e;
            }
            finally
            {
                from.Refresh();
                if(from.Exists)
                    from.Delete(true);
                to.Refresh();
                if(to.Exists)
                    to.Delete(true);
            }
        }

        [Test]
        public void TestCopy()
        {
            // Preparation
            DirectoryInfo to = new DirectoryInfo(Path.Combine(GetPath(), "CopyTo"));
            FileInfo toText = new FileInfo(Path.Combine(to.FullName, "text"));
            if(to.Exists)
                to.Delete(true);

            DirectoryInfo from = new DirectoryInfo(Path.Combine(GetPath(), "CopyFrom"));
            FileInfo fromText = new FileInfo(Path.Combine(from.FullName, "text"));
            from.Create();
            File.WriteAllText(fromText.FullName, "test");

            Action refreshState = () =>
            {
                from.Refresh();
                fromText.Refresh();
                to.Refresh();
                toText.Refresh();
            };
            refreshState();

            try
            {
                Assert.IsTrue(from.Exists);
                Assert.IsTrue(fromText.Exists);
                Assert.AreEqual("test", File.ReadAllText(fromText.FullName));
                Assert.IsFalse(to.Exists);
                Assert.IsFalse(toText.Exists);

                var storage = new DirectoryStorage(GetDirectory());
                storage.Copy("CopyTo", from);

                refreshState();
                Assert.IsTrue(from.Exists);
                Assert.IsTrue(fromText.Exists);
                Assert.AreEqual("test", File.ReadAllText(fromText.FullName));
                Assert.IsTrue(to.Exists);
                Assert.IsTrue(toText.Exists);
                Assert.AreEqual("test", File.ReadAllText(toText.FullName));

                File.WriteAllText(fromText.FullName, "test2");
                storage.Copy("CopyTo", from);

                refreshState();
                Assert.IsTrue(from.Exists);
                Assert.IsTrue(fromText.Exists);
                Assert.AreEqual("test2", File.ReadAllText(fromText.FullName));
                Assert.IsTrue(to.Exists);
                Assert.IsTrue(toText.Exists);
                Assert.AreEqual("test2", File.ReadAllText(toText.FullName));
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                to.Refresh();
                from.Refresh();
                if(to.Exists)
                    to.Delete(true);
                if(from.Exists)
                    from.Delete(true);
            }
        }

        [Test]
        public void TestGet()
        {
            // Preparation
            var directories = new List<DirectoryInfo>()
            {
                new DirectoryInfo(Path.Combine(GetPath(), "Directory0")),
                new DirectoryInfo(Path.Combine(GetPath(), "Directory1"))
            };
            directories.ForEach(d =>
            {
                d.Create();
                d.Refresh();
            });

            try
            {
                var storage = new DirectoryStorage(GetDirectory());
                int index = 0;
                directories.ForEach(d =>
                {
                    Assert.IsTrue(d.Exists);

                    var dir = storage.Get($"Directory{index}");
                    Assert.IsTrue(dir.Exists);
                    Assert.AreEqual(d.FullName, dir.FullName);

                    index++;
                });

                index = 0;
                foreach (var d in storage.GetAll())
                {
                    Assert.IsTrue(d.Exists);
                    Assert.AreEqual(directories[index].FullName, d.FullName);

                    index++;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                directories.ForEach(d =>
                {
                    d.Refresh();
                    if(d.Exists)
                        d.Delete(true);
                });
            }
        }

        [Test]
        public void TestExists()
        {
            var testDir = new DirectoryInfo(Path.Combine(GetPath(), "TestDir"));
            testDir.Create();
            testDir.Refresh();

            try
            {
                Assert.IsTrue(testDir.Exists);

                var storage = new DirectoryStorage(GetDirectory());
                Assert.IsTrue(storage.Exists("TestDir"));
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                testDir.Refresh();
                if(testDir.Exists)
                    testDir.Delete(true);
            }
        }

        [Test]
        public void TestDelete()
        {
            var directories = new List<DirectoryInfo>()
            {
                new DirectoryInfo(Path.Combine(GetPath(), "Dir0")),
                new DirectoryInfo(Path.Combine(GetPath(), "Dir1")),
                new DirectoryInfo(Path.Combine(GetPath(), "Dir2")),
                new DirectoryInfo(Path.Combine(GetPath(), "Dir3")),
            };
            directories.ForEach(d =>
            {
                d.Create();
                d.Refresh();
            });

            try
            {
                directories.ForEach(d =>
                {
                    Assert.IsTrue(d.Exists);
                });

                var storage = new DirectoryStorage(GetDirectory());
                for (int i = 0; i < directories.Count; i++)
                {
                    Assert.IsTrue(storage.Exists($"Dir{i}"));
                }
                storage.Delete("Dir3");
                for (int i = 0; i < directories.Count; i++)
                {
                    directories[i].Refresh();
                    if (i == directories.Count - 1)
                    {
                        Assert.IsFalse(directories[i].Exists);
                        Assert.IsFalse(storage.Exists($"Dir{i}"));
                    }
                    else
                    {
                        Assert.IsTrue(directories[i].Exists);
                        Assert.IsTrue(storage.Exists($"Dir{i}"));
                    }
                }

                storage.DeleteAll();
                for (int i = 0; i < directories.Count; i++)
                {
                    directories[i].Refresh();
                    Assert.IsFalse(directories[i].Exists);
                    Assert.IsFalse(storage.Exists($"Dir{i}"));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                directories.ForEach(d =>
                {
                    d.Refresh();
                    if(d.Exists)
                        d.Delete(true);
                });
            }
        }

        [Test]
        public void TestRestoreBackup()
        {
            var testBackup = new DirectoryInfo(Path.Combine(GetPath(), "test_backup"));
            testBackup.Create();
            testBackup.Refresh();

            var test = new DirectoryInfo(Path.Combine(GetPath(), "test"));
            test.Refresh();

            try
            {
                Assert.IsTrue(testBackup.Exists);

                var storage = new DirectoryStorage(GetDirectory());
                var restored = storage.RestoreBackup();
                Assert.AreEqual(1, restored.Count);
                Assert.AreEqual("test", restored[0]);
                Assert.IsTrue(storage.Exists("test"));
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                testBackup.Refresh();
                test.Refresh();
                if(testBackup.Exists)
                    testBackup.Delete(true);
                if(test.Exists)
                    test.Delete(true);
            }
        }

        private string GetPath()
        {
            return Path.Combine(Application.streamingAssetsPath, "Storages/DirectoryStorageTest");
        }

        private DirectoryInfo GetDirectory()
        {
            return new DirectoryInfo(GetPath());
        }
    }
}