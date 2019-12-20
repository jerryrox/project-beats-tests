using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using PBFramework.Assets.Atlasing;
using PBFramework.Graphics.UI;
using PBFramework.Graphics.UI.Tests;
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