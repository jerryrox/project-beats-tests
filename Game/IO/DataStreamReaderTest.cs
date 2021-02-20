using System;
using System.IO;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.IO
{
    public class DataStreamReaderTest {
        
        [UnityTest]
        public IEnumerator TestSinglePoolSize()
        {
            var dataReader = new DataStreamReader<DummyData>(() => new DummyData(), 1);
            dataReader.StopStream();
            Assert.Throws<Exception>(() => dataReader.ReadData());
            Assert.Throws<Exception>(() => dataReader.PeekData());
            Assert.Throws<ArgumentNullException>(() => dataReader.StartStream(null));
            using (MemoryStream memStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memStream))
                {
                    new DummyData()
                    {
                        Num = 1,
                        Str = "z"
                    }.WriteStreamData(writer);
                    new DummyData()
                    {
                        Num = 2,
                        Str = "x"
                    }.WriteStreamData(writer);
                    new DummyData()
                    {
                        Num = 3,
                        Str = "c"
                    }.WriteStreamData(writer);
                    writer.Flush();

                    memStream.Position = 0;
                    using (BinaryReader reader = new BinaryReader(memStream))
                    {
                        dataReader.StartStream(reader);
                        yield return new WaitForSecondsRealtime(0.1f);

                        Assert.AreEqual(1, dataReader.BufferedCount);
                        var peeked = dataReader.PeekData();
                        Assert.AreEqual(1, dataReader.BufferedCount);
                        Assert.AreEqual(1, peeked.Num);
                        Assert.AreEqual("z", peeked.Str);

                        dataReader.AdvanceIndex();
                        Assert.AreEqual(0, dataReader.BufferedCount);
                        yield return new WaitForSecondsRealtime(0.1f);

                        Assert.AreEqual(1, dataReader.BufferedCount);
                        var read = dataReader.ReadData();
                        Assert.AreEqual(0, dataReader.BufferedCount);
                        Assert.AreEqual(2, read.Num);
                        Assert.AreEqual("x", read.Str);
                        yield return new WaitForSecondsRealtime(0.1f);

                        Assert.AreEqual(1, dataReader.BufferedCount);
                        read = dataReader.ReadData();
                        Assert.AreEqual(0, dataReader.BufferedCount);
                        Assert.AreEqual(3, peeked.Num);
                        Assert.AreEqual("c", peeked.Str);

                        yield return new WaitForSecondsRealtime(0.1f);
                        dataReader.StopStream();
                    }
                }
            }
        }
    }
}