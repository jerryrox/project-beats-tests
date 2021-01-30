using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.IO
{
    public class DataStreamWriterTest {

        [UnityTest]
        public IEnumerator TestSinglePoolSize()
        {
            var dataWriter = new DataStreamWriter<DummyData>(1);
            dataWriter.StopStream();
            Assert.Throws<Exception>(() => dataWriter.WriteData(new DummyData()));
            Assert.Throws<ArgumentNullException>(() => dataWriter.StartStream(null));
            using (MemoryStream memStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(memStream))
                {
                    dataWriter.StartStream(writer);
                    dataWriter.WriteData(new DummyData()
                    {
                        Num = 100,
                        Str = "MyStringLol"
                    });
                    yield return new WaitForSecondsRealtime(0.1f);
                    dataWriter.WriteData(new DummyData()
                    {
                        Num = 101,
                        Str = "MyStringLol2"
                    });
                    dataWriter.StopStream();

                    memStream.Position = 0;
                    using (StreamReader reader = new StreamReader(memStream))
                    {
                        string content = reader.ReadToEnd();
                        Assert.AreEqual("100;MyStringLol\n101;MyStringLol2\n", content);
                    }
                }
            }
        }

        [UnityTest]
        public IEnumerator TestWrap()
        {
            var dataWriter = new DataStreamWriter<DummyData>(2);
            dataWriter.StopStream();
            Assert.Throws<Exception>(() => dataWriter.WriteData(new DummyData()));
            Assert.Throws<ArgumentNullException>(() => dataWriter.StartStream(null));
            using (MemoryStream memStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(memStream))
                {
                    dataWriter.StartStream(writer);
                    dataWriter.WriteData(new DummyData()
                    {
                        Num = 0,
                        Str = "a"
                    });
                    yield return new WaitForSecondsRealtime(0.1f);
                    dataWriter.WriteData(new DummyData()
                    {
                        Num = 1,
                        Str = "b"
                    });
                    dataWriter.WriteData(new DummyData()
                    {
                        Num = 2,
                        Str = "c"
                    });
                    yield return new WaitForSecondsRealtime(0.1f);
                    dataWriter.WriteData(new DummyData()
                    {
                        Num = 3,
                        Str = "d"
                    });
                    yield return new WaitForSecondsRealtime(0.1f);
                    dataWriter.WriteData(new DummyData()
                    {
                        Num = 4,
                        Str = "e"
                    });
                    dataWriter.StopStream();

                    memStream.Position = 0;
                    using (StreamReader reader = new StreamReader(memStream))
                    {
                        string content = reader.ReadToEnd();
                        Assert.AreEqual("0;a\n1;b\n2;c\n3;d\n4;e\n", content);
                    }
                }
            }
        }

        [UnityTest]
        public IEnumerator TestStressedPush()
        {
            var saver = new DataStreamWriter<DummyData>(250);
            using (MemoryStream memStream = new MemoryStream())
            {
                StringBuilder sb = new StringBuilder();
                using (StreamWriter writer = new StreamWriter(memStream))
                {
                    saver.StartStream(writer);
                    for (int r = 0; r < 5; r++)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            var data = new DummyData()
                            {
                                Num = r * 100 + i,
                                Str = "Lolz"
                            };
                            sb.AppendLine(data.ToStreamData());
                            saver.WriteData(data);
                        }
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                    saver.StopStream();

                    memStream.Position = 0;
                    using (StreamReader reader = new StreamReader(memStream))
                    {
                        string content = reader.ReadToEnd();
                        Assert.AreEqual(sb.ToString(), content);
                    }
                }
            }
        }
    }
}