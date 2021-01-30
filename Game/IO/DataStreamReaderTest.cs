using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBGame.IO
{
    public class DataStreamReaderTest {
        
        [UnityTest]
        public IEnumerator TestSinglePoolSize()
        {
            var dataReader = new DataStreamReader<DummyData>(1);
            dataReader.StopStream();
            Assert.Throws<Exception>(() => dataReader.ReadData());
            Assert.Throws<Exception>(() => dataReader.PeekData());
            Assert.Throws<ArgumentNullException>(() => dataReader.StartStream(null));
            using (MemoryStream memStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(memStream))
                {
                    writer.WriteLine(new DummyData()
                    {
                        Num = 1,
                        Str = "z"
                    }.ToStreamData());
                    writer.WriteLine(new DummyData()
                    {
                        Num = 2,
                        Str = "x"
                    }.ToStreamData());
                    writer.WriteLine(new DummyData()
                    {
                        Num = 3,
                        Str = "c"
                    }.ToStreamData());
                    writer.Flush();

                    memStream.Position = 0;
                    using (StreamReader reader = new StreamReader(memStream))
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