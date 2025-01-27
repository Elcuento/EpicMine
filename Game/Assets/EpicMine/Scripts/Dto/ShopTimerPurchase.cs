using Newtonsoft.Json;

namespace BlackTemple.EpicMine.Dto
{
    public struct ShopTimerPurchase
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("date")]
        public long Date;

        [JsonProperty("charge")]
        public int Charge;


        public ShopTimerPurchase(string id, long date, int charge)
        {
            Id = id;
            Date = date;
            Charge = charge;
        }
        public ShopTimerPurchase(Core.ShopTimerPurchase offer)
        {
            Id = offer.Id;
            Date = offer.Date;
            Charge = offer.Charge;
        }

    }
}