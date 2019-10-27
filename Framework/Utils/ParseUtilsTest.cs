using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Utils.Tests
{
    public class ParseUtilsTest {
        
        [Test]
        public void TestParse()
        {
            Assert.AreEqual(5, ParseUtils.ParseInt("5"));
            Assert.AreEqual(10, ParseUtils.ParseInt("sdvavd", 10));
        }
    }
}