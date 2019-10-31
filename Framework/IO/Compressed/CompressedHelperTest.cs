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
            string realPath = Path.Combine(Application.streamingAssetsPath, "TestZip.ZiP");
            string fakePath = Path.Combine(Application.streamingAssetsPath, "TestZip.lol");

            var compressed = CompressedHelper.GetCompressed(new FileInfo(realPath));
            Assert.IsTrue(compressed is ZipCompressed);

            compressed = CompressedHelper.GetCompressed(new FileInfo(fakePath));
            Assert.IsTrue(compressed is DefaultCompressed);
        }
    }
}