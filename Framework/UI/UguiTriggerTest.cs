using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using PBFramework.Graphics;
using PBFramework.Inputs;
using PBFramework.Assets.Atlasing;
using PBFramework.Graphics.Tests;
using PBFramework.Dependencies;

namespace PBFramework.UI.Tests
{
    public class UguiTriggerTest
    {

        [UnityTest]
        public IEnumerator TestWithInputManager()
        {
            var inputManager = InputManager.Create(new Vector2(1280f, 720f));

            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IInputManager>(inputManager);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var camera = root.Cam;

            var trigger = root.CreateChild<UguiTrigger>("toggle");
            var bg = trigger.CreateChild<UguiSprite>("bg");
            {
                bg.Anchor = AnchorType.Fill;
                bg.Offset = Offset.Zero;
            }

            var pointerEvent = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            while (env.IsRunning)
            {
                var mouse = inputManager.GetMouse(0);
                pointerEvent.position = mouse.RawPosition;
                Debug.Log("Mouse pos: " + pointerEvent.position);

                root.Raycaster.Raycast(pointerEvent, results);
                foreach (var result in results)
                {
                    Debug.LogWarning("--------------");
                    Debug.Log("Local pos: " + trigger.transform.InverseTransformPoint(result.worldPosition));
                }

                results.Clear();
                yield return null;
            }
        }
    }
}