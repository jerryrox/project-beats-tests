using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Utils;

namespace PBFramework.Animations.Sections.Tests
{
    public class TimeFrameTest {

        private const float Delta = 0.00001f;


        [Test]
        public void Test()
        {
            var timeFrame = new TimeFrame<float>(0f, () => 0f, EaseType.Linear);
            var timeFrame2 = new TimeFrame<float>(1f, () => 10f, EaseType.Linear);
            var timeFrame3 = new TimeFrame<float>(2f, () => 30f, EaseType.Linear);

            timeFrame.Link(timeFrame2);
            timeFrame2.Link(timeFrame3);

            Assert.AreEqual(0f, timeFrame.GetInterpolant(0), Delta);
            Assert.AreEqual(0.5f, timeFrame.GetInterpolant(0.5f), Delta);
            Assert.AreEqual(1f, timeFrame.GetInterpolant(1f), Delta);
            Assert.AreEqual(2f, timeFrame.GetInterpolant(2f), Delta);

            Assert.AreEqual(0f, timeFrame2.GetInterpolant(1), Delta);
            Assert.AreEqual(0.5f, timeFrame2.GetInterpolant(1.5f), Delta);
            Assert.AreEqual(1f, timeFrame2.GetInterpolant(2f), Delta);
            Assert.AreEqual(2f, timeFrame2.GetInterpolant(3f), Delta);
        }
    }
}