using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using PBFramework.Assets.Atlasing;
using PBFramework.Graphics.Tests;
using PBFramework.Dependencies;

namespace PBFramework.UI.Tests
{
    public class UguiDropdownTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var font = env.ArialFont.ToFont();

            var dropdown = root.CreateChild<UguiDropdown>("dropdown");
            for (int i = 0; i < 10; i++)
                dropdown.Options.Add(new Dropdown.OptionData($"Option {i}"));
            dropdown.Label.Font = font;
            dropdown.Label.Color = Color.black;

            dropdown.Property.Font = font;
            dropdown.Property.PopupBackground.Color = new Color(0f, 0f, 0f, 0.6f);
            dropdown.Property.LabelColor = new Color(1f, 1f, 1f, 0.8f);
            dropdown.Property.SelectedColor = new Color(1f, 1f, 1f, 1f);
            dropdown.Property.EntryHeight = 30f;

            dropdown.Value = 5;
            
            while (env.IsRunning)
            {
                yield return null;
            }
        }
    }
}