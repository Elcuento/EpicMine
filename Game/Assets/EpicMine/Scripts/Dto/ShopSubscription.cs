using Newtonsoft.Json;

namespace BlackTemple.EpicMine.Dto
{
    public struct ShopSubscription
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("date")]
        public long Date;


        public ShopSubscription(Core.ShopSubscription data)
        {
            Date = data.Date;
            Id = data.Id;
        }

        public ShopSubscription(string id, long date)
        {
            Id = id;
            Date = date;
        }
    }
}