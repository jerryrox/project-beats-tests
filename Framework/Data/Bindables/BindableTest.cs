using System;
using NUnit.Framework;

namespace PBFramework.Data.Bindables.Tests
{
    public class BindableTest
    {
        [Test]
        public void TestCreation()
        {
            var dummy = CreateBindable();
            Assert.AreEqual(null, dummy.Value);
            Assert.AreEqual(null, dummy.RawValue);

            var d = new Dummy();
            dummy = CreateBindable(d);
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

            var dummy = CreateBindable();
            dummy.OnValueChanged += (v, _) => {
                d1 = v;
                onValueChangedCalled = true;
            };
            dummy.OnRawValueChanged += (v, _) => {
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

        [Test]
        public void TestTriggerWhenDifferent()
        {
            var dummy1 = new Dummy();
            var dummy2 = new Dummy();

            int checkIndex = 0;
            Action<Dummy, Dummy> sameCheck = (x, y) =>
            {
                Assert.AreEqual(x, y);
                checkIndex++;
            };
            Action<Dummy, Dummy> differentCheck = (x, y) =>
            {
                Assert.AreNotEqual(x, y);
                checkIndex++;
            };

            var bindable = CreateBindable(dummy1);

            bindable.OnValueChanged += sameCheck;
            bindable.Value = dummy1;
            Assert.AreEqual(1, checkIndex);
            bindable.OnValueChanged -= sameCheck;

            bindable.OnValueChanged += differentCheck;
            bindable.Value = dummy2;
            Assert.AreEqual(2, checkIndex);
            bindable.OnValueChanged -= differentCheck;

            bindable.TriggerWhenDifferent = true;

            bindable.OnValueChanged += sameCheck;
            bindable.Value = dummy2;
            Assert.AreEqual(2, checkIndex);
            bindable.OnValueChanged -= sameCheck;

            bindable.OnValueChanged += differentCheck;
            bindable.Value = dummy1;
            Assert.AreEqual(3, checkIndex);
            bindable.OnValueChanged -= differentCheck;

            bindable.OnValueChanged += sameCheck;
            bindable.Value = dummy1;
            Assert.AreEqual(3, checkIndex);
            bindable.OnValueChanged -= sameCheck;
        }

        protected virtual IBindable<Dummy> CreateBindable(Dummy d = null)
        {
            return new Bindable<Dummy>(d);
        }

        public class Dummy
        {

        }
    }
}
