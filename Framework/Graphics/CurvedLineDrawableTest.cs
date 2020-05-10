using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Graphics.Tests
{
    public class CurvedLineDrawableTest
    {
        [UnityTest]
        public IEnumerator TestCreation()
        {
            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(null);

            var obj = root.CreateChild("MyObject");
            var drawable = obj.RawObject.AddComponent<CurvedLineDrawable>();
            drawable.CurveRadius = 15f;
            drawable.CurveAngle = 10f;

            List<Transform> hints = new List<Transform>();
            hints.Add(CreateHintAt(root.transform, new Vector2(-4f, 4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-3f, 3f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-2f, 4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-1f, 3f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(0f, 4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(1f, 3f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(2f, 4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(3f, 3f)));

            hints.Add(CreateHintAt(root.transform, new Vector2(4f, 4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(3f, 3f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(4f, 2f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(3f, 1f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(4f, 0f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(3f, -1f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(4f, -2f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(3f, -3f)));

            hints.Add(CreateHintAt(root.transform, new Vector2(4f, -4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(3f, -3f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(2f, -4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(1f, -3f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(0f, -4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-1f, -3f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-2f, -4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-3f, -3f)));

            hints.Add(CreateHintAt(root.transform, new Vector2(-4f, -4f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-3f, -3f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-4f, -2f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-3f, -1f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-4f, 0f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-3f, 1f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-4f, 2f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-3f, 3f)));
            hints.Add(CreateHintAt(root.transform, new Vector2(-4f, 4f)));

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    hints.ForEach(h => GameObject.Destroy(h.gameObject));
                    hints.Clear();
                    drawable.ClearLines();
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    Transform hint = new GameObject(hints.Count.ToString()).transform;
                    hint.SetParent(root.transform);
                    hint.ResetTransform();
                    hints.Add(hint);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hints.Count > 0)
                    {
                        GameObject.Destroy(hints[hints.Count - 1].gameObject);
                        hints.RemoveAt(hints.Count - 1);
                        drawable.ClearLines();
                    }
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    drawable.color = new Color(Random.Range(0.4f, 1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f));
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    drawable.SetAlpha(0.4f);
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    drawable.SetAlpha(1f);
                }
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    drawable.UseSmoothEnds = !drawable.UseSmoothEnds;
                }
                if (Input.GetKeyDown(KeyCode.Minus))
                {
                    drawable.CurveRadius--;
                }
                if (Input.GetKeyDown(KeyCode.Equals))
                {
                    drawable.CurveRadius++;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    drawable.ClearLines();
                    for (int i = 0; i < hints.Count - 1; i++)
                    {
                        var hint = hints[i];
                        var nextHint = hints[i + 1];
                        drawable.AddLine(new Line(hint.localPosition, nextHint.localPosition));
                    }
                }

                // if(
                yield return null;
            }
        }

        private Transform CreateHintAt(Transform parent, Vector2 pos)
        {
            var tm = new GameObject("def").transform;
            pos *= 50f;
            tm.SetParent(parent);
            tm.ResetTransform();
            tm.localPosition = pos;
            return tm;
        }
    }
}