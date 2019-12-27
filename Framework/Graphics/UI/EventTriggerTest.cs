using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;
using PBFramework.Assets.Atlasing;
using PBFramework.Graphics.Tests;
using PBFramework.Dependencies;

namespace PBFramework.Graphics.UI.Tests
{
    public class EventTriggerTest {
        
        [Test]
        public void Test()
        {
            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(null);

            var eventTrigger = root.CreateChild<UguiObject>("lol").AddComponentInject<EventTrigger>();
            bool eventCalled = false;
            var entry = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerClick,
                callback = new EventTrigger.TriggerEvent()
            };
            entry.callback.AddListener(delegate { eventCalled = true; });
            eventTrigger.triggers.Add(entry);

            eventTrigger.OnPointerClick(null);
            Assert.IsTrue(eventCalled);
        }
    }
}