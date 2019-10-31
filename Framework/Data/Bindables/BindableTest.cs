using System;
using NUnit.Framework;

namespace PBFramework.Data.Bindables.Tests
{
    public class BindableTest
    {
        [Test]
        public void TestCreation()
        {
            var dummy = new Bindable<Dummy>();
            Assert.AreEqual(null, dummy.Value);
            Assert.AreEqual(null, dummy.RawValue);

            var d = new Dummy();
            dummy = new Bindable<Dummy>(d);
            Assert.AreEqual(d, dummy.Value);
            Assert.AreEqual(d, dummy.RawValue);
        }

        [Test]
        public void TestEvent()
        {
            bool onValueChangedCalled = false;
            bool onRawValueChangedCalled = false;
            Dummy d1 = new Dummy();
            Dummy d2 = new Dummy();

            var dummy = new Bindable<Dummy>();
            dummy.OnValueChanged += (v) => {
                d1 = v;
                onValueChangedCalled = true;
            };
            dummy.OnRawValueChanged += (v) => {
                d2 = v as Dummy;
                onRawValueChangedCalled = true;
            };

            Assert.IsFalse(onValueChangedCalled);
            Assert.IsFalse(onRawValueChangedCalled);
            Assert.IsNotNull(d1);
            Assert.IsNotNull(d2);

            dummy.Trigger();

            Assert.IsTrue(onValueChangedCalled);
            Assert.IsTrue(onRawValueChangedCalled);
            Assert.IsNull(d1);
            Assert.IsNull(d2);

            Dummy d3 = new Dummy();
            dummy.Value = d3;

            Assert.AreEqual(d3, d1);
            Assert.AreEqual(d3, d2);

            Dummy d4 = new Dummy();
            dummy.RawValue = d4;

            Assert.AreEqual(d4, d1);
            Assert.AreEqual(d4, d2);
        }

        private class Dummy
        {

        }
    }
}
