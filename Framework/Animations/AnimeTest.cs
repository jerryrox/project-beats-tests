using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.UI;
using PBFramework.Assets.Atlasing;
using PBFramework.Graphics.Tests;
using PBFramework.Dependencies;

namespace PBFramework.Animations.Tests
{
    public class AnimeTest {

        private const float Delta = 0.0001f;


        [UnityTest]
        public IEnumerator Test()
        {
            var dependencies = new DependencyContainer();
            dependencies.CacheAs<IDependencyContainer>(dependencies);
            dependencies.CacheAs<IAtlas<Sprite>>(new ResourceSpriteAtlas());

            var env = GraphicTestEnvironment.Create();
            var root = env.CreateRoot(dependencies);

            var sprite = root.CreateChild<UguiSprite>("sprite");
            sprite.Size = new Vector2(20f, 20f);

            var anime = new Anime();
            anime.AnimateFloat(value => sprite.X = value)
                .AddTime(0f, 0f, Utils.EaseType.QuadEaseOut)
                .AddTime(1f, 200f, Utils.EaseType.Linear)
                .AddTime(2f, 200f, Utils.EaseType.QuadEaseOut)
                .AddTime(3f, 400f)
                .Build();
            Assert.AreEqual(3f, anime.Duration, Delta);

            anime.AddEvent(0f, () => Debug.Log("Event 0"));
            anime.AddEvent(1f, () => Debug.Log("Event 1"));
            anime.AddEvent(2f, () => Debug.Log("Event 2"));
            anime.AddEvent(3f, () => Debug.Log("Event 3"));
            anime.AddEvent(4f, () => Debug.Log("Event 4"));
            Assert.AreEqual(4f, anime.Duration, Delta);

            while (env.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    anime.Play();
                    Assert.IsTrue(anime.IsPlaying);
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    anime.PlayFromStart();
                    Assert.IsTrue(anime.IsPlaying);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    anime.Pause();
                    Assert.IsFalse(anime.IsPlaying);
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    anime.Stop();
                    Assert.IsFalse(anime.IsPlaying);
                }

                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    anime.Seek(0f);
                    Assert.AreEqual(0f, anime.Time, Delta);
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    anime.Seek(1f);
                    Assert.AreEqual(1f, anime.Time, Delta);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    anime.Seek(2f);
                    Assert.AreEqual(2f, anime.Time, Delta);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    anime.Seek(3f);
                    Assert.AreEqual(3f, anime.Time, Delta);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    anime.Seek(4f);
                    Assert.AreEqual(4f, anime.Time, Delta);
                }

                if(Input.GetKeyDown(KeyCode.Equals))
                    anime.Speed += 0.25f;
                else if(Input.GetKeyDown(KeyCode.Minus))
                    anime.Speed -= 0.25f;

                if (Input.GetKeyDown(KeyCode.A))
                {
                    anime.WrapMode = WrapModeType.None;
                    Debug.Log("Wrapmode set to none");
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    anime.WrapMode = WrapModeType.Reset;
                    Debug.Log("Wrapmode set to reset");
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    anime.WrapMode = WrapModeType.Loop;
                    Debug.Log("Wrapmode set to loop");
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    anime.StopMode = StopModeType.None;
                    Debug.Log("Stopmode set to none");
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    anime.StopMode = StopModeType.Reset;
                    Debug.Log("Stopmode set to reset");
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    anime.StopMode = StopModeType.End;
                    Debug.Log("Stopmode set to end");
                }
                yield return null;
            }
        }
    }
}