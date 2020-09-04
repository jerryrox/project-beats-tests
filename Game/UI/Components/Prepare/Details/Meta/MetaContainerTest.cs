using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBGame.Maps;
using PBGame.Tests;
using PBGame.Graphics;
using PBGame.Rulesets;
using PBGame.Rulesets.Maps;
using PBFramework.Testing;
using PBFramework.Threading;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Prepare.Details.Meta.Tests
{
    public class MetaContainerTest {

        private MetaContainer metaContainer;

        private TestGame testGame;


        [ReceivesDependency]
        private IMapManager MapManager { get; set; }

        [ReceivesDependency]
        private IMapSelection MapSelection { get; set; }

        [ReceivesDependency]
        private IRootMain RootMain { get; set; }

        [ReceivesDependency]
        private IModeManager ModeManager { get; set; }


        [UnityTest]
        public IEnumerator Test()
        {
            TestOptions options = new TestOptions()
            {
                UseManualTesting = true,
                Actions = new TestAction[]
                {
                    new TestAction(() => Init()),
                    new TestAction(true, KeyCode.A, () => AssignMap(), "Assigns a map to the meta container.")
                }
            };
            return TestGame.Setup(this, options).Run();
        }

        private IEnumerator Init()
        {
            string testMapId = "2e8a9917-2970-437b-bc51-ca9d4bcdd670";

            // Load a test map
            Debug.Log("Loading map");
            var progress = new ReturnableProgress<IMapset>();
            MapManager.Load(new Guid(testMapId), progress);
            yield return testGame.AwaitProgress(progress);

            Assert.AreEqual(1, MapManager.AllMapsets.Count);
            Assert.AreEqual(testMapId, MapManager.AllMapsets[0].Id.ToString());

            // Create meta container display.
            metaContainer = RootMain.CreateChild<MetaContainer>();
            metaContainer.Width = 1152f;
            metaContainer.Height = 360f;
        }

        private IEnumerator AssignMap()
        {
            var map = MapManager.AllMapsets[0].Maps[0];
            map.CreatePlayable(ModeManager);

            MapSelection.SelectMapset(MapManager.AllMapsets[0], map.GetPlayable(GameModeType.BeatsStandard));
            yield break;
        }
    }
}