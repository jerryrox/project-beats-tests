using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PBFramework.IO.Decoding.Tests
{
    public class JsonDecoderTest {

        [Test]
        public void TestDecode()
        {
            using (var stream = GetStream())
            {
                var decoder = Decoders.GetDecoder<JObject>(stream);
                Assert.IsNotNull(decoder);
                Assert.AreEqual(typeof(JsonDecoder), decoder.GetType());

                var json = decoder.Decode(stream);
                Assert.AreEqual(1, json["A"].Value<int>());
                Assert.AreEqual("Lol", json["B"].ToString());
                Assert.IsTrue(json["C"].Value<bool>());
            }
        }

        private StreamReader GetStream()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "IO/Decoding/JsonDecoderTest.txt");
            return new StreamReader(File.OpenRead(path));
        }
    }
}