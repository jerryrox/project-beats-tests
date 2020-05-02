using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Graphics.Tests
{
    public class LineTest {

        private const float Delta = 0.0001f;


        [Test]
        public void TestThetas()
        {
            Line line = new Line(new Vector2(1f, 0f), new Vector2(0f, 0f));
            Assert.AreEqual(180f * Mathf.Deg2Rad, line.Theta, Delta);

            line = new Line(new Vector2(1f, 0f), new Vector2(0f, 1f));
            Assert.AreEqual(135f * Mathf.Deg2Rad, line.Theta, Delta);

            line = new Line(new Vector2(0f, 0f), new Vector2(0f, 1f));
            Assert.AreEqual(90f * Mathf.Deg2Rad, line.Theta, Delta);

            line = new Line(new Vector2(0f, 0f), new Vector2(1f, 0f));
            Assert.AreEqual(0f * Mathf.Deg2Rad, line.Theta, Delta);

            line = new Line(new Vector2(0f, 0f), new Vector2(-1f, -1f));
            Assert.AreEqual(-135f * Mathf.Deg2Rad, line.Theta, Delta);

            line = new Line(new Vector2(0f, 0f), new Vector2(0f, -1f));
            Assert.AreEqual(-90f * Mathf.Deg2Rad, line.Theta, Delta);
        }

        [Test]
        public void TestAngleDiff()
        {
            Line line = new Line(new Vector2(1f, 0f), new Vector2(0f, 0f));
            Line line2 = new Line(new Vector2(0f, 0f), new Vector2(0f, -1f));
            Assert.AreEqual(90f * Mathf.Deg2Rad, line.GetAngleDiff(line2), Delta);

            line = new Line(new Vector2(1f, 0f), new Vector2(0f, 0f));
            line2 = new Line(new Vector2(0f, 0f), new Vector2(0f, 1f));
            Assert.AreEqual(-90f * Mathf.Deg2Rad, line.GetAngleDiff(line2), Delta);

            line = new Line(new Vector2(1f, 0f), new Vector2(0f, 0f));
            line2 = new Line(new Vector2(0f, 0f), new Vector2(-1f, 0f));
            Assert.AreEqual(0f, line.GetAngleDiff(line2), Delta);

            line = new Line(new Vector2(-1f, 0f), new Vector2(0f, 1f));
            line2 = new Line(new Vector2(0f, 1f), new Vector2(1f, 0f));
            Assert.AreEqual(90f * Mathf.Deg2Rad, line.GetAngleDiff(line2), Delta);

            line = new Line(new Vector2(-1f, 0f), new Vector2(0f, -1f));
            line2 = new Line(new Vector2(0f, -1f), new Vector2(1f, 0f));
            Assert.AreEqual(-90f * Mathf.Deg2Rad, line.GetAngleDiff(line2), Delta);

            line = new Line(new Vector2(1f, 0f), new Vector2(0f, 1f));
            line2 = new Line(new Vector2(0f, 1f), new Vector2(-1f, 0f));
            Assert.AreEqual(90f * Mathf.Deg2Rad, line.GetAngleDiff(line2), Delta);

            line = new Line(new Vector2(1f, 0f), new Vector2(0f, -1f));
            line2 = new Line(new Vector2(0f, -1f), new Vector2(-1f, 0f));
            Assert.AreEqual(-90f * Mathf.Deg2Rad, line.GetAngleDiff(line2), Delta);
        }
    }
}