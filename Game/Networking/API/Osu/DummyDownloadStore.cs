using System.IO;
using UnityEngine;
using PBGame.Stores;
using PBFramework.Storages;

namespace PBGame.Networking.API.Osu.Tests
{
    public class DummyDownloadStore : IDownloadStore {

        public IFileStorage MapStorage { get; private set; }


        public DummyDownloadStore()
        {
            MapStorage = new FileStorage(new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "DownloadedMaps")));
        }
    }
}