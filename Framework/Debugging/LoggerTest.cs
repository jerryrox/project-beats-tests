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

            Logger.LogVerbose("0");
            Logger.LogInfo("1");
            Logger.LogWarning("2");
            Logger.LogError("3");

            Logger.OnWarning += (message) =>
            {
                Logger.LogInfo($"Dispatched warning: {message}");
            };
            Logger.OnError += (message) =>
            {
                Logger.LogInfo($"Dispatched error: {message}");
            };

            Logger.LogVerbose("a");
            Logger.LogInfo("b");
            Logger.LogWarning("c");
            Logger.LogError("d");
        }

        [Test]
        public void TestDummyLogger()
        {
            LogAssert.ignoreFailingMessages = true;
            
            var dummy = new DummyLogService();

            Logger.Register(dummy);
            
            Assert.IsFalse(dummy.LoggedVerbose);
            Assert.IsFalse(dummy.LoggedInfo);
            Assert.IsFalse(dummy.LoggedWarning);
            Assert.IsFalse(dummy.LoggedError);

            Logger.LogInfo("AA");
            Logger.LogInfo("BB");
            Logger.LogWarning("CC");
            Logger.LogError("DD");

            Assert.IsTrue(dummy.LoggedVerbose);
            Assert.IsTrue(dummy.LoggedInfo);
            Assert.IsTrue(dummy.LoggedWarning);
            Assert.IsTrue(dummy.LoggedError);
        }

        private class DummyLogService : ILogService {

            public bool LoggedVerbose = false;
            public bool LoggedInfo = false;
            public bool LoggedWarning = false;
            public bool LoggedError = false;

            public void LogVerbose(object message) => LoggedVerbose = true;
            public void LogInfo(object message) => LoggedInfo = true;
            public void LogWarning(object message) => LoggedWarning = true;
            public void LogError(object message) => LoggedError = true;
        }
    }
}