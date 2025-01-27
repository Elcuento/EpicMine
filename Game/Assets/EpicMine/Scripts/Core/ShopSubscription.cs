using System;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class ShopSubscription
    {
        public string Id { get; private set; }
        public long Date { get; private set; }

        public long TimeLeft => Date - TimeManager.Instance.NowUnixSeconds;

        public bool IsActive => Date - TimeManager.Instance.NowUnixSeconds > 0;

        public void Update(Dto.ShopSubscription data)
        {
            Date = data.Date;
            Id = data.Id;
        }

        public ShopSubscription(Dto.ShopSubscription data, string id)
        {
            Date = data.Date;
            Id = id;
        }

        public ShopSubscription(CommonDLL.Dto.ShopSubscription data)
        {
            Date = data.Date;
            Id = data.Id;
        }

        public ShopSubscription(string id, long date)
        {
            Date = date;
            Id = id;
        }

        public void Deactivate()
        {
            Date = TimeManager.Instance.NowUnixSeconds;
        }
    }
}