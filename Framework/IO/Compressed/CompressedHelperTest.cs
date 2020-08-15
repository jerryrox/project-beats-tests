using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.IO.Compressed.Tests
{
    public class CompressedHelperTest {
        
        [Test]
        public void TestGetCompressed()
        {
            string realPath = Path.Combine(TestConstants.TestAssetPath, "TestZip.ZiP");
            string fakePath = Path.Combine(TestConstants.TestAssetPath, "TestZip.lol");

            var compressed = CompressedHelper.GetCompressed(new FileInfo(realPath));
            Assert.IsTrue(compressed is ZipCompressed);

            compressed = CompressedHelper.GetCompressed(new FileInfo(fakePath));
            Assert.IsNull(compressed);

            compressed = CompressedHelper.GetCompressed(new FileInfo(fakePath), true);
            Assert.IsTrue(compressed is DefaultCompressed);
        }
    }
}