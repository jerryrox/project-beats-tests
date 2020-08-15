using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Storages.Tests
{
    public class TempDirectoryStorageTest {
        
        [Test]
        public void TestNameCreation()
        {
            var storage = new TempDirectoryStorage("TempDirectoryTest");
            try
            {
                Debug.Log("TestNameCreation - Location: " + storage.Container.FullName);
                Assert.IsTrue(storage.Container.Exists);
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                throw e;
            }
            finally
            {
                if(storage.Container.Exists)
                    storage.Container.Delete(true);
            }
        }

        [Test]
        public void TestDirectoryCreation()
        {
            var dir = new DirectoryInfo(Path.Combine(TestConstants.TestAssetPath, "Storages/TempDirectoryTest2"));
            var storage = new TempDirectoryStorage(dir);
            try
            {
                Debug.Log("TestDirectoryCreation - Location: " + storage.Container.FullName);
                Assert.IsTrue(storage.Container.Exists);
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                throw e;
            }
            finally
            {
                if(storage.Container.Exists)
                    storage.Container.Delete(true);
            }
        }
    }
}