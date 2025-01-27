using System;
using CommonDLL.Static;
using UnityEngine;


namespace BlackTemple.EpicMine.Core
{
    public class ShopSale
    {
        public ShopPackType Type { get; private set; }
        public string Id { get; private set; }
        public long Date { get; private set; }
        public int Charge { get; private set; }
        public int BuyCharge { get; private set; }

        public long TimeLeft => Date - TimeManager.Instance.NowUnixSeconds;

        public bool IsActive => Date - TimeManager.Instance.NowUnixSeconds > 0;

        public void Update(ShopSale data)
        {
            Date = data.Date;
            Id = data.Id;
            Charge = data.Charge;
            Type = data.Type;
            BuyCharge = data.BuyCharge;
        }


        public ShopSale(CommonDLL.Dto.ShopSale data)
        {
            Date = data.Date;
            Id = data.Id;
            Charge = data.Charge;
            BuyCharge = data.BuyCharge;
            Type = data.Type;
        }

        public ShopSale(string id, ShopPackType type, long date, int charge, int buyCharge)
        {
            Date = date;
            Id = id;
            Charge = charge;
            BuyCharge = buyCharge;
            Type = type;
        }

        public void SetCharge(int i)
        {
            Charge = i;
        }

        public void SetBuyCharge(int i)
        {
            BuyCharge = i;
        }

        public void SetDate(long dateNow)
        {
            Date = dateNow;
        }
    }
}