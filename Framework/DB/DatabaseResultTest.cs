using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.DB.Entities.Tests;

namespace PBFramework.DB.Tests
{
    public class DatabaseResultTest {
        
        [Test]
        public void TestEnumerator()
        {
            var result = new DatabaseResult<TestEntity>(DummyEnumerator(), 5);
            Assert.AreEqual(5, result.Count);

            for (int i = 0; i < 6; i++)
            {
                if (i == 5)
                {
                    Assert.IsFalse(result.MoveNext());
                }
                else
                {
                    Assert.IsTrue(result.MoveNext());

                    var entity = result.Current;
                    Assert.AreEqual(i, entity.Age);
                    Assert.AreEqual("FN" + i, entity.FirstName);
                    Assert.AreEqual("LN" + i, entity.LastName);
                }
            }
        }

        [Test]
        public void TestReset()
        {
            var result = new DatabaseResult<TestEntity>(DummyEnumerator(), 5);
            try
            {
                result.Reset();
                Assert.Fail("This should've failed!");
            }
            catch (NotSupportedException e)
            {
                Debug.Log("Caught NotSupportedException.");
            }
        }

        [Test]
        public void TestDispose()
        {
            var result = new DatabaseResult<TestEntity>(DummyEnumerator(), 5);
            result.Dispose();
            try
            {
                var a = result.Current;
                Assert.Fail("This should've failed!");
            }
            catch (ObjectDisposedException) { }

            try
            {
                result.MoveNext();
                Assert.Fail("This should've failed!");
            }
            catch (ObjectDisposedException) { }

            try
            {
                var enumerator = result.GetEnumerator();
                enumerator.MoveNext();
                Assert.Fail("This should've failed!");
            }
            catch (ObjectDisposedException) { }
        }

        [Test]
        public void TestEnumerable()
        {
            var result = new DatabaseResult<TestEntity>(DummyEnumerator(), 5);
            int i = 0;
            foreach (var r in result)
            {
                Assert.AreEqual(i, r.Age);
                Assert.AreEqual("FN" + i, r.FirstName);
                Assert.AreEqual("LN" + i, r.LastName);
                i++;
            }
        }

        private IEnumerator<TestEntity> DummyEnumerator()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new TestEntity()
                {
                    Id = Guid.NewGuid(),
                    Age = i,
                    FirstName = "FN" + i,
                    LastName = "LN" + i,
                };
            }
        }
    }
}