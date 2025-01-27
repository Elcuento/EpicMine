using System.Collections.Generic;
using MongoDB.Bson;

namespace AMTServer.Dto
{
    public class RatingInfo
    {
        public ObjectId Id;

        public long LastUpdate;
        public long Spend;
        public List<PlayerMineRating> NewBieTop;
        public List<PlayerMineRating> MineTop;
        public List<PlayerPvpRating> PvpTop;


        public RatingInfo()
        {
            Id = ObjectId.GenerateNewId();
            MineTop = new List<PlayerMineRating>();
            PvpTop = new List<PlayerPvpRating>();
            NewBieTop = new List<PlayerMineRating>();
        }
    }
}
