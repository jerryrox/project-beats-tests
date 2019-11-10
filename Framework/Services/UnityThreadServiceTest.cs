using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Services.Tests
{
    public class UnityThreadServiceTest {

        [UnityTest]
        public IEnumerator TestObjectCreation()
        {
            var obj = new GameObject("ASDF");

            var found = GameObject.Find("ASDF");
            Assert.IsNotNull(found);
            Assert.AreSame(obj, found);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestStartCoroutine()
        {
            var dummy = new Dummy();
            var coroutine = UnityThreadService.StartCoroutine(MyProcess(dummy));

            int limit = 15;
            while (!dummy.Finished)
            {
                yield return new WaitForSecondsRealtime(1);
                limit--;
                if (limit <= 0)
                {
                    Assert.Fail("Coroutine took to much time.");
                }
            }

            Assert.AreEqual(true, dummy.DidRun);
            Assert.AreEqual(0, dummy.I);
            Assert.AreEqual(true, dummy.Finished);
        }

        [UnityTest]
        public IEnumerator TestStopCoroutine()
        {
            var dummy = new Dummy();
            var coroutine = UnityThreadService.StartCoroutine(MyProcess(dummy));

            int dur = 5;
            while (dur > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                dur--;
            }

            UnityThreadService.StopCoroutine(coroutine);
            dur = 8;
            while (dur > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                Debug.Log("Waiting for: " + dur);
                dur--;
            }

            Assert.Greater(dummy.I, 0, "Dummy's I shouldn't be 0 or less!!");
            Assert.AreEqual(true, dummy.DidRun);
            Assert.IsFalse(dummy.Finished);
        }

        IEnumerator MyProcess(Dummy dummy)
        {
            dummy.DidRun = true;
            while (dummy.I > 0)
            {
                float curTime = Time.realtimeSinceStartup;
                yield return new WaitForSecondsRealtime(1);

                Debug.Log(dummy.I.ToString());
                Assert.LessOrEqual(curTime, Time.realtimeSinceStartup - 0.75f, "The waiting seem to have finished too early!");
                dummy.I--;
            }
            dummy.Finished = true;
        }


        private class Dummy
        {
            public bool DidRun = false;
            public int I = 10;

            public bool Finished = false;
        }
    }
}