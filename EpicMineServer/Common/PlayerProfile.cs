using MongoDB.Bson;
using System.Collections.Generic;

namespace AMTServer.Common
{
    public class MongoNetProfiler
    {
        public ObjectId Id;
        public NetProfiler Profile;

        public MongoNetProfiler()
        {
            Profile = new NetProfiler();
        }
    }

    public class NetProfiler
    {
        public string UserNik;
        public string UserId;
        public int LikeCoins;
        public int FollowerCoins;
        public string ReferalCode;
        public List<Order> Orders;
        public List<string> FollowerOrders;
        public List<string> LikeOrders;
        public int Telegramm;
        public int FaceBook;
        public string TimeLastActivateBonus;
    }
}
