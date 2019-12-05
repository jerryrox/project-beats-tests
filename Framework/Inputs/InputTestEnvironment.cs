using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Inputs.Tests
{
    public class InputTestEnvironment : MonoBehaviour {

        private List<IInput> activatables = new List<IInput>();


        public bool IsActivated { get; private set; } = true;

        public bool IsRunning { get; private set; } = true;

        public Vector2 Resolution { get; set; } = new Vector2(1280f, 720f);

        public static InputTestEnvironment Create()
        {
            return new GameObject("InputTestEnv").AddComponent<InputTestEnvironment>();
        }

        void Awake()
        {
            Debug.LogWarning("Press Esc to fail.");
            Debug.LogWarning("Press Enter to succeed.");
        }

        public void SetActivatable(IEnumerable<IInput> inputs)
        {
            foreach(var input in inputs)
                activatables.Add(input);
        }

        public void ListenToState(IEnumerable<IInput> inputs)
        {
            int i = 0;
            foreach (var input in inputs)
            {
                int index = i;
                input.State.OnValueChanged += (newValue, oldValue) =>
                {
                    Debug.LogWarning($"Input {index} changed state to ({newValue}) from ({oldValue})");
                };
                i++;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Assert.Fail("Manually failed test.");
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                IsRunning = false;
            }

            IsActivated = !Input.GetKey(KeyCode.LeftShift);
            for (int i = 0; i < activatables.Count; i++)
            {
                if (IsActivated)
                {
                    if (!activatables[i].IsActive.Value)
                    {
                        activatables[i].SetActive(true);
                        Debug.LogWarning($"Input {activatables[i].Key} activated");
                    }
                    Assert.IsTrue(activatables[i].IsActive.Value);
                }
                else
                {
                    if (activatables[i].IsActive.Value)
                    {
                        activatables[i].SetActive(false);
                        activatables[i].Release();
                        Debug.LogWarning($"Input {activatables[i].Key} deactivated");
                    }
                    Assert.IsFalse(activatables[i].IsActive.Value);
                }
            }
        }
    }
}