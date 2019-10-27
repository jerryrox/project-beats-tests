using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Utils.Tests
{
    public class MathUtilsTest {

        private const float Delta = 0.00001f;

        [Test]
        public void TestClamp()
        {
            Assert.AreEqual(0, MathUtils.Clamp(-100, 0, 1000), Delta);
            Assert.AreEqual(1, MathUtils.Clamp(1, 0, 2), Delta);
            Assert.AreEqual(2, MathUtils.Clamp(100, 0, 2), Delta);
        }

        [Test]
        public void TestInverseLerp()
        {
            Assert.AreEqual(0, MathUtils.InverseLerp(0, 100, 0), Delta);
            Assert.AreEqual(1, MathUtils.InverseLerp(0, 100, 100), Delta);
            Assert.AreEqual(0.5, MathUtils.InverseLerp(0, 100, 50), Delta);
        }

        [Test]
        public void TestAlmostEquals()
        {
            Assert.IsTrue(MathUtils.AlmostEquals(0.5, 2.0 / 4.0));
            Assert.IsTrue(!MathUtils.AlmostEquals(0.5, 2.1 / 4.0));
        }
    }
}