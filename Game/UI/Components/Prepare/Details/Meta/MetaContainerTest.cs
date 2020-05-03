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
using PBFramework.Threading;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Prepare.Details.Meta.Tests
{
    public class MetaContainerTest {

        private MetaContainer metaContainer;


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
            yield return TestGame.Run(
                this,
                () => Init(),
                () =>
                {
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        var map = MapManager.AllMapsets[0].Maps[0];
                        map.CreatePlayable(ModeManager);

                        MapSelection.SelectMapset(MapManager.AllMapsets[0], map.GetPlayable(GameModeType.BeatsStandard));
                    }
                }
            );
        }

        private IEnumerator Init()
        {
            string testMapId = "";

            // Load a test map
            Debug.Log("Loading map");
            var progress = new ReturnableProgress<IMapset>();
            MapManager.Load(new Guid("2e8a9917-2970-437b-bc51-ca9d4bcdd670"), progress);
            yield return TestGame.AwaitProgress(progress);
            Assert.AreEqual(1, MapManager.AllMapsets.Count);
            Assert.AreEqual("2e8a9917-2970-437b-bc51-ca9d4bcdd670", MapManager.AllMapsets[0].Id.ToString());

            // Create meta container display.
            metaContainer = RootMain.CreateChild<MetaContainer>();
            metaContainer.Width = 1152f;
            metaContainer.Height = 360f;
        }
    }
}