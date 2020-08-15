using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.IO.Compressed.Tests
{
    public class ZipCompressedTest {

        [Test]
        public void TestSource()
        {
            var fileInfo = GetFileInfo();
            var zip = new ZipCompressed(fileInfo);
            Assert.AreEqual(fileInfo, zip.Source);
        }

        [Test]
        public void TestGetUncompressedSize()
        {
            var fileInfo = GetFileInfo();
            var zip = new ZipCompressed(fileInfo);
            long size = zip.GetUncompressedSize();

            Debug.Log("Returned size: " + size);
        }
        
        private FileInfo GetFileInfo()
        {
            string path = Path.Combine(TestConstants.TestAssetPath, "TestZip.zip");
            var source = new FileInfo(path);
            Assert.IsTrue(source.Exists);
            return source;
        }
    }
}