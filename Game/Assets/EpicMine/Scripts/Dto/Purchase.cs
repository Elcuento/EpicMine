

using CommonDLL.Static;

namespace BlackTemple.EpicMine.Assets.EpicMine.Scripts.Dto
{
    public struct Purchase
    {
        public string shopId;
        public string receipt;
        public string date;
        public ShopPurchaseStatus status;

        public Purchase(string shopId, string receipt, string date, ShopPurchaseStatus status)
        {
            this.shopId = shopId;
            this.receipt = receipt;
            this.date = date;
            this.status = status;
        }
    } 
}
