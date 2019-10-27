using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Utils.Tests
{
    public class PathUtilsTest {
        
        [Test]
        public void TestStandardPath()
        {
            Assert.AreEqual("Testing/Path/Lolz", PathUtils.StandardPath("Testing\\Path/Lolz"));
        }

        [Test]
        public void TestRequestPath()
        {
            Assert.AreEqual("file://Test/Path/Lolz/A", PathUtils.LocalRequestPath("Test\\Path\\Lolz/A"));
        }

        [Test]
        public void TestNativePath()
        {
            char s = Path.DirectorySeparatorChar;
            string target = $"My{s}Test{s}Path";
            Assert.AreEqual(target, PathUtils.NativePath("My\\Test/Path"));
        }
    }
}