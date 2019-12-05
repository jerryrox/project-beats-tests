using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Inputs.Tests
{
    public class MouseCursorTest {

        private const float Delta = 0.0001f;


        [UnityTest]
        public IEnumerator Test()
        {
            var environment = InputTestEnvironment.Create();
            var cursors = new MouseCursor[] {
                new MouseCursor(KeyCode.Mouse0, environment.Resolution),
                new MouseCursor(KeyCode.Mouse1, environment.Resolution)
            };

            while (environment.IsRunning)
            {
                for (int i = 0; i < cursors.Length; i++)
                {
                    var active = !Input.GetKey(KeyCode.LeftShift);
                    if (active)
                    {
                        if (!cursors[i].IsActive.Value)
                        {
                            cursors[i].SetActive(true);
                            Debug.LogWarning($"Cursor {i} activated");
                        }
                        Assert.IsTrue(cursors[i].IsActive.Value);

                        cursors[i].Process();
                        if (i == 0)
                        {
                            Debug.Log($"Raw pos: {cursors[i].RawPosition}, Delta: {cursors[i].RawDelta}");
                            Debug.Log($"Pos: {cursors[i].Position}, Delta: {cursors[i].Delta}");
                        }
                    }
                    else
                    {
                        if (cursors[i].IsActive.Value)
                        {
                            cursors[i].SetActive(false);
                            cursors[i].Release();
                            Debug.LogWarning($"Cursor {i} released");
                        }
                        Assert.IsFalse(cursors[i].IsActive.Value);
                    }
                }

                Assert.AreEqual(cursors[0].RawPosition.x, cursors[1].RawPosition.x, Delta);
                Assert.AreEqual(cursors[0].RawPosition.y, cursors[1].RawPosition.y, Delta);
                Assert.AreEqual(cursors[0].RawDelta.x, cursors[1].RawDelta.x, Delta);
                Assert.AreEqual(cursors[0].RawDelta.y, cursors[1].RawDelta.y, Delta);
                Assert.AreEqual(cursors[0].Position.x, cursors[1].Position.x, Delta);
                Assert.AreEqual(cursors[0].Position.y, cursors[1].Position.y, Delta);
                Assert.AreEqual(cursors[0].Delta.x, cursors[1].Delta.x, Delta);
                Assert.AreEqual(cursors[0].Delta.y, cursors[1].Delta.y, Delta);

                yield return null;
            }
        }
    }
}