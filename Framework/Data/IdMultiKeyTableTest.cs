using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Data.Tests
{
    public class IdMultiKeyTableTest {
        
        [Test]
        public void TestCreation()
        {
            var table = new IdMultiKeyTable<Dummy>();
            Assert.AreEqual(1, table.KeysetCount);

            Dummy dummy = new Dummy();
            table.Add(dummy);
            Assert.AreEqual(1, table.Count);

            Assert.AreSame(dummy, table.Get("Id", dummy.Id.ToString()));
        }

        private class Dummy : IHasIdentifier
        {
            public Guid Id { get; set; }
        }
    }
}