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
using PBFramework.UI.Navigations;
using PBFramework.Data.Bindables;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Navigations.Overlays.Tests
{
    public class SettingsMenuOverlayTest
    {
        private SettingsData settingsData;
        private SettingsMenuOverlay overlay;


        [ReceivesDependency]
        private IRootMain RootMain { get; set; }

        [ReceivesDependency]
        private IOverlayNavigator OverlayNavigator { get; set; }


        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                Actions = new TestAction[]
                {
                    new TestAction(true, KeyCode.Q, () => AssignSettingsData(), "Assigns settings data to overlay.")
                }
            };
            return TestGame.Setup(this, options).Run();
        }

        [InitWithDependency]
        private void Init()
        {
            settingsData = new SettingsData();
            settingsData.AddTabData(CreateTabData("A", "icon-settings"));
            settingsData.AddTabData(CreateTabData("B", "icon-play"));
            settingsData.AddTabData(CreateTabData("C", "icon-music"));

            var bg = RootMain.CreateChild<UguiSprite>("bg", -1);
            {
                bg.Anchor = AnchorType.Fill;
                bg.Offset = Offset.Zero;
                bg.Alpha = 0.25f;
            }
        }

        private IEnumerator AssignSettingsData()
        {
            overlay = OverlayNavigator.Show<SettingsMenuOverlay>();
            overlay.Model.SetSettingsData(settingsData);
            yield break;
        }

        private SettingsTab CreateTabData(string name, string iconName)
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

            var tabData = new SettingsTab(name, iconName);
            tabData.AddEntry(new SettingsEntryFloat(nameof(bindableFloat), bindableFloat));
            tabData.AddEntry(new SettingsEntryString(nameof(bindableString), bindableString));
            tabData.AddEntry(new SettingsEntryInt(nameof(bindableInt), bindableInt));
            tabData.AddEntry(new SettingsEntryAction("Do action!", () => Debug.Log("Performed action")));
            tabData.AddEntry(new SettingsEntryEnum<TestType>(nameof(bindableEnum), bindableEnum));
            tabData.AddEntry(new SettingsEntryBool(nameof(bindableBool), bindableBool));
            tabData.AddEntry(new SettingsEntryBool(nameof(bindableBool2), bindableBool2));

            return tabData;
        }

        private enum TestType
        {
            TypeA,
            TypeB,
            TypeC
        }
    }
}