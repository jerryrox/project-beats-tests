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
using PBGame.Configurations;
using PBFramework.UI;
using PBFramework.Graphics;
using PBFramework.Threading;
using PBFramework.Dependencies;

namespace PBGame.UI.Components.Prepare.Details.Meta.Tests
{
    public class MetaContainerTest {

        private IMetaContainer metaContainer;


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

                        MapSelection.SelectMapset(MapManager.AllMapsets[0], map.GetPlayable(GameModes.BeatsStandard));
                    }
                }
            );
        }

        private IEnumerator Init()
        {
            // Load a test map
            Debug.Log("Loading map");
            var progress = new ReturnableProgress<IMapset>();
            MapManager.Load(new Guid("f97d1638-e8de-4f25-b860-76f1803167c7"), progress);
            yield return TestGame.AwaitProgress(progress);
            Assert.AreEqual(1, MapManager.AllMapsets.Count);
            Assert.AreEqual("f97d1638-e8de-4f25-b860-76f1803167c7", MapManager.AllMapsets[0].Id.ToString());

            // Create meta container display.
            metaContainer = RootMain.CreateChild<MetaContainer>();
            metaContainer.Width = 1152f;
            metaContainer.Height = 360f;
        }
    }
}