using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Testing;
using PBFramework.Threading;

namespace PBGame.Tests
{
    public class TestGame : ProjectBeatsGame
    {
        /// <summary>
        /// Returns the testing environment instance used by this game.
        /// </summary>
        public TestEnvironment Environment { get; private set; }

        public override bool ShouldShowFirstView => false;

        public override bool IsTestMode => true;


        /// <summary>
        /// Starts a new testing session for PBGame.
        /// </summary>
        public static TestGame Setup<T>(T tester, TestOptions testOptions = null)
            where T : class
        {
            TestGame game = new GameObject("TestGame").AddComponent<TestGame>();

            // Rebuild test options to inject internal dependencies.
            testOptions = game.CreateTestOptions(testOptions);

            game.Environment = TestEnvironment.Setup(tester, testOptions);
            return game;
        }

        /// <summary>
        /// Handles the actual running procedure for testing the PBGame side.
        /// </summary>
        public IEnumerator Run() => Environment.Run();

        /// <summary>
        /// Returns an enumerator which awaits until the specified progress has finished.
        /// </summary>
        public IEnumerator AwaitProgress(IEventProgress progress)
        {
            bool isFinished = false;
            progress.OnFinished += () => isFinished = true;
            while (!isFinished)
                yield return null;
        }

        /// <summary>
        /// Generates a new test environment option specifically for testing the game.
        /// </summary>
        private TestOptions CreateTestOptions(TestOptions customOptions)
        {
            if (customOptions == null)
            {
                customOptions = new TestOptions()
                {
                    Dependency = base.Dependencies,
                    UseCamera = true,
                    CleanupMethod = OnTestEnvironmentCleanup,
                };
            }
            else
            {
                // Ensure dependencies include all those used in the game.
                if(customOptions.Dependency == null)
                    customOptions.Dependency = base.Dependencies;
                else
                    customOptions.Dependency.CacheFrom(base.Dependencies, true);

                // Enforce camera use
                customOptions.UseCamera = true;

                // Hook custom cleanup method.
                Action customCleanupMethod = customOptions.CleanupMethod;
                customOptions.CleanupMethod = () =>
                {
                    customCleanupMethod?.Invoke();
                    OnTestEnvironmentCleanup();
                };
            }
            return customOptions;
        }

        /// <summary>
        /// Event called on cleaning up the test environment.
        /// </summary>
        private void OnTestEnvironmentCleanup()
        {
            GameObject.Destroy(gameObject);
        }
    }
}