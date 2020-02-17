using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.Tests
{
    public class TestGame : ProjectBeatsGame
    {
        private Action onInit;
        private Action onUpdate;
        private Action onDispose;

        private bool isRunning = true;
        private bool shouldFail = false;


        /// <summary>
        /// Initializes a new game test environment and returns a yieldable coroutine for UnityTest cases.
        /// </summary>
        public static Coroutine Run(Action onInit = null, Action onUpdate = null, Action onDispose = null)
        {
            var testGame = new GameObject("TestGame").AddComponent<TestGame>();

            Debug.LogWarning("==============================");
            Debug.LogWarning("Test game initialized.");
            Debug.LogWarning("Press Cmd+1 to Succeed test");
            Debug.LogWarning("Press Cmd+2 to Fail test");
            Debug.LogWarning("==============================");

            testGame.onInit = onInit;
            testGame.onUpdate = onUpdate;
            testGame.onDispose = onDispose;

            return testGame.StartCoroutine(testGame.TestRoutine());
        }

        /// <summary>
        /// Handles the test case lifecycle.
        /// </summary>
        private IEnumerator TestRoutine()
        {
            onInit?.Invoke();

            while (isRunning)
            {
                onUpdate?.Invoke();
                yield return null;
            }

            onDispose?.Invoke();

            if(shouldFail)
                Assert.Fail("Manually failed test.");
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (Input.GetKey(KeyCode.LeftCommand))
                {
                    isRunning = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (Input.GetKey(KeyCode.LeftCommand))
                {
                    isRunning = false;
                    shouldFail = true;
                }
            }
        }
    }
}