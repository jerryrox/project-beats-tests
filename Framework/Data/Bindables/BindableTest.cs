using System;
using NUnit.Framework;

using IntList = System.Collections.Generic.List<int>;

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

        [Test]
        public void TestSetValueWithoutTrigger()
        {
            BindableInt bindableInt = new BindableInt(0);
            int updatedValue = bindableInt.Value;
            bindableInt.OnNewValue += (val) => updatedValue = val;

            bindableInt.Value = 1;
            Assert.AreEqual(1, bindableInt.Value);
            Assert.AreEqual(bindableInt.Value, updatedValue);

            bindableInt.SetWithoutTrigger(5);
            Assert.AreEqual(5, bindableInt.Value);
            Assert.AreEqual(1, updatedValue);
        }

        [Test]
        public void TestTriggerWithCustomPrevState()
        {
            BindableInt bindableInt = new BindableInt(5);
            int newValue = bindableInt.Value;
            int prevValue = bindableInt.Value;
            bindableInt.OnValueChanged += (n, p) =>
            {
                newValue = n;
                prevValue = p;
            };

            bindableInt.Value = 10;
            Assert.AreEqual(10, newValue);
            Assert.AreEqual(5, prevValue);

            bindableInt.TriggerWithPrevious(9);
            Assert.AreEqual(10, newValue);
            Assert.AreEqual(9, prevValue);
        }

        [Test]
        public void TestModifyValue()
        {
            Bindable<IntList> bindableList = new Bindable<IntList>(new IntList() {
                0, 1
            });

            bool calledNewValue = false;
            bindableList.OnNewValue += delegate { calledNewValue = true; };

            Assert.AreEqual(2, bindableList.Value.Count);
            Assert.IsFalse(calledNewValue);

            bindableList.ModifyValue((list) =>
            {
                list.Add(2);
                list.Add(3);
            });
            Assert.AreEqual(4, bindableList.Value.Count);
            Assert.IsTrue(calledNewValue);
        }

        [Test]
        public void TestBindToAndUnbindFrom()
        {
            Bindable<Dummy> dummy = new Bindable<Dummy>(new Dummy());
            Bindable<Dummy> dummy2 = new Bindable<Dummy>(new Dummy());
            Assert.AreNotEqual(dummy.Value, dummy2.Value);

            dummy2.BindTo(dummy);
            Assert.AreEqual(dummy.Value, dummy2.Value);

            dummy.Value = null;
            Assert.IsNull(dummy.Value);
            Assert.IsNull(dummy2.Value);

            dummy2.UnbindFrom(dummy);
            dummy.Value = new Dummy();
            Assert.IsNotNull(dummy.Value);
            Assert.IsNull(dummy2.Value);

            Bindable<SubDummy> subDummy = new Bindable<SubDummy>(new SubDummy());
            dummy.BindToRaw(subDummy);
            Assert.AreEqual(subDummy.Value, dummy.Value);

            subDummy.Value = new SubDummy();
            Assert.AreEqual(subDummy.Value, dummy.Value);

            dummy.UnbindFromRaw(subDummy);
            subDummy.Value = null;
            Assert.IsNull(subDummy.Value);
            Assert.IsNotNull(dummy.Value);

            dummy.Value = new Dummy();
            Assert.Throws(typeof(ArgumentException), () =>
            {
                subDummy.BindToRaw(dummy);
            });
        }

        [Test]
        public void TestBindUnbind()
        {
            var bindable = new Bindable<string>("");
            var rawBindable = bindable as IBindable;
            int newValueCount = 0;
            int valueChangeCount = 0;
            int newRawValueCount = 0;
            int rawValueChangeCount = 0;

            Action<string> onNewValue = (v) => newValueCount++;
            Action<string, string> onValueChange = (v, v2) => valueChangeCount++;
            Action<object> onNewRawValue = (v) => newRawValueCount++;
            Action<object, object> onRawValueChange = (v, v2) => rawValueChangeCount++;
            bindable.Bind(onNewValue);
            bindable.Bind(onValueChange);
            rawBindable.Bind(onNewRawValue);
            rawBindable.Bind(onRawValueChange);

            rawBindable.RawValue = "a";
            Assert.AreEqual(1, newValueCount);
            Assert.AreEqual(1, valueChangeCount);
            Assert.AreEqual(1, newRawValueCount);
            Assert.AreEqual(1, rawValueChangeCount);

            bindable.Unbind(onNewValue);
            bindable.Unbind(onValueChange);
            bindable.Value = "b";
            Assert.AreEqual(1, newValueCount);
            Assert.AreEqual(1, valueChangeCount);
            Assert.AreEqual(2, newRawValueCount);
            Assert.AreEqual(2, rawValueChangeCount);

            bindable.Unbind(onNewRawValue);
            bindable.Unbind(onRawValueChange);
            bindable.Value = "c";
            Assert.AreEqual(1, newValueCount);
            Assert.AreEqual(1, valueChangeCount);
            Assert.AreEqual(3, newRawValueCount);
            Assert.AreEqual(3, rawValueChangeCount);

            rawBindable.Unbind(onNewRawValue);
            rawBindable.Unbind(onRawValueChange);
            bindable.Value = "d";
            Assert.AreEqual(1, newValueCount);
            Assert.AreEqual(1, valueChangeCount);
            Assert.AreEqual(3, newRawValueCount);
            Assert.AreEqual(3, rawValueChangeCount);
        }

        protected virtual IBindable<Dummy> CreateBindable(Dummy d = null)
        {
            return new Bindable<Dummy>(d);
        }


        public class Dummy
        {
        }

        public class SubDummy : Dummy
        {
        }
    }
}
