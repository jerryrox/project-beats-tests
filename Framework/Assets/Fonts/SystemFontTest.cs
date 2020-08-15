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
            ISystemFontInfo courierFont = null;
            foreach (var font in SystemFontProvider.Fonts)
            {
                if (courierFont == null && font.Name.Equals("Courier", StringComparison.OrdinalIgnoreCase))
                {
                    courierFont = font;
                }
                Debug.Log(font.ToString());
            }

            Assert.AreEqual("Courier", courierFont.Name);
            Assert.AreEqual("Courier Bold", courierFont.BoldName);
            Assert.AreEqual("Courier Italic", courierFont.ItalicName);
            Assert.AreEqual("Courier Bold Italic", courierFont.BoldItalicName);

            SystemFont systemFont = new SystemFont(courierFont);
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