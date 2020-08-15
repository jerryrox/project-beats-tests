using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Tests
{
    public class SynchronizedBoolTest {
        
        [Test]
        public void Test()
        {
            Assert.IsFalse(new SynchronizedBool(false).Value);
            Assert.IsTrue(new SynchronizedBool(true).Value);

            SynchronizedBool sync = new SynchronizedBool(false);
            sync.Value = true;
            Assert.IsTrue(sync.Value);
            sync.Value = true;
            Assert.IsTrue(sync.Value);
            sync.Value = false;
            Assert.IsFalse(sync.Value);
            sync.Value = true;
            Assert.IsTrue(sync.Value);
        }
    }
}