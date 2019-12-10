using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using PBFramework.Dependencies;

namespace PBFramework.Graphics.Tests
{
    public class GraphicTestEnvironment : MonoBehaviour {

        public bool IsRunning { get; protected set; } = true;


        /// <summary>
        /// Creates a new test environment.
        /// </summary>
        public static GraphicTestEnvironment Create()
        {
            Debug.LogWarning("Left shift + alpha 1 = success");
            Debug.LogWarning("Left shift + alpha 2 = fail");
            return new GameObject("TestEnv").AddComponent<GraphicTestEnvironment>();
        }

        /// <summary>
        /// Creates a new ugui root object.
        /// </summary>
        public UguiRoot CreateRoot(IDependencyContainer dependency)
        {
            var root = UguiRoot.Create(dependency);
            var camera = new GameObject("Cam").AddComponent<Camera>();
            root.SetCameraRender(camera);
            root.Resolution = new Vector2(1280f, 720f);
            return root;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Debug.Log("Manually succeeded test");
                    IsRunning = false;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Assert.Fail("Manually failed test");
                }
            }
        }
    }
}