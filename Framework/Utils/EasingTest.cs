using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Utils.Tests
{
    public class EasingTest
    {
        private const float MaxDelta = 0.0001f;

        [Test]
        public void TestLinear()
        {
            Assert.AreEqual(-1, Easing.Linear(0, -1, 1, 0), MaxDelta);

            Assert.AreEqual(-0.5f, Easing.Linear(0.5f, -1, 1, 0), MaxDelta);

            Assert.AreEqual(0, Easing.Linear(1, -1, 1, 0), MaxDelta);
        }

        [Test]
        public void TestEase()
        {
            Assert.AreEqual(0, Easing.Ease(0, 0, 2, 0, EaseType.QuadEaseOut), MaxDelta);
            
            Assert.AreEqual(1.5f, Easing.Ease(0.5f, 0, 2, 0, EaseType.QuadEaseOut), MaxDelta);
            
            Assert.AreEqual(2, Easing.Ease(1, 0, 2, 0, EaseType.QuadEaseOut), MaxDelta);
        }
    }
}
