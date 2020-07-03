using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading.Tests
{
    public class UnityThreadTest {

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
            var coroutine = UnityThread.StartCoroutine(MyProcess(dummy));

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
            var coroutine = UnityThread.StartCoroutine(MyProcess(dummy));

            int dur = 5;
            while (dur > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                dur--;
            }

            UnityThread.StopCoroutine(coroutine);
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

        [UnityTest]
        public IEnumerator TestDispatch()
        {
            bool finished = false;

            int unityThread = Thread.CurrentThread.ManagedThreadId;

            UnityThread.Initialize();
            Task.Run(() =>
            {
                Assert.AreNotEqual(unityThread, Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(500);
                Assert.IsTrue((bool)UnityThread.Dispatch(() => finished = true));
            });
            while (!finished)
            {
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator TestDispatchUnattended()
        {
            bool finished = false;

            int unityThread = Thread.CurrentThread.ManagedThreadId;

            UnityThread.Initialize();
            Task.Run(() =>
            {
                Assert.AreNotEqual(unityThread, Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(500);
                UnityThread.DispatchUnattended(() => finished = true);
            });
            while (!finished)
            {
                yield return null;
            }
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