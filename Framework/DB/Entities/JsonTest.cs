using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBFramework.Data.Bindables;

namespace PBFramework.DB.Entities.Tests
{
    public class JsonTest {

        private const float Delta = 0.0000001f;

        [Test]
        public void TestSerializeToJObject()
        {
            Dummy dummy = new Dummy(55)
            {
                A = 10,
                B = "ASDF",
            };
            JObject json = (JObject)JToken.FromObject(dummy);
            Debug.Log(json.ToString());
            Assert.AreEqual(dummy.A, json["A"].Value<int>());
            Assert.AreEqual(dummy.B, json["B"].ToString());
        }

        [Test]
        public void TestDeserialize()
        {
            var json = JsonConvert.DeserializeObject<JObject>(@"
                {
                    'A': '100',
                    'B': 'FDSA'
                }
            ");
            Assert.AreEqual(100, json["A"].Value<int>());
            Assert.AreEqual("FDSA", json["B"].ToString());

            var dummy = json.ToObject<Dummy>();
            Assert.IsNotNull(dummy);
            Assert.AreEqual(100, dummy.A);
            Assert.AreEqual("FDSA", dummy.B);
        }

        [Test]
        public void TestAlias()
        {
            Dummy dummy = new Dummy()
            {
                C = 10.5f
            };
            JObject json = (JObject)JToken.FromObject(dummy);
            Debug.Log(json.ToString());
            Assert.AreEqual(dummy.C, json["CC"].Value<float>(), Delta);

            Dummy newDummy = JsonConvert.DeserializeObject<Dummy>(@"
                {
                    'CC': 11.2
                }
            ");
            Assert.IsNotNull(newDummy);
            Assert.AreNotSame(dummy, newDummy);
            Assert.AreEqual(11.2f, newDummy.C, Delta);
        }

        [Test]
        public void TestIgnore()
        {
            Dummy dummy = new Dummy()
            {
                D = true
            };
            JObject json = (JObject)JToken.FromObject(dummy);
            Debug.Log(json.ToString());
            Assert.IsFalse(json.ContainsKey("D"));

            json = JsonConvert.DeserializeObject<JObject>(@"
                {
                    'D': true
                }
            ");
            Assert.IsTrue(json.ContainsKey("D"));
            Assert.IsTrue(json["D"].Value<bool>());

            dummy = json.ToObject<Dummy>();
            Assert.IsFalse(dummy.D);
        }

        [Test]
        public void TestSerializeBindableInt()
        {
            BindableInt original = new BindableInt(100);
            original.OnValueChanged += (_, __) => { };
            string jsonStr = JToken.FromObject(original).ToString();
            Debug.Log(jsonStr);

            BindableInt reconstructed = JsonConvert.DeserializeObject<BindableInt>(jsonStr);
            Assert.AreEqual(original.Value, reconstructed.Value);
            Assert.AreEqual(original.MaxValue, reconstructed.MaxValue);
            Assert.AreEqual(original.MinValue, reconstructed.MinValue);
            Assert.AreEqual(original.TriggerWhenDifferent, reconstructed.TriggerWhenDifferent);
            Assert.AreEqual(original.RawValue, reconstructed.RawValue);
        }

        [Test]
        public void TestSerializeBindableObject()
        {
            Dummy dummy = new Dummy()
            {
                A = 1,
                B = "A",
                C = 5.1f,
                D = true
            };
            Bindable<Dummy> original = new Bindable<Dummy>(dummy);
            original.OnValueChanged += (_, __) => { };
            string jsonStr = JToken.FromObject(original).ToString();
            Debug.Log(jsonStr);

            Bindable<Dummy> reconstructed = JsonConvert.DeserializeObject<Bindable<Dummy>>(jsonStr);
            Assert.AreEqual(dummy.A, reconstructed.Value.A);
            Assert.AreEqual(dummy.B, reconstructed.Value.B);
            Assert.AreEqual(dummy.C, reconstructed.Value.C);
            // D has JsonIgnore.
            Assert.AreNotEqual(dummy.D, reconstructed.Value.D);
        }


        public class Dummy
        {
            public int A { get; set; }
            public string B { get; set; }

            [JsonProperty("CC")]
            public float C { get; set; }

            [JsonIgnore]
            public bool D { get; set; }

            [JsonProperty]
            private int E { get; set; }


            public Dummy()
            {
            }

            public Dummy(int e)
            {
                E = e;
            }
        }
    }
}