using System.IO;
using UnityEngine;

public class TestConstants {

    public const string RemoteMp3Url = "https://vgmdownloads.com/soundtracks/touhou-youyoumu-perfect-cherry-blossom/vrdyenmp/%5B01%5D%20Youyoumu%20~%20Snow%20or%20Cherry%20Petal.mp3";

    public const string RemoteImageUrl = "https://cdn-www.bluestacks.com/bs-images/Banner_com.sunborn.girlsfrontline.en-1.jpg";

    public static readonly string TestAssetPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")), "_Test");
}