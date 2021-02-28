using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.Notifications.Tests
{
    public class NotificationBoxTest {
        
        [Test]
        public void TestForceStored()
        {
            NotificationBox box = new NotificationBox();
            box.Add(new Notification()
            {
                Id = "lol",
                Type = NotificationType.Verbose,
            });
            box.Add(new Notification()
            {
                Id = "lol4",
                Type = NotificationType.Error,
            });
            Assert.AreEqual(0, box.Notifications.Count);

            box.ForceStoreLevel = NotificationType.Info;
            box.Add(new Notification()
            {
                Id = "lol",
                Type = NotificationType.Verbose,
            });
            box.Add(new Notification()
            {
                Id = "lol2",
                Type = NotificationType.Info,
            });
            box.Add(new Notification()
            {
                Id = "lol3",
                Type = NotificationType.Warning,
            });
            box.Add(new Notification()
            {
                Id = "lol4",
                Type = NotificationType.Error,
            });
            Assert.AreEqual(3, box.Notifications.Count);
            Assert.AreEqual(0, box.Notifications.Where((notification) => notification.Type == NotificationType.Verbose).Count());
        }
    }
}