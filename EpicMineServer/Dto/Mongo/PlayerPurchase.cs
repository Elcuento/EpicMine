using System.Collections.Generic;
using CommonDLL.Dto;
using MongoDB.Bson;

namespace AMTServer.Dto
{
    public class PlayerPurchase
    {
        public ObjectId Id;
        public string PlayerId;

        public List<Purchase> Purchases;

        public PlayerPurchase(string playerId, List<Purchase> list)
        {
            PlayerId = playerId;
            Id = ObjectId.GenerateNewId();
            Purchases = new List<Purchase>();
            Purchases = list ?? new List<Purchase>();
        }
    }
}
