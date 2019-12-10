using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace PBFramework.Graphics.Tests
{
    public class UguiRootTest {
        
        [Test]
        public void TestCreation()
        {
            var root = UguiRoot.Create(null);
            root.SetOverlayRender();
            root.Resolution = new Vector2(1280f, 720f);

            var canvas = root.GetComponent<Canvas>();
            var scaler = root.GetComponent<CanvasScaler>();
            var raycaster = root.GetComponent<GraphicRaycaster>();
            Assert.IsNotNull(canvas);
            Assert.IsNotNull(scaler);
            Assert.IsNotNull(raycaster);
            Assert.AreEqual(root.Resolution, new Vector2(1280f, 720f));
            Assert.AreEqual(canvas.renderMode, RenderMode.ScreenSpaceOverlay);
            Assert.AreEqual(scaler.uiScaleMode, CanvasScaler.ScaleMode.ScaleWithScreenSize);
            Assert.AreEqual(scaler.screenMatchMode, CanvasScaler.ScreenMatchMode.Expand);
        }
    }
}