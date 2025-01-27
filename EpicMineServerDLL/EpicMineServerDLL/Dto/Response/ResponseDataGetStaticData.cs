using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class ResponseDataGetStaticData : SendData
    {
        public StaticData Data;

        public ResponseDataGetStaticData(StaticData data)
        {
            Data = data;
        }
    }
}