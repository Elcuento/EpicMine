using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct ShopBuyShopPackEvent
    {
        public ShopPack ShopPack;

        public ShopBuyShopPackEvent(ShopPack shopPack)
        {
            ShopPack = shopPack;
        }
    }
}