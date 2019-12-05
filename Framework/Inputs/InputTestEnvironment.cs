using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Inputs.Tests
{
    public class InputTestEnvironment : MonoBehaviour {

        public bool IsRunning { get; private set; } = true;

        public Vector2 Resolution { get; set; } = new Vector2(1280f, 720f);

        public static InputTestEnvironment Create()
        {
            return new GameObject("InputTestEnv").AddComponent<InputTestEnvironment>();
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
        }
    }
}