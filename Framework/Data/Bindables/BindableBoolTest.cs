using System;
using NUnit.Framework;

namespace PBFramework.Data.Bindables.Tests
{
    public class BindableBoolTest
    {
        [Test]
        public void Test()
        {
            bool lastUpdated = false;
            var bindable = new BindableBool();
            bindable.OnValueChanged += (v, _) => lastUpdated = v;

            Assert.IsFalse(bindable.Value);

            bindable.Value = true;
            Assert.IsTrue(bindable.Value);
            Assert.IsTrue(lastUpdated);
        }
    }
}