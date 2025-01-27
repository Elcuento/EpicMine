using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class Shop
    {
        public List<ShopOffer> ShopOffer;
        public List<ShopSale> ShopSale;
        public List<ShopSubscription> ShopSubscription;
        public List<ShopTimerPurchase> TimePurchase;

        public Shop()
        {
            ShopOffer = new List<ShopOffer>();
            ShopSale = new List<ShopSale>();
            ShopSubscription = new List<ShopSubscription>();
            TimePurchase = new List<ShopTimerPurchase>();
        }

        public Shop(BlackTemple.EpicMine.Core.Shop data)
        {
            ShopOffer = new List<ShopOffer>();
            ShopSale = new List<ShopSale>();
            ShopSubscription = new List<ShopSubscription>();
            TimePurchase = new List<ShopTimerPurchase>();

            foreach (var shopOffer in data.ShopOffer)
            {
                ShopOffer.Add(new ShopOffer()
                {
                    Date = shopOffer.Date,
                    Id = shopOffer.Id,
                    IsCompleted = shopOffer.IsCompleted
                });
            }
            foreach (var shopSale in data.ShopSale)
            {
                ShopSale.Add(new ShopSale()
                {
                    Date = shopSale.Date,
                    Id = shopSale.Id,
                    BuyCharge = shopSale.BuyCharge,
                    Charge = shopSale.Charge,
                    Type = shopSale.Type
                });
            }
            foreach (var timePurch in data.TimePurchase)
            {
                TimePurchase.Add(new ShopTimerPurchase()
                {
                    Date = timePurch.Date,
                    Id = timePurch.Id,
                    Charge = timePurch.Charge,
                });
            }

            foreach (var sub in data.ShopSubscription)
            {
                ShopSubscription.Add(new ShopSubscription()
                {
                    Date = sub.Date,
                    Id = sub.Id,
                });
            }
        }
    }
}