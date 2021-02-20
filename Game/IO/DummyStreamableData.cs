using System.IO;

namespace PBGame.IO
{
    public class DummyData : IStreamableData
    {
        public int Num;
        public string Str;

        public void WriteStreamData(BinaryWriter writer)
        {
            writer.Write(Num);
            writer.Write(Str);
        }

        public void ReadStreamData(BinaryReader reader)
        {
            Num = reader.ReadInt32();
            Str = reader.ReadString();
        }
    }
}