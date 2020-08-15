using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Utils.Tests
{
    public class IOUtilsTest {
        
        [Test]
        public void TestCopyDirectory()
        {
            var from = new DirectoryInfo(Path.Combine(TestConstants.TestAssetPath, "from"));
            var fromFile = new FileInfo(Path.Combine(from.FullName, "text"));
            var to = new DirectoryInfo(Path.Combine(TestConstants.TestAssetPath, "to"));
            var toFile = new FileInfo(Path.Combine(to.FullName, "text"));

            from.Create();
            File.WriteAllText(fromFile.FullName, "test");
            if (to.Exists)
            {
                to.Delete(true);
                to.Refresh();
                toFile.Refresh();
            }

            try
            {
                Assert.IsTrue(from.Exists);
                Assert.IsTrue(fromFile.Exists);
                Assert.AreEqual("test", File.ReadAllText(fromFile.FullName));
                Assert.IsFalse(to.Exists);
                Assert.IsFalse(toFile.Exists);

                IOUtils.CopyDirectory(from, to, false);
                to.Refresh();
                toFile.Refresh();
                Assert.IsTrue(from.Exists);
                Assert.IsTrue(fromFile.Exists);
                Assert.AreEqual("test", File.ReadAllText(fromFile.FullName));
                Assert.IsTrue(to.Exists);
                Assert.IsTrue(toFile.Exists);
                Assert.AreEqual("test", File.ReadAllText(toFile.FullName));

                File.WriteAllText(fromFile.FullName, "test2");
                try
                {
                    IOUtils.CopyDirectory(from, to, false);
                    Assert.Fail();
                }
                catch (Exception) {}
                Assert.IsTrue(from.Exists);
                Assert.IsTrue(fromFile.Exists);
                Assert.AreEqual("test2", File.ReadAllText(fromFile.FullName));
                Assert.IsTrue(to.Exists);
                Assert.IsTrue(toFile.Exists);
                Assert.AreEqual("test", File.ReadAllText(toFile.FullName));

                IOUtils.CopyDirectory(from, to, true);
                Assert.IsTrue(from.Exists);
                Assert.IsTrue(fromFile.Exists);
                Assert.AreEqual("test2", File.ReadAllText(fromFile.FullName));
                Assert.IsTrue(to.Exists);
                Assert.IsTrue(toFile.Exists);
                Assert.AreEqual("test2", File.ReadAllText(toFile.FullName));
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                throw e;
            }
            finally
            {
                from.Refresh();
                to.Refresh();
                if(from.Exists)
                    from.Delete(true);
                if(to.Exists)
                    to.Delete(true);
            }
        }
    }
}