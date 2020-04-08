using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Data.Bindables.Tests
{
    public class ProxyBindableNumberTest {

        private const float FloatDelta = 0.00000001f;


        [Test]
        public void TestBindableFloat()
        {
            float lastUpdated = 0.0f;
            float orig = 0f;
            var bindable = new ProxyBindableFloat(() => orig, (v) => orig = v);
            bindable.OnValueChanged += (v, _) => lastUpdated = v;

            Assert.AreEqual(float.MinValue, bindable.MinValue, FloatDelta);
            Assert.AreEqual(float.MaxValue, bindable.MaxValue, FloatDelta);
            Assert.AreEqual(0.0, bindable.Value, FloatDelta);

            bindable.MinValue = 1.0f;
            Assert.AreEqual(1.0f, bindable.MinValue, FloatDelta);
            Assert.AreEqual(1.0f, bindable.Value, FloatDelta);
            Assert.AreEqual(1.0f, lastUpdated, FloatDelta);

            bindable.Value = 2;
            Assert.AreEqual(2.0f, bindable.Value, FloatDelta);
            Assert.AreEqual(2.0f, lastUpdated, FloatDelta);

            orig = 5f;
            bindable = new ProxyBindableFloat(() => orig, (v) => orig = v, -100, 100);
            bindable.OnValueChanged += (v, _) => lastUpdated = v;

            Assert.AreEqual(-100.0f, bindable.MinValue, FloatDelta);
            Assert.AreEqual(100.0f, bindable.MaxValue, FloatDelta);
            Assert.AreEqual(5.0f, bindable.Value, FloatDelta);

            bindable.MaxValue = -1;
            Assert.AreEqual(-1.0f, bindable.MaxValue, FloatDelta);
            Assert.AreEqual(-1.0f, bindable.Value, FloatDelta);
            Assert.AreEqual(-1.0f, lastUpdated, FloatDelta);
        }

        [Test]
        public void TestBindableInt()
        {
            int lastUpdated = 0;
            int orig = 0;
            var bindable = new ProxyBindableInt(() => orig, (v) => orig = v);
            bindable.OnValueChanged += (v, _) => lastUpdated = v;

            Assert.AreEqual(int.MinValue, bindable.MinValue);
            Assert.AreEqual(int.MaxValue, bindable.MaxValue);
            Assert.AreEqual(0, bindable.Value);

            bindable.MinValue = 1;
            Assert.AreEqual(1, bindable.MinValue);
            Assert.AreEqual(1, bindable.Value);
            Assert.AreEqual(1, lastUpdated);

            bindable.Value = 2;
            Assert.AreEqual(2, bindable.Value);
            Assert.AreEqual(2, lastUpdated);

            orig = 5;
            bindable = new ProxyBindableInt(() => orig, (v) => orig = v, -100, 100);
            bindable.OnValueChanged += (v, _) => lastUpdated = v;

            Assert.AreEqual(-100, bindable.MinValue);
            Assert.AreEqual(100, bindable.MaxValue);
            Assert.AreEqual(5, bindable.Value);

            bindable.MaxValue = -1;
            Assert.AreEqual(-1, bindable.MaxValue);
            Assert.AreEqual(-1, bindable.Value);
            Assert.AreEqual(-1, lastUpdated);
        }
    }
}