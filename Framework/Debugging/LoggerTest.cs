using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Logger = PBFramework.Debugging.Logger;

namespace PBFramework.Debugging
{
    public class LoggerTest {
        
        [Test]
        public void TestLogger()
        {
            LogAssert.ignoreFailingMessages = true;

            Logger.Log("1");
            Logger.LogWarning("2");
            Logger.LogError("3");

            Logger.OnWarning += (message) =>
            {
                Logger.Log($"Dispatched warning: {message}");
            };
            Logger.OnError += (message) =>
            {
                Logger.Log($"Dispatched error: {message}");
            };

            Logger.Log("a");
            Logger.LogWarning("b");
            Logger.LogError("c");
        }

        [Test]
        public void TestDummyLogger()
        {
            LogAssert.ignoreFailingMessages = true;
            
            var dummy = new DummyLogService();

            Logger.Register(dummy);
            
            Assert.IsFalse(dummy.LoggedNormal);
            Assert.IsFalse(dummy.LoggedWarning);
            Assert.IsFalse(dummy.LoggedError);

            Logger.Log("AA");
            Logger.LogWarning("BB");
            try
            {
                Logger.LogError("CC");
            }
            catch(Exception) {}

            Assert.IsTrue(dummy.LoggedNormal);
            Assert.IsTrue(dummy.LoggedWarning);
            Assert.IsTrue(dummy.LoggedError);
        }

        private class DummyLogService : ILogService {

            public bool LoggedNormal = false;
            public bool LoggedWarning = false;
            public bool LoggedError = false;

            public void Log(object message) => LoggedNormal = true;
            public void LogWarning(object message) => LoggedWarning = true;
            public void LogError(object message) => LoggedError = true;
        }
    }
}