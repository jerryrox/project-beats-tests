using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBFramework.UI;
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
            yield return TestGame.Run(
                this,
                () => Init(),
                Update
            );
        }
        
        private IEnumerator Init()
        {
            context = new DropdownContext();
            context.OnSelection += OnSelectionChange;
            context.Datas.Add(new DropdownData("Selection A", "Extra A"));
            context.Datas.Add(new DropdownData("Selection B", "Extra B"));
            context.Datas.Add(new DropdownData("Selection C", "Extra C"));
            context.Datas.Add(new DropdownData("Selection D", "Extra D"));
            context.Datas.Add(new DropdownData("Selection E", "Extra E"));

            var container = RootMain.CreateChild<UguiSprite>("bg");
            {
                container.Size = new Vector2(500f, 500f);
                container.Alpha = 0.25f;

                menu = container.CreateChild<DropdownMenu>("dropdown");
                menu.CloseMenu();
            }
            yield break;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                menu.OpenMenu(context);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                menu.CloseMenu();
            }
        }

        private void OnSelectionChange(DropdownData data)
        {
            Debug.Log("OnSelectionChange: " + data.Text + ", extra: " + data.ExtraData);
        }
    }
}