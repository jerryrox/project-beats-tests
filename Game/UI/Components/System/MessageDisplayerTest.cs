using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBGame.Notifications;
using PBFramework.UI;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.System
{
    public class MessageDisplayerTest {

        private int nextNotifType;
        private MessageDisplayer displayer;


        [ReceivesDependency]
        private INotificationBox NotificationBox { get; set; }

        [ReceivesDependency]
        private IRootMain RootMain { get; set; }
        
        
        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                Actions = new TestAction[]
                {
                    new TestAction(true, KeyCode.Q, () => AddNotification(1, NotificationScope.Temporary), "Adds a new temporary notification with one line."),
                    new TestAction(true, KeyCode.W, () => AddNotification(2, NotificationScope.Stored), "Adds a new stored notification with two lines."),
                    new TestAction(true, KeyCode.E, () => AddNotification(3, NotificationScope.Temporary), "Adds a new temporary notification with three lines."),
                }
            };
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init()
        {
            displayer = RootMain.CreateChild<MessageDisplayer>("displayer", 0);
            {
                displayer.Size = new Vector2(320f, 0f);
                displayer.Position = new Vector3(0f, 300f, 0f);
            }
        }

        private IEnumerator AddNotification(int lines, NotificationScope scope)
        {
            string message = string.Join("\n", Enumerable.Range(0, lines).Select(i => "asdf" + i));

            NotificationBox.Add(new Notification() {
                Message = message,
                Type = GetNotificationType(),
                Scope = scope,
            });
            yield break;
        }

        private NotificationType GetNotificationType()
        {
            NotificationType type = (NotificationType)nextNotifType;
            nextNotifType++;
            if(nextNotifType >= Enum.GetValues(typeof(NotificationType)).Length)
                nextNotifType = 0;
            return type;
        }
    }
}