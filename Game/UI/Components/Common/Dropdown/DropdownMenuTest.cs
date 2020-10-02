using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBFramework.UI;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Common.Dropdown.Tests
{
    public class DropdownMenuTest {

        private DropdownMenu menu;
        private DropdownContext context;


        [ReceivesDependency]
        private IRootMain RootMain { get; set; }
        
        
        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                Actions = new TestAction[]
                {
                    new TestAction(true, KeyCode.Q, () => OpenMenu(), "Opens dropdown menu"),
                    new TestAction(true, KeyCode.W, () => CloseMenu(), "Closes current dropdown menu")
                }
            };
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init()
        {
            context = new DropdownContext();
            context.OnSelection += OnSelectionChange;
            context.Datas.Add(new DropdownData("Selection A", "Extra A"));
            context.Datas.Add(new DropdownData("Selection B", "Extra B"));
            context.Datas.Add(new DropdownData("Selection C", "Extra C"));
            context.Datas.Add(new DropdownData("Selection D", "Extra D"));
            context.Datas.Add(new DropdownData("Selection E", "Extra E"));
            context.Datas.Add(new DropdownData("Selection F", "Extra F"));
            context.Datas.Add(new DropdownData("Selection G", "Extra G"));
            context.Datas.Add(new DropdownData("Selection H", "Extra H"));
            context.Datas.Add(new DropdownData("Selection I", "Extra I"));
            context.Datas.Add(new DropdownData("Selection J", "Extra J"));

            var container = RootMain.CreateChild<UguiSprite>("bg");
            {
                container.Size = new Vector2(500f, 500f);
                container.Alpha = 0.25f;

                menu = container.CreateChild<DropdownMenu>("dropdown");
                menu.CloseMenu();
            }
        }

        private IEnumerator OpenMenu()
        {
            menu.OpenMenu(context);
            yield break;
        }

        private IEnumerator CloseMenu()
        {
            menu.CloseMenu();
            yield break;
        }

        private void OnSelectionChange(DropdownData data)
        {
            Debug.Log("OnSelectionChange: " + data.Text + ", extra: " + data.ExtraData);
        }
    }
}