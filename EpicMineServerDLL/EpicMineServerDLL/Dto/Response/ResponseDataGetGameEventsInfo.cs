using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataGetGameEventsInfo : SendData
    {
        public List<GameEvent> Data;

        public ResponseDataGetGameEventsInfo(List<GameEvent> news)
        {
            Data = news;
        }
    }
}