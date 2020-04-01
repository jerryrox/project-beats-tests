using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;

namespace PBGame.Tests
{
    public class TestGame : ProjectBeatsGame
    {
        private static TestGame Game;

        private Func<IEnumerator> onInit;
        private Action onUpdate;
        private Func<IEnumerator> onDispose;

        private bool isRunning = true;
        private bool shouldFail = false;


        public override bool ShouldShowFirstView => false;


        /// <summary>
        /// Initializes a new game test environment and returns a yieldable coroutine for UnityTest cases.
        /// </summary>
        public static Coroutine Run<T>(T tester, Func<IEnumerator> onInit = null, Action onUpdate = null, Func<IEnumerator> onDispose = null)
            where T : class
        {
            if(Game != null)
                throw new Exception("TestGame instance is already initialized!");

            Camera camera = new GameObject("camera").AddComponent<Camera>();
            camera.gameObject.AddComponent<AudioListener>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;

            Game = new GameObject("TestGame").AddComponent<TestGame>();

            Debug.LogWarning("==============================");
            Debug.LogWarning("Test game initialized.");
            Debug.LogWarning("Press Cmd+1 to Succeed test");
            Debug.LogWarning("Press Cmd+2 to Fail test");
            Debug.LogWarning("==============================");

            if(tester != null)
                Game.Dependencies.Inject(tester);

            Game.onInit = onInit;
            Game.onUpdate = onUpdate;
            Game.onDispose = onDispose;

            return Game.StartCoroutine(Game.TestRoutine());
        }

        /// <summary>
        /// Waits for the specified progress to be finished.
        /// </summary>
        public static Coroutine AwaitProgress(IEventProgress progress)
        {
            return Game.StartCoroutine(Game.ProgressYieldRoutine(progress));
        }

        /// <summary>
        /// Handles the process for awaiting the specified progress.
        /// </summary>
        private IEnumerator ProgressYieldRoutine(IEventProgress progress)
        {
            bool isFinished = false;
            progress.OnFinished += () => isFinished = true;
            while (!isFinished)
                yield return null;
        }

        /// <summary>
        /// Handles the test case lifecycle.
        /// </summary>
        private IEnumerator TestRoutine()
        {
            if (onInit != null)
                yield return onInit.Invoke();

            while (isRunning)
            {
                onUpdate?.Invoke();
                yield return null;
            }

            if(onDispose != null)
                yield return onDispose.Invoke();

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