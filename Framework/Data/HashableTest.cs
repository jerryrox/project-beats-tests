using System;
using System.Collections;
using NUnit.Framework;

namespace PBFramework.Data.Tests
{
    public class HashableTest
    {
        [Test]
        public void Test()
        {
            var dummy = new Dummy() {
                A = "asdf",
                B = 15
            };
            var dummy2 = new Dummy() {
                A = "asdf",
                B = 15
            };
            var dummy3 = new Dummy() {
                A = "basdf",
                B = 15
            };
            var dummy4 = new Dummy() {
                A = "asdf",
                B = 16
            };

            Assert.AreEqual(0, dummy.HashCode);

            dummy.CalculateHash();
            dummy2.CalculateHash();
            dummy3.CalculateHash();
            dummy4.CalculateHash();

            Assert.AreNotEqual(0, dummy.HashCode);
            Assert.AreEqual(dummy.HashCode, dummy2.HashCode);
            Assert.AreNotEqual(dummy.HashCode, dummy3.HashCode);
            Assert.AreNotEqual(dummy.HashCode, dummy4.HashCode);
            Assert.AreNotEqual(dummy3.HashCode, dummy4.HashCode);
        }

        private class Dummy : IHashable
        {
            public string A { get; set; }
            public int B { get; set; }

            public int HashCode { get; set; }

            public IEnumerable GetHashParams()
            {
                yield return A;
                yield return B;
            }
        }
    }
}
