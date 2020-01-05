using NUnit.Framework;
using UnityEngine.EventSystems;
using PBFramework.Graphics;
using PBFramework.Graphics.Tests;

namespace PBFramework.UI.Tests
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