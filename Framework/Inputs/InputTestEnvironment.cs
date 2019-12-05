using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Inputs.Tests
{
    public class InputTestEnvironment : MonoBehaviour {

        private const float Delta = 0.0001f;

        private List<IInput> activatables = new List<IInput>();
        private List<IInput> stateEmitters = new List<IInput>();

        private List<MouseCursor> mouses = new List<MouseCursor>();

        private List<TouchCursor> touches = new List<TouchCursor>();
        private uint touchUpdateId = 0;

        private IInputManager inputManager;


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

        public void SetInputManager(IInputManager inputManager)
        {
            this.inputManager = inputManager;
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
                if (stateEmitters.Contains(input))
                    continue;
                stateEmitters.Add(input);

                int index = i;
                input.State.OnValueChanged += (newValue, oldValue) =>
                {
                    Debug.LogWarning($"Input {input.Key} changed state to ({newValue}) from ({oldValue})");
                };
                i++;
            }
        }

        public void SetMouses(IEnumerable<MouseCursor> cursors)
        {
            mouses.AddEnumerable(cursors);
        }

        public void SetTouches(IEnumerable<TouchCursor> cursors)
        {
            touches.AddEnumerable(cursors);
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

            for (int i = 0; i < mouses.Count; i++)
            {
                if (IsActivated)
                {
                    if(inputManager == null)
                        mouses[i].Process();

                    if (i == 0 && Input.GetKey(KeyCode.LeftControl))
                    {
                        Debug.Log($"Raw pos: {mouses[i].RawPosition}, Delta: {mouses[i].RawDelta}");
                        Debug.Log($"Pos: {mouses[i].Position}, Delta: {mouses[i].Delta}");
                    }
                }

                if (i > 0)
                {
                    Assert.AreEqual(mouses[0].RawPosition.x, mouses[i].RawPosition.x, Delta);
                    Assert.AreEqual(mouses[0].RawPosition.y, mouses[i].RawPosition.y, Delta);
                    Assert.AreEqual(mouses[0].RawDelta.x, mouses[i].RawDelta.x, Delta);
                    Assert.AreEqual(mouses[0].RawDelta.y, mouses[i].RawDelta.y, Delta);
                    Assert.AreEqual(mouses[0].Position.x, mouses[i].Position.x, Delta);
                    Assert.AreEqual(mouses[0].Position.y, mouses[i].Position.y, Delta);
                    Assert.AreEqual(mouses[0].Delta.x, mouses[i].Delta.x, Delta);
                    Assert.AreEqual(mouses[0].Delta.y, mouses[i].Delta.y, Delta);
                }
            }

            if (touches.Count > 0)
            {
                touchUpdateId++;

                for (int i = 0; i < Input.touchCount; i++)
                {
                    var touch = Input.GetTouch(i);
                    if(touch.fingerId >= touches.Count)
                        continue;

                    if(inputManager == null)
                        touches[touch.fingerId].Process(touch, touchUpdateId);

                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (Input.GetKey(KeyCode.Mouse0 + i))
                        {
                            Debug.LogWarning("Touch for code: " + touches[touch.fingerId].Key);
                            Debug.Log($"Raw pos: {mouses[i].RawPosition}, Delta: {mouses[i].RawDelta}");
                            Debug.Log($"Pos: {mouses[i].Position}, Delta: {mouses[i].Delta}");
                        }
                    }
                }
                if (inputManager == null)
                {
                    for (int i = 0; i < touches.Count; i++)
                        touches[i].VerifyTouch(touchUpdateId);
                }
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

            if (inputManager != null)
            {
                if (IsActivated)
                {
                    if(!inputManager.UseMouse)
                        inputManager.UseMouse = true;
                    if(!inputManager.UseTouch)
                        inputManager.UseTouch = true;
                    if(!inputManager.UseKeyboard)
                        inputManager.UseKeyboard = true;

                    Assert.IsTrue(inputManager.UseMouse);
                    Assert.IsTrue(inputManager.UseTouch);
                    Assert.IsTrue(inputManager.UseKeyboard);
                }
                else
                {
                    if(inputManager.UseMouse)
                        inputManager.UseMouse = false;
                    if(inputManager.UseTouch)
                        inputManager.UseTouch = false;
                    if(inputManager.UseKeyboard)
                        inputManager.UseKeyboard = false;

                    Assert.IsFalse(inputManager.UseMouse);
                    Assert.IsFalse(inputManager.UseTouch);
                    Assert.IsFalse(inputManager.UseKeyboard);
                }
            }
        }
    }
}