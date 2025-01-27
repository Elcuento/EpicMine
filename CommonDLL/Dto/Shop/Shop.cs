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
    }
}