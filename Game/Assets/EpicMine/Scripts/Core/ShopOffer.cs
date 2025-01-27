using CommonDLL.Static;


namespace BlackTemple.EpicMine.Core
{
    public class ShopOffer
    {
        public string Id { get; }
        public long Date {get;}
        public bool IsCompleted;

        public ShopPack StaticShopPack { get; }

        public long TimeLeft => Date - TimeManager.Instance.NowUnixSeconds;

        public bool IsActive
        {
            get
            {
                if (IsCompleted)
                    return false;

                return Date - TimeManager.Instance.NowUnixSeconds > 0;

            }
        }

        public void SetCompleted()
        {
            IsCompleted = true;
        }

        public ShopOffer(ShopPack shopPack, ShopOffer offer)
        {
            Id = shopPack.Id;
            Date = offer.Date;
            IsCompleted = offer.IsCompleted;
            StaticShopPack = shopPack;
        }

        public ShopOffer(ShopPack shopPack, CommonDLL.Dto.ShopOffer offer)
        {
            Id = shopPack.Id;
            Date = offer.Date;
            IsCompleted = offer.IsCompleted;
            StaticShopPack = shopPack;
        }

        public ShopOffer(ShopPack shopPack, long date, bool isComplete = false)
        {
            Id = shopPack.Id;
            Date = date;
            IsCompleted = isComplete;
            StaticShopPack = shopPack;
        }
    }
}