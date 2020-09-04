using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading.Futures;

namespace PBFramework.Threading.Tests
{
    public class TimerTest {

        private const float Delta = 0.00000001f;


        [UnityTest]
        public IEnumerator TestSynchronizedSimple()
        {
            yield return TestSimple(new SynchronizedTimer());
        }

        [UnityTest]
        public IEnumerator TestSynchronizedLimit()
        {
            yield return TestLimit(new SynchronizedTimer());
        }

        [UnityTest]
        public IEnumerator TestAsynchronizedSimple()
        {
            yield return TestSimple(new AsynchronizedTimer());
        }

        [UnityTest]
        public IEnumerator TestAsynchronizedLimit()
        {
            yield return TestLimit(new AsynchronizedTimer());
        }

        private IEnumerator TestSimple(ITimer timer)
        {
            // Surrounded with a bunch of try/catch to prevent leak when using AsynchornizedTimer.

            try
            {
                Assert.AreEqual(0f, timer.Current, Delta);
                Assert.IsFalse(timer.IsRunning);
                Assert.IsFalse(timer.IsCompleted.Value);

                timer.Start();
                Assert.IsTrue(timer.IsRunning);
                Assert.IsFalse(timer.IsCompleted.Value);
            }
            catch (Exception e)
            {
                timer.Stop();
                throw e;
            }

            yield return new WaitForSecondsRealtime(1);

            float curTime = timer.Current;
            try
            {
                Debug.Log("Timer time: " + curTime);
                Assert.Greater(timer.Current, 0.75f);
                Assert.IsTrue(timer.IsRunning);
                Assert.IsFalse(timer.IsCompleted.Value);

                timer.Pause();
                curTime = timer.Current;
                Assert.AreEqual(curTime, timer.Current, Delta);
                Assert.IsFalse(timer.IsRunning);
                Assert.IsFalse(timer.IsCompleted.Value);

                timer.Stop();
                Assert.AreEqual(0f, timer.Current, Delta);
                Assert.IsFalse(timer.IsRunning);
                Assert.IsFalse(timer.IsCompleted.Value);

                timer.Start();
            }
            catch (Exception e)
            {
                timer.Stop();
                throw e;
            }

            yield return new WaitForSecondsRealtime(1);
            try
            {
                Debug.Log("Timer time 2: " + timer.Current);
                curTime = timer.Current;
                Assert.Greater(timer.Current, 0.75f);
                Assert.IsTrue(timer.IsRunning);
                Assert.IsFalse(timer.IsCompleted.Value);

                timer.Stop();
            }
            catch (Exception e)
            {
                timer.Stop();
                throw e;
            }
        }

        private IEnumerator TestLimit(ITimer timer)
        {
            timer.Limit = 1f;

            bool finished = false;
            timer.IsCompleted.OnNewValue += (completed) => finished = true;
            bool finisehd2 = false;
            ((IFuture)timer).IsCompleted.OnNewValue += (completed) => finisehd2 = true;

            try
            {
                Assert.IsFalse(timer.IsRunning);
                Assert.IsFalse(timer.IsCompleted.Value);

                timer.Start();
                Assert.IsFalse(finished);
                Assert.IsFalse(finisehd2);
                Assert.IsTrue(timer.IsRunning);
                Assert.IsFalse(timer.IsCompleted.Value);
            }
            catch (Exception e)
            {
                timer.Stop();
                throw e;
            }

            yield return new WaitForSecondsRealtime(1.25f);
            try
            {
                Assert.AreEqual(1f, timer.Progress);
                Assert.IsTrue(finished);
                Assert.IsTrue(finisehd2);
                Assert.IsFalse(timer.IsRunning);
                Assert.True(timer.IsCompleted.Value);
                Assert.AreEqual(timer.Current, timer.Limit, Delta);
            }
            catch (Exception e)
            {
                timer.Stop();
                throw e;
            }
        }
    }
}