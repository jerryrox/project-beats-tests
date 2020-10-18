using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using PBGame.Tests;
using PBGame.Graphics;
using PBGame.Rulesets.Judgements;
using PBFramework.UI;
using PBFramework.Testing;
using PBFramework.Graphics;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Result.Tests
{
    public class JudgementCountItemTest {

        private JudgementCountItem item;

        [ReceivesDependency]
        private IRootMain RootMain { get; set; }

        [ReceivesDependency]
        private IColorPreset ColorPreset { get; set; }


        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                Actions = new TestAction[]
                {
                    new TestAction(KeyCode.Q, () => TestHitResultTypeAssignment(), "Tests hit result type assignment"),
                    new TestAction(KeyCode.W, () => TestCountAssignment(), "Tests hit count assignment"),
                },
            };
            return TestGame.Setup(this, options).Run();
        }
        
        [InitWithDependency]
        private void Init()
        {
            item = RootMain.CreateChild<JudgementCountItem>("item");
            {
                item.Size = new Vector2(64, 64f);
            }
        }

        private IEnumerator TestHitResultTypeAssignment()
        {
            foreach (var type in (HitResultType[])Enum.GetValues(typeof(HitResultType)))
            {
                item.SetResultType(type);

                var color = ColorPreset.GetHitResultColor(type);
                Assert.AreEqual(color.Base, item.FindWithName<UguiSprite>("base").Color);
                Assert.AreEqual(color.Alpha(0.25f), item.FindWithName<UguiSprite>("bg").Color);

                yield return null;
            }
        }

        private IEnumerator TestCountAssignment()
        {
            var countLabel = item.FindWithName<Label>("count");

            item.SetCount(0);
            Assert.AreEqual("0", countLabel.Text);

            item.SetCount(512);
            Assert.AreEqual("512", countLabel.Text);

            item.SetCount(1024);
            Assert.AreEqual("1,024", countLabel.Text);
            yield break;
        }
    }
}