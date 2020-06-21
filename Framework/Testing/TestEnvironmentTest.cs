using System;
using System.Collections;
using System.Collections.Generic;
using PBFramework.Graphics;
using PBFramework.Dependencies;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace PBFramework.Testing.Tests
{
    public class TestEnvironmentTest {

        [UnityTest]
        public IEnumerator TestSuccessCase()
        {
            TestOptions options = new TestOptions()
            {
                UseCamera = false
            };
            DummyAgent agent = new DummyAgent(options);
            yield return DoGeneralRoutine(agent);
            yield return DoResultRoutine(agent, true);
        }

        [UnityTest]
        public IEnumerator TestFailCase()
        {
            TestOptions options = new TestOptions()
            {
                UseCamera = false
            };
            DummyAgent agent = new DummyAgent(options);
            yield return DoGeneralRoutine(agent);
            yield return DoResultRoutine(agent, false);
        }

        [UnityTest]
        public IEnumerator TestCamera()
        {
            TestOptions options = new TestOptions()
            {
                UseCamera = true
            };
            DummyAgent agent = new DummyAgent(options);
            yield return DoGeneralRoutine(agent);
            yield return DoResultRoutine(agent, true);
        }

        [UnityTest]
        public IEnumerator TestRootCreation()
        {
            TestOptions options = new TestOptions()
            {
                DefaultRoot = new DefaultRootOptions()
                {
                    BaseResolution = new Vector2(640f, 640f)
                }
            };
            DummyAgent agent = new DummyAgent(options);
            yield return DoGeneralRoutine(agent);
            yield return DoResultRoutine(agent, true);
        }

        [UnityTest]
        public IEnumerator TestRootCreationWithCamera()
        {
            TestOptions options = new TestOptions()
            {
                UseCamera = true,
                DefaultRoot = new DefaultRootOptions()
                {
                    BaseResolution = new Vector2(640f, 640f)
                }
            };
            DummyAgent agent = new DummyAgent(options);
            yield return DoGeneralRoutine(agent);
            yield return DoResultRoutine(agent, true);
        }

        [UnityTest]
        public IEnumerator TestWithoutUpdate()
        {
            DummyAgent agent = new DummyAgent(new TestOptions());
            agent.Options.UpdateMethod = null;
            yield return DoGeneralRoutine(agent);
        }

        [UnityTest]
        public IEnumerator TestWithKeyBinding()
        {
            DummyAgent agent = new DummyAgent(new TestOptions());
            agent.Options.KeyAction = new KeyActionOptions()
            {
                KeyBindings = new TestKeyBinding[]
                {
                    new TestKeyBinding(KeyCode.A, (isAuto) => agent.KeyBoundAction(), "Lol")
                }
            };
            yield return DoGeneralRoutine(agent);
            yield return DoResultRoutine(agent, true);
        }

        private IEnumerator DoGeneralRoutine(DummyAgent agent)
        {
            Assert.IsTrue(agent.Environment.IsRunning);
            Assert.IsTrue(agent.IsInitCalled);
            Assert.IsFalse(agent.IsUpdateCalled);
            Assert.IsFalse(agent.IsCleanupCalled);

            if (agent.Options.UseCamera)
            {
                Camera camera = Camera.main;
                Assert.IsNotNull(camera);
                Assert.IsNotNull(camera.GetComponent<AudioListener>());
            }

            if (agent.Options.DefaultRoot != null)
            {
                IRoot root = agent.Dependency.Get<IRoot>();
                Assert.IsNotNull(root);
                if(agent.Options.UseCamera)
                    Assert.AreEqual(Camera.main, root.Cam);
                else
                    Assert.IsNull(root.Cam);
                Assert.AreEqual(new Vector2(640f, 640f), root.BaseResolution);
            }

            // Using StartCoroutine here is a bit of a hack to bypass current testing limitations.
            agent.Environment.StartCoroutine(agent.Run());
            Assert.IsTrue(agent.IsInitCalled);

            if (agent.Options.UpdateMethod != null)
            {
                Assert.IsTrue(agent.IsUpdateCalled);
                Assert.AreEqual(1, agent.UpdateCalls);
                Assert.IsFalse(agent.IsCleanupCalled);

                // Test whether update is initially called after a frame.
                yield return null;
                Assert.IsTrue(agent.IsInitCalled);
                Assert.IsTrue(agent.IsUpdateCalled);
                Assert.AreEqual(2, agent.UpdateCalls);
                Assert.IsFalse(agent.IsCleanupCalled);

                // Test whether update method is constantly called every frame.
                agent.IsUpdateCalled = false;
                yield return null;
                Assert.IsTrue(agent.IsInitCalled);
                Assert.IsTrue(agent.IsUpdateCalled);
                Assert.AreEqual(3, agent.UpdateCalls);
                Assert.IsFalse(agent.IsCleanupCalled);
            }
            else if (agent.Options.KeyAction != null)
            {
                Assert.IsFalse(agent.IsKeyActionCalled);
                yield return null;
                Assert.IsTrue(agent.IsKeyActionCalled);
                Assert.IsFalse(agent.IsCleanupCalled);
            }
            else
            {
                Assert.IsFalse(agent.Environment.IsRunning);
                Assert.IsTrue(agent.IsCleanupCalled);
            }
        }

        private IEnumerator DoResultRoutine(DummyAgent agent, bool shouldSucceed)
        {
            // Test whether stopping actually stops running the environment.
            Assert.IsTrue(agent.Environment.IsRunning);
            try
            {
                agent.Stop(shouldSucceed);
                if(!shouldSucceed)
                    Assert.Fail("Failing test should've thrown an exception.");
            }
            catch (Exception e)
            {
                if(shouldSucceed)
                    Assert.Fail("Successful test shoudn't have thrown an exception.");
                Assert.AreEqual($"[Test manually failed] : {DummyAgent.FailReason}", e.Message);
                yield break;
            }

            // Only successful results can enter this area now.

            Assert.IsFalse(agent.Environment.IsRunning);
            // Stopping doesn't immediately terminate the environment.
            // It needs to be processed within the coroutine which the UnityTest has invoked so the success/fail indication is shown properly in the editor.
            Assert.IsFalse(agent.IsCleanupCalled);

            // Attempting to run or end should be ignored.
            agent.Run();
            Assert.IsFalse(agent.Environment.IsRunning);
            // Shouldn't throw an exception because the test is already finished.
            agent.Stop(false);

            // Test whether cleanup is called.
            yield return null;
            Assert.IsTrue(agent.IsCleanupCalled);
        }


        public class DummyAgent
        {
            public const string FailReason = "Failed";

            public TestOptions Options;
            public TestEnvironment Environment;

            public bool IsInitCalled;
            public bool IsUpdateCalled;
            public int UpdateCalls;
            public bool IsCleanupCalled;
            public bool IsKeyActionCalled;


            [ReceivesDependency]
            public IDependencyContainer Dependency { get; set; }


            public DummyAgent(TestOptions options)
            {
                this.Options = options;
                options.UpdateMethod = TestUpdate;
                options.CleanupMethod = TestCleanup;

                Environment = TestEnvironment.Setup(this, Options);
            }

            [InitWithDependency]
            private void Init()
            {
                Assert.IsFalse(IsUpdateCalled);
                Assert.IsFalse(IsCleanupCalled);

                IsInitCalled = true;
                Assert.IsNotNull(Dependency);
            }

            public IEnumerator Run() => Environment.Run();

            public void Stop(bool isSuccess)
            {
                if(isSuccess)
                    Environment.EndSuccess();
                else
                    Environment.EndFail(FailReason);
            }

            public IEnumerator KeyBoundAction()
            {
                yield return null;
                IsKeyActionCalled = true;
            }

            private void TestUpdate()
            {
                Assert.IsTrue(Environment.IsRunning);
                Assert.IsFalse(IsCleanupCalled);
                Assert.IsTrue(IsInitCalled);
                IsUpdateCalled = true;
                UpdateCalls++;
            }

            private void TestCleanup()
            {
                Assert.IsFalse(Environment.IsRunning);
                Assert.IsTrue(IsInitCalled);
                if(Options.UpdateMethod != null)
                    Assert.IsTrue(IsUpdateCalled);
                IsCleanupCalled = true;

                if (Camera.main != null)
                {
                    GameObject.DestroyImmediate(Camera.main.gameObject);
                }
                if (Dependency.Contains<IRoot>())
                {
                    GameObject.DestroyImmediate(Dependency.Get<IRoot>().RawObject);
                }
            }
        }
    }
}