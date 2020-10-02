using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.UI.Components.SettingsMenu.Navbars;
using PBGame.Tests;
using PBGame.Graphics;
using PBGame.Configurations;
using PBGame.Configurations.Settings;
using PBFramework.UI;
using PBFramework.Data.Bindables;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.SettingsMenu.Navbars.Tests
{
    public class NavBarTest {

        private bool isUsingActualConfig = false;

        private ISettingsData settingsData;
        private NavBar navBar;

        private BindableBool testDataAA;
        private BindableBool testDataAB;
        private Bindable<TestType> testDataAC;

        private BindableFloat testDataBA;
        private BindableFloat testDataBB;
        private BindableInt testDataBC;


        [ReceivesDependency]
        private IGameConfiguration GameConfiguration { get; set; }

        [ReceivesDependency]
        private IRootMain RootMain { get; set; }


        [UnityTest]
        public IEnumerator TestDummyConfig()
        {
            isUsingActualConfig = false;
            TestOptions options = new TestOptions()
            {
                UpdateMethod = Update,
            };
            return TestGame.Setup(this, options).Run();
        }

        [UnityTest]
        public IEnumerator TestActualConfig()
        {
            isUsingActualConfig = true;
            TestOptions options = new TestOptions()
            {
                UpdateMethod = Update,
            };
            return TestGame.Setup(this, options).Run();
        }

        [InitWithDependency]
        private void Init()
        {
            if (isUsingActualConfig)
            {
                // Load configurations
                GameConfiguration.Load();
                settingsData = GameConfiguration.Settings;
            }
            else
            {
                settingsData = new SettingsData();
                var a = settingsData.AddTabData(new SettingsTab("A", "icon-arrow-left"));
                a.AddEntry(new SettingsEntryBool("a-a", testDataAA = new BindableBool(false)));
                a.AddEntry(new SettingsEntryBool("a-b", testDataAB = new BindableBool(true)));
                a.AddEntry(new SettingsEntryEnum<TestType>("a-c", testDataAC = new Bindable<TestType>(TestType.TypeB)));

                var b = settingsData.AddTabData(new SettingsTab("B", "icon-backward"));
                b.AddEntry(new SettingsEntryFloat("b-a", testDataBA = new BindableFloat(0f, -10f, 10f)));
                b.AddEntry(new SettingsEntryFloat("b-b", testDataBB = new BindableFloat(5f, 10f, 20f)));
                b.AddEntry(new SettingsEntryInt("b-c", testDataBC = new BindableInt(0, -20, 5)));
            }

            // Create nav bar.
            navBar = RootMain.CreateChild<NavBar>("nav-bar");
            {
                navBar.Size = new Vector2(64f, 400f);
                navBar.SetSettingsData(settingsData);
            }
        }

        private void Update()
        {
            for (int i = 0; i < settingsData.TabCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    navBar.ShowFocusOnTab(settingsData[i]);
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