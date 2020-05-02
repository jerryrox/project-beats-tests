using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.UI;
using PBFramework.Graphics.Tests;

namespace PBFramework.Graphics.Effects.Shaders.Tests
{
    public class DefaultShaderEffectTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(null);

            var obj1 = root.CreateChild<UguiSprite>("1", 0);
            {
                obj1.Alpha = 0.5f;
                obj1.Position = new Vector2(-50f, 50f);
            }
            var obj2 = root.CreateChild<UguiSprite>("2", 1);
            {
                obj2.Alpha = 0.5f;
                obj2.Position = new Vector2(0f, 00f);
            }
            var obj3 = root.CreateChild<UguiSprite>("3", 2);
            {
                obj3.Alpha = 0.5f;
                obj3.Position = new Vector2(20f, 25f);
            }

            var effects = new DefaultShaderEffect[] {
                obj1.AddEffect(new DefaultShaderEffect()),
                obj2.AddEffect(new DefaultShaderEffect()),
                obj3.AddEffect(new DefaultShaderEffect()),
            };

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    effects.ForEach(e =>
                    {
                        e.StencilOperation = StencilOp.IncrementSaturate;
                        e.CompareFunction = CompareFunction.Equal;
                        Debug.Log("Changed op: " + e.StencilOperation + ", comp : " + e.CompareFunction);
                    });
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    effects.ForEach(e =>
                    {
                        e.StencilOperation = StencilOp.Keep;
                        e.CompareFunction = CompareFunction.Always;
                    });
                }
                yield return null;
            }
        }
    }
}