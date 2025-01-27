using CommonDLL.Static;

namespace BlackTemple.EpicMine.Core
{
    public class ShopTimerPurchase
    {
        public string Id { get; }
        public long Date {get; private set; }
        public int Charge { get; private set; }

        public ShopPack StaticShopPack { get; }

        public long TimeLeft => Date - TimeManager.Instance.NowUnixSeconds;

        public bool IsActive => Date - TimeManager.Instance.NowUnixSeconds < 0 || Charge > 0;

        public ShopTimerPurchase(ShopPack shopPack, Dto.ShopTimerPurchase timePurchase)
        {
            Id = shopPack.Id;
            Date = timePurchase.Date;
            StaticShopPack = shopPack;
            Charge = timePurchase.Charge;
        }

        public ShopTimerPurchase(ShopPack shopPack, CommonDLL.Dto.ShopTimerPurchase timePurchase)
        {
            Id = shopPack.Id;
            Date = timePurchase.Date;
            StaticShopPack = shopPack;
            Charge = timePurchase.Charge;
        }
        public void RecalculateCharges()
        {
            var timeNow = TimeManager.Instance.NowUnixSeconds;

            if (Date < timeNow)
            {
                var totalLeftTime = timeNow - Date;
                var timeCost =  (StaticShopPack.Time ?? 1) * 60 * 60;

                while (Charge < StaticShopPack.Charge && totalLeftTime > timeCost)
                {
                    totalLeftTime -= timeCost;
                    Charge++;
                }
            }

            Charge = Charge;
        }

        public ShopTimerPurchase(ShopPack shopPack, long date, int charge)
        {
            Id = shopPack.Id;
            Charge = charge;
            Date = date;
            StaticShopPack = shopPack;
        }

        public void Update(ShopTimerPurchase shopTimerPurchase)
        {
            Date = shopTimerPurchase.Date;
            Charge = shopTimerPurchase.Charge;
        }

        public void SetCharge(int charge)
        {
            Charge = charge;
        }

        public void SetDate(long timeCost)
        {
            Date  = timeCost;
        }
    }
}