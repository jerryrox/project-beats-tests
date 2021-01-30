namespace PBGame.IO
{
    public class DummyData : IStreamableData
    {
        public int Num;
        public string Str;

        public string ToStreamData()
        {
            return $"{Num};{Str}";
        }

        public void FromStreamData(string data)
        {
            string[] splits = data.Split(';');
            Num = int.Parse(splits[0]);
            Str = splits[1];
        }
    }
}