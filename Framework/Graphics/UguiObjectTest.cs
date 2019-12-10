using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Graphics.Tests
{
    public class UguiObjectTest {

        private const float Delta = 0.0001f;


        [UnityTest]
        public IEnumerator TestCreation()
        {
            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(null);

            var obj = root.CreateChild("MyObject");
            Assert.AreEqual("MyObject", obj.Name);
            Assert.AreEqual(1, root.transform.childCount);
            Assert.AreEqual(obj.RawTransform, root.transform.GetChild(0));

            var child = obj.CreateChild();
            Assert.AreEqual(child.Parent, obj);

            child.SetParent(root);
            Assert.AreEqual(child.Parent, root);
            Assert.AreEqual(2, root.transform.childCount);

            obj.Destroy();
            yield return null;
            Assert.AreEqual(1, root.transform.childCount);
            Assert.AreEqual(child.RawTransform, root.transform.GetChild(0));

            while (env.IsRunning)
            {
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator TestProperties()
        {
            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(null);

            var obj = root.CreateChild("MyObject");

            obj.Name = "Lol object";
            Assert.AreEqual("Lol object", obj.Name);

            obj.X = 100f;
            obj.Y = 50f;
            Assert.AreEqual(100f, obj.X, Delta);
            Assert.AreEqual(50f, obj.Y, Delta);
            Assert.AreEqual(new Vector2(100f, 50f), obj.Position);

            obj.Width = 20;
            obj.Height = 25;
            Assert.AreEqual(20, obj.Width, Delta);
            Assert.AreEqual(25, obj.Height, Delta);
            Assert.AreEqual(new Vector2(20f, 25f), obj.Size);

            obj.Position = new Vector2(10f, 20f);
            Assert.AreEqual(new Vector2(10f, 20f), obj.Position);
            Assert.AreEqual(10f, obj.X, Delta);
            Assert.AreEqual(20f, obj.Y, Delta);

            obj.RotationX = 5f;
            Assert.AreEqual(5f, obj.RotationX, Delta);
            obj.RotationX = 0f;

            obj.RotationY = 6f;
            Assert.AreEqual(6f, obj.RotationY, Delta);
            obj.RotationY = 0f;

            obj.RotationZ = 60f;
            Assert.AreEqual(60f, obj.RotationZ, Delta);
            obj.RotationZ = 0f;

            obj.Rotation = new Vector3(5f, 5f, 5f);
            Debug.Log("Rotation set to (5, 5, 5). Should be checked in inspector!");

            obj.ScaleX = 0.5f;
            obj.ScaleY = 0.75f;
            Assert.AreEqual(0.5f, obj.ScaleX, Delta);
            Assert.AreEqual(0.75f, obj.ScaleY, Delta);
            Assert.AreEqual(new Vector3(0.5f, 0.75f, 1f), obj.Scale);

            obj.Scale = new Vector3(1.1f, 1.2f, 1f);
            Assert.AreEqual(new Vector3(1.1f, 1.2f, 1f), obj.Scale);

            obj.Pivot = Pivots.TopRight;
            Assert.AreEqual(GraphicHelper.GetPivot(Pivots.TopRight), obj.RawTransform.pivot);

            obj.Anchor = Anchors.BottomRight;
            Assert.AreEqual(GraphicHelper.GetMinAnchor(Anchors.BottomRight), obj.RawTransform.anchorMin);
            Assert.AreEqual(GraphicHelper.GetMinAnchor(Anchors.BottomRight), obj.RawTransform.anchorMax);

            obj.Depth = 100;
            Assert.AreEqual(100, obj.Depth);

            while (env.IsRunning)
            {
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator TestDepth()
        {
            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(null);

            var obj = root.CreateChild("0");
            var obj2 = root.CreateChild("1");
            var obj3 = root.CreateChild("2");

            obj.Depth = 1;
            obj3.Depth = 0;
            obj2.Depth = 2;

            Assert.AreEqual(3, root.transform.childCount);
            Assert.AreEqual(obj3.RawTransform, root.transform.GetChild(0));
            Assert.AreEqual(obj.RawTransform, root.transform.GetChild(1));
            Assert.AreEqual(obj2.RawTransform, root.transform.GetChild(2));

            obj2.Depth = -1;
            Assert.AreEqual(obj2.RawTransform, root.transform.GetChild(0));
            Assert.AreEqual(obj3.RawTransform, root.transform.GetChild(1));
            Assert.AreEqual(obj.RawTransform, root.transform.GetChild(2));

            int curDepth = -1;
            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    curDepth--;
                    obj.Depth = curDepth;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    curDepth--;
                    obj2.Depth = curDepth;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    curDepth--;
                    obj3.Depth = curDepth;
                }
                yield return null;
            }
        }
    }
}