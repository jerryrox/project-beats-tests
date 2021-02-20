using System;
using System.IO;
using System.Text;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.IO
{
    public class DataStreamWriterTest {

        [UnityTest]
        public IEnumerator TestSinglePoolSize()
        {
            var dataWriter = new DataStreamWriter<DummyData>(() => new DummyData(), 1);
            dataWriter.StopStream();
            Assert.Throws<Exception>(() => dataWriter.WriteData(new DummyData()));
            Assert.Throws<ArgumentNullException>(() => dataWriter.StartStream(null));
            using (MemoryStream memStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memStream))
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
                    using (BinaryReader reader = new BinaryReader(memStream))
                    {
                        Assert.AreEqual(100, reader.ReadInt32());
                        Assert.AreEqual("MyStringLol", reader.ReadString());
                        Assert.AreEqual(101, reader.ReadInt32());
                        Assert.AreEqual("MyStringLol2", reader.ReadString());
                    }
                }
            }
        }

        [UnityTest]
        public IEnumerator TestWrap()
        {
            var dataWriter = new DataStreamWriter<DummyData>(() => new DummyData(), 2);
            dataWriter.StopStream();
            Assert.Throws<Exception>(() => dataWriter.WriteData(new DummyData()));
            Assert.Throws<ArgumentNullException>(() => dataWriter.StartStream(null));
            using (MemoryStream memStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memStream))
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
                    using (BinaryReader reader = new BinaryReader(memStream))
                    {
                        Assert.AreEqual(0, reader.ReadInt32());
                        Assert.AreEqual("a", reader.ReadString());
                        Assert.AreEqual(1, reader.ReadInt32());
                        Assert.AreEqual("b", reader.ReadString());
                        Assert.AreEqual(2, reader.ReadInt32());
                        Assert.AreEqual("c", reader.ReadString());
                        Assert.AreEqual(3, reader.ReadInt32());
                        Assert.AreEqual("d", reader.ReadString());
                        Assert.AreEqual(4, reader.ReadInt32());
                        Assert.AreEqual("e", reader.ReadString());
                    }
                }
            }
        }

        [UnityTest]
        public IEnumerator TestStressedPush()
        {
            var saver = new DataStreamWriter<DummyData>(() => new DummyData(), 250);
            using (MemoryStream memStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memStream))
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
                            saver.WriteData(data);
                        }
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                    saver.StopStream();

                    memStream.Position = 0;
                    using (BinaryReader reader = new BinaryReader(memStream))
                    {
                        for (int r = 0; r < 5; r++)
                        {
                            for (int i = 0; i < 100; i++)
                            {
                                Assert.AreEqual(r * 100 + i, reader.ReadInt32());
                                Assert.AreEqual("Lolz", reader.ReadString());
                            }
                        }
                    }
                }
            }
        }
    }
}