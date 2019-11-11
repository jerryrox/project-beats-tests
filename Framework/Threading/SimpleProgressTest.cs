using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Tests
{
    public class SimpleProgressTest {

        private const float Delta = 0.00000001f;


        [Test]
        public void Test()
        {
            SimpleProgress progress = new SimpleProgress();
            Assert.AreEqual(0f, progress.Progress, Delta);

            progress.Report(0.55f);
            Assert.AreEqual(0.55f, progress.Progress, Delta);
        }
    }
}