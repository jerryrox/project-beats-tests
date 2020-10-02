using System;
using System.Collections;
using System.Collections.Generic;
using PBFramework.Dependencies;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace PBFramework.Testing.Tests
{
    public class TestEnvironmentKeyBindingTest {

        private bool autoTestedQ;
        private bool autoTestedW;
        private bool autoTestedE;


        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                Actions = new TestAction[]
                {
                    new TestAction(() => TestQ(true), "Tests Q automatically"),
                    new TestAction(() => TestE(true), "Tests E automatically"),
                    new TestAction(true, KeyCode.Q, () => TestQ(false), "Tests Q"),
                    new TestAction(true, KeyCode.W, () => TestW(false), "Tests W"),
                    new TestAction(true, KeyCode.E, () => TestE(false), "Tests E"),
                }
            };
            var environment = TestEnvironment.Setup(this, options);
            return environment.Run();
        }

        private IEnumerator TestQ(bool isAuto)
        {
            if (isAuto)
            {
                Assert.IsFalse(autoTestedQ);
                Assert.IsFalse(autoTestedW);
                Assert.IsFalse(autoTestedE);
                autoTestedQ = true;
            }
            else
            {
                Assert.IsTrue(autoTestedQ);
                Assert.IsFalse(autoTestedW);
                Assert.IsTrue(autoTestedE);
            }

            Assert.AreEqual(2, 1 + 1);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(5, 2 + 3);
            
            if(!isAuto)
                Debug.Log("TestQ");
        }

        private IEnumerator TestW(bool isAuto)
        {
            if (isAuto)
            {
                Assert.Fail("TestW shouldn't be called automatically.");
            }
            else
            {
                Assert.IsTrue(autoTestedQ);
                Assert.IsFalse(autoTestedW);
                Assert.IsTrue(autoTestedE);
            }

            Assert.AreEqual(3, 1 + 2);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(7, 4 + 3);

            if(!isAuto)
                Debug.Log("TestW");
        }

        private IEnumerator TestE(bool isAuto)
        {
            if (isAuto)
            {
                Assert.IsTrue(autoTestedQ);
                Assert.IsFalse(autoTestedW);
                Assert.IsFalse(autoTestedE);
                autoTestedE = true;
            }
            else
            {
                Assert.IsTrue(autoTestedQ);
                Assert.IsFalse(autoTestedW);
                Assert.IsTrue(autoTestedE);
            }

            Assert.AreEqual(3, 1 + 2);
            yield return null;
            Assert.AreEqual(7, 4 + 3);

            if(!isAuto)
                Debug.Log("TestE");
        }
    }
}