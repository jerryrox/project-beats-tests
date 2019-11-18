using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.DB.Entities.Tests;

namespace PBFramework.DB.Tests
{
    public class DatabaseTest {
        
        [Test]
        public void TestInitialize()
        {
            var database = new Database<TestEntity>("TestEntity");
            Debug.Log("Database at: " + database.Directory.FullName);
            Assert.AreEqual(Path.Combine(Application.persistentDataPath, "TestEntity"), database.Directory.FullName);

            Assert.IsTrue(database.Initialize());
            Assert.IsTrue(database.IsAlive);
        }

        [Test]
        public void TestEdit()
        {
            // Detailed tests should be done on DatabaseEditorTest.

            var database = new Database<TestEntity>("TestEntity");
            database.Initialize();
            var editor = database.Edit();
            Assert.AreEqual(typeof(DatabaseEditor<TestEntity>), editor.GetType());
        }

        [Test]
        public void TestQuery()
        {
            // Detailed tests should be done on DatabaseQueryTest.

            var database = new Database<TestEntity>("TestEntity");
            database.Initialize();
            var query = database.Query();
            Assert.AreEqual(typeof(DatabaseQuery<TestEntity>), query.GetType());
        }

        [Test]
        public void TestDispose()
        {
            var database = new Database<TestEntity>("TestEntity");
            database.Initialize();
            database.Dispose();
            Assert.IsFalse(database.IsAlive);

            try
            {
                database.Edit();
                Assert.Fail("Should've thrown an exception!");
            }
            catch (ObjectDisposedException) { }

            try
            {
                database.Query();
                Assert.Fail("Should've thrown an exception!");
            }
            catch (ObjectDisposedException) { }
        }
    }
}