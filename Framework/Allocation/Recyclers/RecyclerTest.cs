using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Allocation.Recyclers
{
    public class RecyclerTest {
        
        [Test]
        public void TestPrecook()
        {
            var recycler = new Recycler<Dummy>(() => new Dummy());
            Assert.AreEqual(0, recycler.TotalCount);
            Assert.AreEqual(0, recycler.UnusedCount);

            recycler.Precook(5);
            Assert.AreEqual(5, recycler.TotalCount);
            Assert.AreEqual(5, recycler.UnusedCount);

            recycler.Precook(1);
            Assert.AreEqual(6, recycler.TotalCount);
            Assert.AreEqual(6, recycler.UnusedCount);
        }

        [Test]
        public void TestGetNext()
        {
            var recycler = new Recycler<Dummy>(() => new Dummy());
            Assert.AreEqual(0, recycler.TotalCount);
            Assert.AreEqual(0, recycler.UnusedCount);

            var dummy = recycler.GetNext();
            Assert.IsTrue(dummy.IsAlive);
            Assert.AreSame(recycler, dummy.Recycler);
            Assert.AreEqual(1, recycler.TotalCount);
            Assert.AreEqual(0, recycler.UnusedCount);

            var dummy2 = recycler.GetNext();
            Assert.IsTrue(dummy2.IsAlive);
            Assert.AreSame(recycler, dummy2.Recycler);
            Assert.AreEqual(2, recycler.TotalCount);
            Assert.AreEqual(0, recycler.UnusedCount);
            Assert.AreNotSame(dummy2, dummy);
        }

        [Test]
        public void TestReturn()
        {
            var recycler = new Recycler<Dummy>(() => new Dummy());
            Assert.AreEqual(0, recycler.TotalCount);
            Assert.AreEqual(0, recycler.UnusedCount);

            recycler.Precook(2);
            Assert.AreEqual(2, recycler.TotalCount);
            Assert.AreEqual(2, recycler.UnusedCount);

            var dummy = recycler.GetNext();
            Assert.AreEqual(2, recycler.TotalCount);
            Assert.AreEqual(1, recycler.UnusedCount);
            Assert.IsTrue(dummy.IsAlive);

            recycler.Return(dummy);
            Assert.AreEqual(2, recycler.TotalCount);
            Assert.AreEqual(2, recycler.UnusedCount);
            Assert.IsFalse(dummy.IsAlive);

            dummy = recycler.GetNext();
            Assert.AreEqual(2, recycler.TotalCount);
            Assert.AreEqual(1, recycler.UnusedCount);
            Assert.IsTrue(dummy.IsAlive);

            dummy.ReturnToRecycler();
            Assert.AreEqual(2, recycler.TotalCount);
            Assert.AreEqual(2, recycler.UnusedCount);
            Assert.IsFalse(dummy.IsAlive);
        }

        private class Dummy : IRecyclable<Dummy>
        {
            public bool IsAlive { get; set; } = false;

            public IRecycler<Dummy> Recycler { get; set; }


            public void OnRecycleNew()
            {
                IsAlive = true;
            }

            public void OnRecycleDestroy()
            {
                IsAlive = false;
            }
        }
    }
}