using CommonDLL.Dto;

namespace BlackTemple.EpicMine
{
    public struct ShopSpecialOfferEvent
    {
        public ShopOffer Offer;

        public ShopSpecialOfferEvent(ShopOffer offer)
        {
            Offer = offer;
        }
    }
}