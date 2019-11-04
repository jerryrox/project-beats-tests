using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Exceptions;

namespace PBFramework.Data.Tests
{
    public class MultiKeyTableTest {
        
        [Test]
        public void TestAddKeyset()
        {
            var table = new MultiKeyTable<Dummy>();

            try
            {
                table.AddKeyset(null, null);
                Assert.Fail("The statement was supposed to throw an exception.");
            }
            catch (ArgumentException e) {}
            Assert.AreEqual(0, table.KeysetCount);
            try
            {
                table.AddKeyset("", null);
                Assert.Fail("The statement was supposed to throw an exception.");
            }
            catch (ArgumentException e) {}
            Assert.AreEqual(0, table.KeysetCount);

            try
            {
                table.AddKeyset("a", null);
                Assert.Fail("The statement was supposed to throw an exception.");
            }
            catch (ArgumentNullException e) {}
            Assert.AreEqual(0, table.KeysetCount);

            // Add a proper keyset first
            table.AddKeyset("Id", d => d.Id.ToString());
            Assert.AreEqual(1, table.KeysetCount);

            try
            {
                table.AddKeyset("Id", d => d.Name);
                Assert.Fail("The statement was supposed to throw an exception.");
            }
            catch (DuplicateKeyException e)
            {
            }
            Assert.AreEqual(1, table.KeysetCount);
        }

        [Test]
        public void TestAdd()
        {
            var table = new MultiKeyTable<Dummy>();
            var dummy1 = new Dummy("a", 0);

            // Adding value first then add keyset

            table.Add(dummy1);
            Assert.AreEqual(1, table.Count);
            Assert.AreEqual(0, table.KeysetCount);
            Assert.IsNull(table.Get("Name", "a"));

            table.AddKeyset("Name", d => d.Name);
            Assert.AreEqual(1, table.Count);
            Assert.AreEqual(1, table.KeysetCount);

            Assert.AreSame(dummy1, table.Get("Name", "a"));
            Assert.IsNull(table.Get("Nameasdf", "a"));
            Assert.IsNull(table.Get("Name", "asdf"));

            // Adding keyset first then add value.

            table = new MultiKeyTable<Dummy>();
            Assert.AreEqual(0, table.KeysetCount);
            Assert.AreEqual(0, table.Count);

            table.AddKeyset("Passcode", d => d.Passcode.ToString());
            Assert.AreEqual(1, table.KeysetCount);
            Assert.AreEqual(0, table.Count);

            table.Add(dummy1);
            Assert.AreEqual(1, table.KeysetCount);
            Assert.AreEqual(1, table.Count);
            Assert.AreSame(dummy1, table.Get("Passcode", "0"));
            Assert.IsNull(table.Get("Passcode1", "0"));
            Assert.IsNull(table.Get("Passcode", "1"));
        }

        [Test]
        public void TestContains()
        {
            var table = new MultiKeyTable<Dummy>();
            var dummy = new Dummy("asdf", 50);

            // Adding value first

            Assert.IsFalse(table.Contains("Name", "asdf"));

            table.Add(dummy);
            Assert.IsFalse(table.Contains("Name", "asdf"));

            table.AddKeyset("Name", d => d.Name);
            Assert.IsTrue(table.Contains("Name", "asdf"));

            // Adding keyset first

            table = new MultiKeyTable<Dummy>();
            Assert.IsFalse(table.Contains("Name", "asdf"));

            table.AddKeyset("Name", d => d.Name);
            Assert.IsFalse(table.Contains("Name", "asdf"));

            table.Add(dummy);
            Assert.IsTrue(table.Contains("Name", "asdf"));
        }

        [Test]
        public void TestClear()
        {
            var table = new MultiKeyTable<Dummy>();
            for (int i = 0; i < 5; i++)
                table.Add(new Dummy(i.ToString(), i));
            Assert.AreEqual(5, table.Count);

            table.AddKeyset("Id", v => v.Id.ToString());
            table.AddKeyset("Name", v => v.Name);
            table.AddKeyset("Passcode", v => v.Passcode.ToString());
            Assert.AreEqual(3, table.KeysetCount);

            table.Clear();
            Assert.AreEqual(0, table.Count);
            Assert.AreEqual(3, table.KeysetCount);
        }

        [Test]
        public void TestRemove()
        {
            var table = new MultiKeyTable<Dummy>();
            var dummies = new Dummy[] {
                new Dummy("A", 0),
                new Dummy("B", 1),
                new Dummy("C", 2)
            };
            dummies.ForEach(d => table.Add(d));
            Assert.AreEqual(dummies.Length, table.Count);

            table.AddKeyset("Id", v => v.Id.ToString());
            table.AddKeyset("Name", v => v.Name);
            table.AddKeyset("Passcode", v => v.Passcode.ToString());
            Assert.AreEqual(3, table.KeysetCount);

            dummies.ForEach(d => Assert.AreSame(d, table.Get("Id", d.Id.ToString())));
            dummies.ForEach(d => Assert.AreSame(d, table.Get("Name", d.Name.ToString())));
            dummies.ForEach(d => Assert.AreSame(d, table.Get("Passcode", d.Passcode.ToString())));

            table.Remove(dummies[1]);

            dummies.ForEach(d => {
                if(d == dummies[1])
                    Assert.IsNull(table.Get("Id", d.Id.ToString()));
                else
                    Assert.AreSame(d, table.Get("Id", d.Id.ToString()));
            });
            dummies.ForEach(d => {
                if(d == dummies[1])
                    Assert.IsNull(table.Get("Name", d.Name));
                else
                    Assert.AreSame(d, table.Get("Name", d.Name));
            });
            dummies.ForEach(d => {
                if(d == dummies[1])
                    Assert.IsNull(table.Get("Passcode", d.Passcode.ToString()));
                else
                    Assert.AreSame(d, table.Get("Passcode", d.Passcode.ToString()));
            });
            Assert.AreEqual(dummies.Length - 1, table.Count);
        }

        private class Dummy
        {
            public readonly Guid Id;
            public readonly String Name;
            public readonly int Passcode;

            public Dummy(string name, int passcode)
            {
                Id = Guid.NewGuid();
                Name = name;
                Passcode = passcode;
            }
        }
    }
}