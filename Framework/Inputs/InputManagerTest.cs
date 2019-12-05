using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Inputs.Tests
{
    public class InputManagerTest {
        
        [UnityTest]
        public IEnumerator TestMouse()
        {
            var environment = InputTestEnvironment.Create();
            var inputManager = InputManager.Create(environment.Resolution, 2, 0);

            environment.SetInputManager(inputManager);
            environment.SetMouses(inputManager.GetMouses().Cast<MouseCursor>());
            environment.ListenToState(inputManager.GetMouses().Cast<IInput>());

            while (environment.IsRunning)
            {
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator TestKeyboard()
        {
            var environment = InputTestEnvironment.Create();
            var inputManager = InputManager.Create(environment.Resolution, 0, 0);

            environment.SetInputManager(inputManager);
            environment.ListenToState(inputManager.GetKeys().Cast<IInput>());

            while (environment.IsRunning)
            {
                if (Input.GetKey(KeyCode.RightShift))
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        inputManager.RemoveKey(KeyCode.A);
                        Debug.Log("Removed key A");
                    }
                    if(Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        inputManager.RemoveKey(KeyCode.S);
                        Debug.Log("Removed key S");
                    }
                    if(Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        inputManager.RemoveKey(KeyCode.D);
                        Debug.Log("Removed key D");
                    }
                    if(Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        inputManager.RemoveKey(KeyCode.F);
                        Debug.Log("Removed key F");
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        inputManager.AddKey(KeyCode.A);
                        environment.ListenToState(inputManager.GetKeys().Cast<IInput>());
                        Debug.Log("Added key A");
                    }
                    if(Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        inputManager.AddKey(KeyCode.S);
                        environment.ListenToState(inputManager.GetKeys().Cast<IInput>());
                        Debug.Log("Added key S");
                    }
                    if(Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        inputManager.AddKey(KeyCode.D);
                        environment.ListenToState(inputManager.GetKeys().Cast<IInput>());
                        Debug.Log("Added key D");
                    }
                    if(Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        inputManager.AddKey(KeyCode.F);
                        environment.ListenToState(inputManager.GetKeys().Cast<IInput>());
                        Debug.Log("Added key F");
                    }
                }
                yield return null;
            }
        }
    }
}