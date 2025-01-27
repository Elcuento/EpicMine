using MongoDB.Bson;

namespace AMTServer.Dto
{
    public class PlayerMineRating
    {
        public ObjectId Id;
        public CommonDLL.Dto.PlayerMineRating Rating;

        public PlayerMineRating(CommonDLL.Dto.PlayerMineRating rating)
        {
            Id = ObjectId.GenerateNewId();
            Rating = rating;
        }
    }
}
