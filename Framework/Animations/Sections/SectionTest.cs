using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Animations.Sections.Tests
{
    public class SectionTest {

        private const float Delta = 0.00001f;


        [Test]
        public void TestBuildFail()
        {
            var section = new Section<float>(new DummyAnime(), (value) => {});
            Assert.AreEqual(0f, section.Duration);

            try
            {
                section.Build();
                Assert.Fail();
            }
            catch (Exception) { }

            section.AddTime(0f, 0f).Build();
            try
            {
                section.Build();
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [Test]
        public void TestBuildWithPlaceholder()
        {
            float curValue = 0f;

            var section = new Section<float>(new DummyAnime(), (value) => {
                curValue = value;
            });
            Assert.AreEqual(0f, section.Duration);

            section.AddTime(1f, 10f)
                .AddTime(2f, 30f)
                .Build();
            Assert.AreEqual(2f, section.Duration, Delta);

            TestTimeManipulation(section, () => curValue);
        }


        [Test]
        public void TestBuildWithoutPlaceholder()
        {
            float curValue = 0f;

            var section = new Section<float>(new DummyAnime(), (value) => {
                curValue = value;
            });
            Assert.AreEqual(0f, section.Duration);

            section.AddTime(0f, 10f)
                .AddTime(1f, 10f)
                .AddTime(2f, 30f)
                .Build();
            Assert.AreEqual(2f, section.Duration, Delta);

            TestTimeManipulation(section, () => curValue);
        }

        private void TestTimeManipulation(Section<float> section, Func<float> getValue)
        {
            section.UpdateTime(0f);
            Assert.AreEqual(10f, getValue(), Delta);

            section.UpdateTime(0.5f);
            Assert.AreEqual(10f, getValue(), Delta);

            section.UpdateTime(1f);
            Assert.AreEqual(10f, getValue(), Delta);

            section.UpdateTime(1.5f);
            Assert.AreEqual(20f, getValue(), Delta);

            section.UpdateTime(2f);
            Assert.AreEqual(30f, getValue(), Delta);

            section.UpdateTime(2.1f);
            Assert.AreEqual(30f, getValue(), Delta);

            section.SeekTime(1f);
            Assert.AreEqual(10f, getValue(), Delta);

            section.SeekTime(2f);
            Assert.AreEqual(30f, getValue(), Delta);

            section.SeekTime(0f);
            Assert.AreEqual(10f, getValue(), Delta);
        }

        private class DummyAnime : IAnimeEditor
        {
            void IAnimeEditor.OnBuildSection(ISection section) {}
        }
    }
}