using CommonDLL.Static;
using UnityEngine.Purchasing;

namespace BlackTemple.EpicMine
{
    public struct IapPurchaseCompleteEvent
    {
        public Product Product;
        public ShopPack Pack;
        public string Receipt;

        public IapPurchaseCompleteEvent(Product product = null, ShopPack pack = null, string receipt = "")
        {
            Product = product;
            Pack = pack;
            Receipt = receipt;
        }
    }
}