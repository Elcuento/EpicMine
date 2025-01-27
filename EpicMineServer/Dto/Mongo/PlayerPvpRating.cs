using MongoDB.Bson;

namespace AMTServer.Dto
{
    public class PlayerPvpRating
    {
        public ObjectId Id;
        public CommonDLL.Dto.PlayerPvpRating Rating;

        public PlayerPvpRating(CommonDLL.Dto.PlayerPvpRating rating)
        {
            Id = ObjectId.GenerateNewId();
            Rating = rating;
        }
    }
}
