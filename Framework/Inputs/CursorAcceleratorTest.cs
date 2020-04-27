using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Inputs.Tests
{
    public class CursorAcceleratorTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var accelerator = new CursorAccelerator();
            while (true)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                    break;
                else if(Input.GetKeyDown(KeyCode.Alpha2))
                    Assert.Fail();

                accelerator.Update();
                Debug.Log("Acceleration: " + accelerator.Acceleration);
                yield return null;
            }
        }
    }
}