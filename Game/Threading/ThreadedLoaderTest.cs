using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PBFramework.Threading;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.Threading.Tests
{
    public class ThreadedLoaderTest {
        
        [UnityTest]
        public IEnumerator Test()
        {
            ThreadedLoader<TestInput, TestOutput> loader = new ThreadedLoader<TestInput, TestOutput>((input) =>
            {
                Thread.Sleep(input.I * 100);
                return new TestOutput(input);
            });
            var inputs = new List<TestInput>()
            {
                new TestInput("a", 1),
                new TestInput("b", 2),
                new TestInput("c", 3),
            };
            Assert.Throws<Exception>(() => loader.StartLoad(0, inputs, outputs: null, listener: null));
            Assert.Throws<Exception>(() => loader.StartLoad(-1, inputs, outputs: null, listener: null));
            Assert.Throws<ArgumentNullException>(() => loader.StartLoad(1, null, outputs: null, listener: null));
            Assert.Throws<ArgumentException>(() => loader.StartLoad(1, inputs, outputs: new TestOutput[0], listener: null));
            Assert.Throws<ArgumentException>(() => loader.StartLoad(1, inputs, outputs: new TestOutput[inputs.Count - 1], listener: null));

            for (int taskCount = 1; taskCount < 4; taskCount++)
            {
                var listener = new TaskListener<TestOutput[]>();
                loader.StartLoad(taskCount, inputs, outputs: new TestOutput[inputs.Count], listener: listener);
                while (!listener.IsFinished)
                {
                    yield return null;
                }

                var outputs = listener.Value;
                Assert.AreEqual(inputs.Count, outputs.Length);
                Assert.AreEqual("a", inputs[0].Str);
                Assert.AreEqual(1, inputs[0].I);
                Assert.AreEqual(inputs[0].Str, outputs[0].Str);
                Assert.AreEqual(inputs[0].I, outputs[0].I);
                Assert.AreEqual("b", inputs[1].Str);
                Assert.AreEqual(2, inputs[1].I);
                Assert.AreEqual(inputs[1].Str, outputs[1].Str);
                Assert.AreEqual(inputs[1].I, outputs[1].I);
                Assert.AreEqual("c", inputs[2].Str);
                Assert.AreEqual(3, inputs[2].I);
                Assert.AreEqual(inputs[2].Str, outputs[2].Str);
                Assert.AreEqual(inputs[2].I, outputs[2].I);
            }
        }

        [UnityTest]
        public IEnumerator TestTimeout()
        {
            bool attemptedLoad = false;
            bool finishedLoad = false;
            ThreadedLoader<TestInput, TestOutput> loader = new ThreadedLoader<TestInput, TestOutput>((input) =>
            {
                attemptedLoad = true;
                Thread.Sleep(3000);
                finishedLoad = true;
                return new TestOutput(input);
            });
            loader.Timeout = 600;

            var listener = new TaskListener<TestOutput[]>();
            bool exceptionCaught = false;
            Task.Run(async () =>
            {
                try
                {
                    await loader.StartLoad(
                        1,
                        new List<TestInput>()
                        {
                        new TestInput("should fail", 0),
                        },
                        outputs: null,
                        listener: listener
                    );
                }
                catch (TimeoutException)
                {
                    exceptionCaught = true;
                }
            });
            yield return new WaitForSecondsRealtime(0.25f);
            Assert.IsTrue(attemptedLoad);
            Assert.IsFalse(finishedLoad);
            Assert.IsFalse(exceptionCaught);
            yield return new WaitForSecondsRealtime(1);
            Assert.IsTrue(attemptedLoad);
            Assert.IsFalse(finishedLoad);
            Assert.True(exceptionCaught);
            yield return new WaitForSecondsRealtime(2.5f);
            Assert.IsTrue(attemptedLoad);
            Assert.IsTrue(finishedLoad);
            Assert.True(exceptionCaught);
        }

        private class TestInput
        {
            public string Str;
            public int I;

            public TestInput(string str, int i)
            {
                this.Str = str;
                this.I = i;
            }
        }

        private class TestOutput
        {
            public string Str;
            public int I;

            public TestOutput(TestInput input)
            {
                this.Str = input.Str;
                this.I = input.I;
            }
        }
    }
}