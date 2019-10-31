using System;
using NUnit.Framework;

namespace PBFramework.Data.Bindables.Tests
{
    public class BindableNumberTest
    {
        private const float FloatDelta = 0.00000001f;
        private const double DoubleDelta = 0.0000000001;


        [Test]
        public void TestBindableDouble()
        {
            double lastUpdated = 0.0;
            var bindable = new BindableDouble();
            bindable.OnValueChanged += (v) => lastUpdated = v;

            Assert.AreEqual(double.MinValue, bindable.MinValue, DoubleDelta);
            Assert.AreEqual(double.MaxValue, bindable.MaxValue, DoubleDelta);
            Assert.AreEqual(0.0, bindable.Value, DoubleDelta);

            bindable.MinValue = 1.0;
            Assert.AreEqual(1.0, bindable.MinValue, DoubleDelta);
            Assert.AreEqual(1.0, bindable.Value, DoubleDelta);
            Assert.AreEqual(1.0, lastUpdated, DoubleDelta);

            bindable.Value = 2;
            Assert.AreEqual(2.0, bindable.Value, DoubleDelta);
            Assert.AreEqual(2.0, lastUpdated, DoubleDelta);

            bindable = new BindableDouble(5, -100, 100);
            bindable.OnValueChanged += (v) => lastUpdated = v;

            Assert.AreEqual(-100.0, bindable.MinValue, DoubleDelta);
            Assert.AreEqual(100.0, bindable.MaxValue, DoubleDelta);
            Assert.AreEqual(5.0, bindable.Value, DoubleDelta);

            bindable.MaxValue = -1;
            Assert.AreEqual(-1.0, bindable.MaxValue, DoubleDelta);
            Assert.AreEqual(-1.0, bindable.Value, DoubleDelta);
            Assert.AreEqual(-1.0, lastUpdated, DoubleDelta);
        }

        [Test]
        public void TestBindableFloat()
        {
            float lastUpdated = 0.0f;
            var bindable = new BindableFloat();
            bindable.OnValueChanged += (v) => lastUpdated = v;

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

            bindable = new BindableFloat(5, -100, 100);
            bindable.OnValueChanged += (v) => lastUpdated = v;

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
            var bindable = new BindableInt();
            bindable.OnValueChanged += (v) => lastUpdated = v;

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

            bindable = new BindableInt(5, -100, 100);
            bindable.OnValueChanged += (v) => lastUpdated = v;

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