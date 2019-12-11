using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Assets.Fonts.Tests
{
    public class SystemFontTest {
        
        [Test]
        public void LogAllOsFonts()
        {
            foreach (var name in Font.GetOSInstalledFontNames())
            {
                Debug.Log(name);
            }
        }

        [Test]
        public void TestSystemFontProvider()
        {
            ISystemFontInfo arialFont = null;
            foreach (var font in SystemFontProvider.Fonts)
            {
                if (arialFont == null && font.Name.Equals("Arial", StringComparison.OrdinalIgnoreCase))
                {
                    arialFont = font;
                }
                Debug.Log(font.ToString());
            }

            Assert.AreEqual("Arial", arialFont.Name);
            Assert.AreEqual("Arial Bold", arialFont.BoldName);
            Assert.AreEqual("Arial Italic", arialFont.ItalicName);
            Assert.AreEqual("Arial Bold Italic", arialFont.BoldItalicName);

            SystemFont systemFont = new SystemFont(arialFont);
            Assert.IsNotNull(systemFont.Normal);
            Assert.IsNotNull(systemFont.Bold);
            Assert.IsNotNull(systemFont.Italic);
            Assert.IsNotNull(systemFont.BoldItalic);
            Assert.IsTrue(systemFont.HasBold);
            Assert.IsTrue(systemFont.HasItalic);
            Assert.IsTrue(systemFont.HasBoldItalic);

            systemFont.Dispose();
            Assert.IsNull(systemFont.Normal);
            Assert.IsNull(systemFont.Bold);
            Assert.IsNull(systemFont.Italic);
            Assert.IsNull(systemFont.BoldItalic);
            Assert.IsFalse(systemFont.HasBold);
            Assert.IsFalse(systemFont.HasItalic);
            Assert.IsFalse(systemFont.HasBoldItalic);
        }
    }
}