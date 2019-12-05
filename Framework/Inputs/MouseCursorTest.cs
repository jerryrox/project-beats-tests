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

            environment.SetActivatable(cursors);
            environment.SetMouses(cursors);
            environment.ListenToState(cursors);

            while (environment.IsRunning)
            {
                yield return null;
            }
        }
    }
}