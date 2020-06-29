using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.Configurations
{
    public class EnvConfigurationTest {
        
        [Test]
        public void TestDev()
        {
            IEnvConfiguration config = new EnvConfiguration(true);
            Assert.IsTrue(config.IsDevelopment);
            Assert.IsFalse(config.IsLoaded);
            Assert.IsNull(config.Variables);

            config.Load("Configurations/Env");

            Assert.IsTrue(config.IsLoaded);
            Assert.IsNotNull(config.Variables);
            Assert.IsTrue(config.Variables.IsDevelopment);
        }

        [Test]
        public void TestProd()
        {
            IEnvConfiguration config = new EnvConfiguration(false);
            Assert.IsFalse(config.IsDevelopment);
            Assert.IsFalse(config.IsLoaded);
            Assert.IsNull(config.Variables);

            config.Load("Configurations/Env");

            Assert.IsTrue(config.IsLoaded);
            Assert.IsNotNull(config.Variables);
            Assert.IsFalse(config.Variables.IsDevelopment);
        }

        [Test]
        public void TestFailLoad()
        {
            IEnvConfiguration config = new EnvConfiguration(true);
            try
            {
                config.Load("fake/path/lolz");
                Assert.Fail();
            }
            catch (Exception)
            {
            }
            Assert.IsNull(config.Variables);
        }
    }
}