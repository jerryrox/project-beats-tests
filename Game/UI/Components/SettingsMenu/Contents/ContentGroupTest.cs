using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBGame.Configurations.Settings;
using PBFramework.UI;
using PBFramework.Data.Bindables;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.SettingsMenu.Contents.Test
{
    public class ContentGroupTest {

        private SettingsTab tabData;
        private ContentGroup contentGroup;


        [ReceivesDependency]
        private IRootMain RootMain { get; set; }
        

        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions();
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init()
        {
            BindableFloat bindableFloat = new BindableFloat(10f, -1, 20f);
            bindableFloat.OnValueChanged += (val, _) => Debug.Log($"{nameof(bindableFloat)} value: {val}");

            Bindable<string> bindableString = new Bindable<string>("My Text");
            bindableString.OnValueChanged += (val, _) => Debug.Log($"{nameof(bindableString)} value: {val}");

            BindableInt bindableInt = new BindableInt(-10, -20, 0);
            bindableInt.OnValueChanged += (val, _) => Debug.Log($"{nameof(bindableInt)} value: {val}");

            Bindable<TestType> bindableEnum = new Bindable<TestType>(TestType.TypeB);
            bindableEnum.OnValueChanged += (val, _) => Debug.Log($"{nameof(bindableEnum)} value: {val}");

            BindableBool bindableBool = new BindableBool(false);
            bindableBool.OnValueChanged += (val, _) => Debug.Log($"{nameof(bindableBool)} value: {val}");

            BindableBool bindableBool2 = new BindableBool(true);
            bindableBool2.OnValueChanged += (val, _) => Debug.Log($"{nameof(bindableBool2)} value: {val}");

            tabData = new SettingsTab("A", "icon-arrow-left");
            tabData.AddEntry(new SettingsEntryFloat(nameof(bindableFloat), bindableFloat));
            tabData.AddEntry(new SettingsEntryString(nameof(bindableString), bindableString));
            tabData.AddEntry(new SettingsEntryInt(nameof(bindableInt), bindableInt));
            tabData.AddEntry(new SettingsEntryEnum<TestType>(nameof(bindableEnum), bindableEnum));
            tabData.AddEntry(new SettingsEntryBool(nameof(bindableBool), bindableBool));
            tabData.AddEntry(new SettingsEntryBool(nameof(bindableBool2), bindableBool2));

            var bg = RootMain.CreateChild<UguiSprite>("bg", -1);
            {
                bg.Size = new Vector2(600f, 600f);
                bg.Alpha = 0.5f;
            }
            contentGroup = RootMain.CreateChild<ContentGroup>();
            {
                contentGroup.Width = 400f;
                contentGroup.SetTabData(tabData);
            }
        }

        private enum TestType
        {
            TypeA,
            TypeB,
            TypeC
        }
    }
}