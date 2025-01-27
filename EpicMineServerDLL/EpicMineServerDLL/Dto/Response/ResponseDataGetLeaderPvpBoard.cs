using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataGetLeaderPvpBoard : SendData
    {
        public List<PlayerPvpRating> Players;

        public ResponseDataGetLeaderPvpBoard(List<PlayerPvpRating> players)
        {
            Players = players;
        }
    }
}