using System;
using NUnit.Framework;

namespace PBFramework.Data.Tests
{
    public class SortedListTest
    {
        [Test]
        public void AddTest()
        {
            var list = new SortedList<string>();
            list.Add("z");
            list.Add("c");
            list.Add("a");

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("a", list[0]);
            Assert.AreEqual("c", list[1]);
            Assert.AreEqual("z", list[2]);
            Assert.AreEqual(0, list.IndexOf("a"));
            Assert.AreEqual(1, list.IndexOf("c"));
            Assert.AreEqual(2, list.IndexOf("z"));
        }

        [Test]
        public void RemoveTest()
        {
            var list = new SortedList<string>();
            list.Add("a");
            list.Add("b");
            list.Add("c");

            Assert.AreEqual(3, list.Count);

            list.Remove("b");
            Assert.AreEqual(2, list.Count);
            Assert.IsFalse(list.Contains("b"));
            Assert.AreEqual("a", list[0]);
            Assert.AreEqual("c", list[1]);

            list.RemoveAt(0);
            Assert.AreEqual(1, list.Count);
            Assert.IsFalse(list.Contains("a"));
            Assert.AreEqual("c", list[0]);
        }
    }
}