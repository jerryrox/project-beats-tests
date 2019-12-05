using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Inputs
{
    public class CursorTest {

        private const float Delta = 0.0001f;


        [Test]
        public void Test()
        {
            var cursor = new DummyCursor();
            Assert.AreEqual(KeyCode.Mouse0, cursor.Key);
            Assert.AreEqual(InputState.Idle, cursor.State.Value);
            Assert.AreEqual(false, cursor.IsActive.Value);
            Assert.AreEqual(Vector2.zero, cursor.RawPosition);
            Assert.AreEqual(Vector2.zero, cursor.RawDelta);
            Assert.AreEqual(Vector2.zero, cursor.Position);
            Assert.AreEqual(Vector2.zero, cursor.Delta);

            cursor.SetActive(true);
            Assert.AreEqual(true, cursor.IsActive.Value);

            cursor.SetState(InputState.Hold);
            Assert.AreEqual(InputState.Hold, cursor.State.Value);

            cursor.Release();
            Assert.AreEqual(InputState.Idle, cursor.State.Value);

            cursor.Process(0, 0);
            Assert.AreEqual(0f, cursor.RawPosition.x, Delta);
            Assert.AreEqual(0f, cursor.RawPosition.y, Delta);
            Assert.AreEqual(-640f, cursor.Position.x, Delta);
            Assert.AreEqual(360f, cursor.Position.y, Delta);

            cursor.Process(Screen.width, Screen.height);
            Assert.AreEqual(Screen.width, cursor.RawPosition.x, Delta);
            Assert.AreEqual(Screen.height, cursor.RawPosition.y, Delta);
            Assert.AreEqual(Screen.width, cursor.RawDelta.x, Delta);
            Assert.AreEqual(Screen.height, cursor.RawDelta.y, Delta);
            Assert.AreEqual(640, cursor.Position.x, Delta);
            Assert.AreEqual(-360, cursor.Position.y, Delta);
            Assert.AreEqual(1280f, cursor.Delta.x, Delta);
            Assert.AreEqual(-720f, cursor.Delta.y, Delta);
        }

        private class DummyCursor : Cursor
        {
            public DummyCursor() : base(KeyCode.Mouse0, new Vector2(1280, 720)) {}

            public void SetState(InputState state) => this.state.Value = state;

            public void Process(float x, float y) => ProcessPosition(x, y);
        }
    }
}