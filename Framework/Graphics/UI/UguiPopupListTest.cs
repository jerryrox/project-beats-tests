using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Assets.Atlasing;
using PBFramework.Graphics.Tests;
using PBFramework.Dependencies;

namespace PBFramework.Graphics.UI.Tests
{
    public class UguiPopupListTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var dropdown = root.CreateChild<UguiDropdown>();

            while (env.IsRunning)
            {
                
                yield return null;
            }
        }
    }
}