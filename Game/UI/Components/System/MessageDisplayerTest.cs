using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBGame.Notifications;
using PBFramework.UI;
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
            yield return TestGame.Run(
                this,
                () => Init(),
                Update
            );
        }
        
        private IEnumerator Init()
        {
            displayer = RootMain.CreateChild<MessageDisplayer>("displayer", 0);
            {
                displayer.Size = new Vector2(320f, 0f);
                displayer.Position = new Vector3(0f, 300f, 0f);
            }
            yield break;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                NotificationBox.Add(new Notification() {
                    Message = "My message lolz",
                    Type = GetNotificationType(),
                    Scope = NotificationScope.Temporary,
                });
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                NotificationBox.Add(new Notification()
                {
                    Message = "My message lolz\nNext line",
                    Type = GetNotificationType(),
                    Scope = NotificationScope.Stored,
                });
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                NotificationBox.Add(new Notification()
                {
                    Message = "My message lolz\nNext line\nAnother line",
                    Type = GetNotificationType(),
                    Scope = NotificationScope.Temporary,
                });
            }
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