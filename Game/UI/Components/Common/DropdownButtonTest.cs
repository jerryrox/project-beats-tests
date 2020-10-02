using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.UI.Components.Common.Dropdown;
using PBGame.Tests;
using PBGame.Graphics;
using PBFramework.UI;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Common.Tests
{
    public class DropdownButtonTest {

        private DropdownContext context;


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
            context = new DropdownContext();
            context.Datas.Add(new DropdownData("Selection A", "Extra A"));
            context.Datas.Add(new DropdownData("Selection B", "Extra B"));
            context.Datas.Add(new DropdownData("Selection C", "Extra C"));
            context.Datas.Add(new DropdownData("Selection D", "Extra D"));
            context.Datas.Add(new DropdownData("Selection E", "Extra E"));
            context.SelectData(context.Datas[0]);
            context.OnSelection += OnSelectionChange;

            var container = RootMain.CreateChild<UguiSprite>("bg");
            {
                container.Size = new Vector2(500f, 500f);
                container.Alpha = 0.25f;

                var button = container.CreateChild<DropdownButton>("dropdown");
                button.Size = new Vector2(200f, 42f);
                button.Context = context;
            }
        }

        private void OnSelectionChange(DropdownData data)
        {
            Debug.Log("OnSelectionChange: " + data.Text + ", extra: " + data.ExtraData);
        }
    }
}