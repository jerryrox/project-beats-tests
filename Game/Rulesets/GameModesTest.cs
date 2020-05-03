using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.Rulesets.Tests
{
    public class GameModesTest {
        
        [Test]
        public void Test()
        {
            Assert.AreEqual(0, GameModeType.OsuStandard.GetIndex());
            Assert.AreEqual(0, GameModeType.BeatsStandard.GetIndex());
        }
    }
}