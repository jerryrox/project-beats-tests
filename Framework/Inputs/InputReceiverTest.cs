using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using PBFramework.Assets.Atlasing;
using PBFramework.Graphics;
using PBFramework.Graphics.UI;
using PBFramework.Graphics.UI.Tests;
using PBFramework.Graphics.Tests;
using PBFramework.Dependencies;

namespace PBFramework.Inputs.Tests
{
    public class InputReceiverTest {
        
        [UnityTest]
        public IEnumerator TestRaw()
        {
            var inputManager = InputManager.Create(new Vector2(1280f, 720f));

            var dependency = new DependencyContainer();
            dependency.CacheAs<IDependencyContainer>(dependency);
            //dependency.CacheAs<IInputManager>(InputManager.Create(new Vector2(1280f, 720f)));

            var qReceiver = new DummyReceiver()
            {
                Key = inputManager.AddKey(KeyCode.Q),
                InputLayer = 2,
                ShouldPass = true,
            };
            var wReceiver = new DummyReceiver()
            {
                Key = inputManager.AddKey(KeyCode.W),
                InputLayer = 1,
                ShouldPass = false,
            };
            
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if(qReceiver.Added)
                        inputManager.RemoveReceiver(qReceiver);
                    else
                        inputManager.AddReceiver(qReceiver);

                    qReceiver.Added = !qReceiver.Added;
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    if(wReceiver.Added)
                        inputManager.RemoveReceiver(wReceiver);
                    else
                        inputManager.AddReceiver(wReceiver);

                    wReceiver.Added = !wReceiver.Added;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    qReceiver.ShouldPass = !qReceiver.ShouldPass;
                    Debug.Log("Passing from Q : " + qReceiver.ShouldPass);
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    wReceiver.ShouldPass = !wReceiver.ShouldPass;
                    Debug.Log("Passing from W : " + wReceiver.ShouldPass);
                }


                if (Input.GetKeyDown(KeyCode.Return))
                {
                    break;
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Assert.Fail();
                }

                yield return null;
            }
        }

        /*

GUI Test scenario

Root (0)
- Obj (0)
- Canvas (1)
- - Obj (0)
- Obj (4)
- - Obj (1)
- - - Obj (0)
- - Obj (2)
- Obj (5)
- - Obj (100)

        */

        [UnityTest]
        public IEnumerator TestUgui()
        {
            var inputManager = InputManager.Create(new Vector2(1280f, 720f));
            var dependency = new DependencyContainer(true);
            dependency.CacheAs<IInputManager>(inputManager);

            var receivers = new DummyUguiObject[9];

            Action<int> assertReceivedUntil = (int lastReceivedIndex) =>
            {
                for(int i=0; i<receivers.Length; i++)
                {
                    if(i <= lastReceivedIndex)
                        Assert.IsTrue(receivers[i].DidUpdate);
                    else
                        Assert.IsFalse(receivers[i].DidUpdate);
                }
            };

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependency);
            {
                var base0 = root.CreateChild<DummyUguiObject>("base0", 0);
                {
                    base0.SetReceiveInputs(true);
                    receivers[8] = base0;
                }
                var base1 = root.CreateChild<DummyUguiObject>("base1", 1);
                {
                    base1.Canvas = base1.RawObject.AddComponent<Canvas>();
                    base1.Canvas.overrideSorting = true;
                    base1.Canvas.sortingOrder = 1;
                    base1.SetReceiveInputs(true);
                    receivers[1] = base1;

                    var obj0 = base1.CreateChild<DummyUguiObject>("obj0", 0);
                    {
                        obj0.SetReceiveInputs(true);
                        receivers[0] = obj0;
                    }
                }
                var base2 = root.CreateChild<DummyUguiObject>("base2", 4);
                {
                    base2.SetReceiveInputs(true);
                    receivers[7] = base2;

                    var obj0 = base2.CreateChild<DummyUguiObject>("obj0", 1);
                    {
                        obj0.SetReceiveInputs(true);
                        receivers[6] = obj0;

                        var obj00 = obj0.CreateChild<DummyUguiObject>("obj00", 0);
                        {
                            obj00.SetReceiveInputs(true);
                            receivers[5] = obj00;
                        }
                    }
                    var obj1 = base2.CreateChild<DummyUguiObject>("obj1", 2);
                    {
                        obj1.SetReceiveInputs(true);
                        receivers[4] = obj1;
                    }
                }
                var base3 = root.CreateChild<DummyUguiObject>("base3", 5);
                {
                    base3.SetReceiveInputs(true);
                    receivers[3] = base3;

                    var obj0 = base3.CreateChild<DummyUguiObject>("obj0", 100);
                    {
                        obj0.SetReceiveInputs(true);
                        receivers[2] = obj0;
                    }
                }
            }

            assertReceivedUntil(-1);
            for (int i = 0; i < receivers.Length; i++)
            {
                yield return null;
                Debug.Log("checking index: " + i);
                assertReceivedUntil(i);
                receivers[i].SetReceiveInputs(false);
            }
        }

        private class DummyUguiObject : UguiObject
        {
            public bool DidUpdate { get; set; } = false;

            public override bool ProcessInput()
            {
                DidUpdate = true;
                return false;
            }
        }

        private class DummyReceiver : IInputReceiver
        {
            public int InputLayer { get; set; }

            public bool ShouldPass { get; set; } = true;

            public IKey Key { get; set; }

            public bool Added { get; set; }


            public void PrepareInputSort()
            {
                Debug.Log("Sort called for key: " + Key.Key);
            }

            public bool ProcessInput()
            {
                if (Key.State.Value == InputState.Press)
                {
                    Debug.Log($"Key ({Key.Key}) pressed!");
                }

                return ShouldPass;
            }

            public int CompareTo(IInputReceiver other) => other.InputLayer.CompareTo(InputLayer);
        }
    }
}