
using CommonDLL.Static;
using Newtonsoft.Json;

namespace BlackTemple.EpicMine.Dto
{
    public struct ShopSale
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("date")]
        public long Date;

        [JsonProperty("charge")]
        public int Charge;

        [JsonProperty("buyCharge")]
        public int BuyCharge;

        [JsonProperty("type")]
        public ShopPackType Type;

        public ShopSale(Core.ShopSale data)
        {
            Date = data.Date;
            Id = data.Id;
            Charge = data.Charge;
            BuyCharge = data.BuyCharge;
            Type = data.Type;
        }

        public ShopSale(string id, ShopPackType type, long date, int charge, int buyCharge)
        {
            Id = id;
            Date = date;
            Charge = charge;
            BuyCharge = buyCharge;
            Type = type;
        }

 
    }
}