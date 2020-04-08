using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Data.Bindables;

namespace PBGame.Configurations.Settings.Tests
{
    public class SettingsEntryTest {
        
        [Test]
        public void TestEntryBool()
        {
            BindableBool data = new BindableBool(false);

            SettingsEntryBool boolEntry = new SettingsEntryBool("test-bool", data);
            Assert.AreEqual("test-bool", boolEntry.Name);
            Assert.AreEqual(data.Value, boolEntry.Value);

            boolEntry.Value = true;
            Assert.IsTrue(boolEntry.Value);
            Assert.IsTrue(data.Value);

            data.Value = false;
            Assert.IsFalse(boolEntry.Value);
            Assert.IsFalse(data.Value);
        }

        [Test]
        public void TestEntryInt()
        {
            BindableInt data = new BindableInt(1, -10, 10);

            SettingsEntryInt intEntry = new SettingsEntryInt("test-int", data);
            Assert.AreEqual("test-int", intEntry.Name);
            Assert.AreEqual(data.Value, intEntry.Value);
            Assert.AreEqual(data.MaxValue, intEntry.MaxValue);
            Assert.AreEqual(data.MinValue, intEntry.MinValue);

            data.MinValue = -11;
            data.MaxValue = 11;
            Assert.AreEqual(-11, intEntry.MinValue);
            Assert.AreEqual(11, intEntry.MaxValue);

            data.Value = 8;
            Assert.AreEqual(8, intEntry.Value);

            intEntry.Value = 10;
            Assert.AreEqual(10, intEntry.Value);
            Assert.AreEqual(10, data.Value);
        }

        [Test]
        public void TestEntryFloat()
        {
            BindableFloat data = new BindableFloat(1, -10, 10);

            SettingsEntryFloat floatEntry = new SettingsEntryFloat("test-float", data);
            Assert.AreEqual("test-float", floatEntry.Name);
            Assert.AreEqual(data.Value, floatEntry.Value);
            Assert.AreEqual(data.MaxValue, floatEntry.MaxValue);
            Assert.AreEqual(data.MinValue, floatEntry.MinValue);

            data.MinValue = -11;
            data.MaxValue = 11;
            Assert.AreEqual(-11, floatEntry.MinValue);
            Assert.AreEqual(11, floatEntry.MaxValue);

            data.Value = 8;
            Assert.AreEqual(8, floatEntry.Value);

            floatEntry.Value = 10;
            Assert.AreEqual(10, floatEntry.Value);
            Assert.AreEqual(10, data.Value);
        }

        [Test]
        public void TestEntryEnum()
        {
            Bindable<TestEnum> data = new Bindable<TestEnum>(TestEnum.TypeB);

            SettingsEntryEnum<TestEnum> enumEntry = new SettingsEntryEnum<TestEnum>("test-enum", data);
            Assert.AreEqual("test-enum", enumEntry.Name);
            Assert.AreEqual(data.Value.ToString(), enumEntry.Value);

            data.Value = TestEnum.TypeA;
            Assert.AreEqual(TestEnum.TypeA.ToString(), enumEntry.Value);

            enumEntry.Value = TestEnum.TypeC.ToString();
            Assert.AreEqual(TestEnum.TypeC, data.Value);
            Assert.AreEqual(TestEnum.TypeC.ToString(), enumEntry.Value);

            var domain = enumEntry.GetValues().ToList();
            var testEnumNames = (string[])Enum.GetNames(typeof(TestEnum));
            Assert.Greater(testEnumNames.Length, 0);
            Assert.AreEqual(testEnumNames.Length, domain.Count);
            for (int i = 0; i < testEnumNames.Length; i++)
                Assert.AreEqual(testEnumNames[i], domain[i]);
        }

        private enum TestEnum
        {
            TypeA,
            TypeB,
            TypeC
        }
    }
}