using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataGetLeaderBoard : SendData
    {
        public List<PlayerMineRating> Players;

        public ResponseDataGetLeaderBoard(List<PlayerMineRating> players)
        {
            Players = players;
        }
    }
}