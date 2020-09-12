using System.IO;
using UnityEngine;

public class TestConstants {

    public const string RemoteMp3Url = "https://vgmdownloads.com/soundtracks/touhou-youyoumu-perfect-cherry-blossom/bufsdczp/%5B05%5D%20Diao%20ye%20zong%20%28withered%20leaf%29.mp3";

    public const string RemoteImageUrl = "https://cdn-www.bluestacks.com/bs-images/Banner_com.sunborn.girlsfrontline.en-1.jpg";

    public static readonly string TestAssetPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")), "_Test");
}