using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Inputs.Tests
{
    public class KeyboardKeyTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var environment = InputTestEnvironment.Create();
            var keys = new KeyboardKey[] {
                new KeyboardKey(KeyCode.A),
                new KeyboardKey(KeyCode.S),
                new KeyboardKey(KeyCode.D),
                new KeyboardKey(KeyCode.F),
            };

            environment.ListenToState(keys);
            environment.SetActivatable(keys);

            while (environment.IsRunning)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i].Process();
                }
                yield return null;
            }
        }
    }
}