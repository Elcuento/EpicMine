using System;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class Gifts
    {
        public bool IsGiftExists => OpenedCount < App.Instance.StaticData.Configs.Gifts.DailyCount;

        public bool IsCanOpen
        {
            get
            {
                if (!IsGiftExists)
                    return false;

                return TimeManager.Instance.NowUnixSeconds >= LastOpenTime + App.Instance.StaticData.Configs.Gifts.PeriodInMinutes * 60;
            }
        }

        public long ReadyTime
        {
            get
            {
                var secondLeft = LastOpenTime + (App.Instance.StaticData.Configs.Gifts.PeriodInMinutes * 60) -
                                 TimeManager.Instance.NowUnixSeconds;

                if (secondLeft <= 0)
                    secondLeft = 0;

                return secondLeft;
            }
        }

        public long LastOpenTime { get; private set; }

        public int OpenedCount { get; private set; }



        public Gifts(CommonDLL.Dto.Gifts giftsData)
        {
            var now = TimeManager.Instance.NowUnixSeconds;

            if (now > giftsData.LastOpenTime + 60 * 60 * 24)
            {
                LastOpenTime = 0;
                OpenedCount = 0;
            }
            else
            {
                LastOpenTime = giftsData.LastOpenTime;
                OpenedCount = giftsData.OpenedCount;
            }
        }


        public void Open(Action<bool, Pack> onComplete = null)
        {
            var giftNumber = OpenedCount;

            var staticData = App.Instance.StaticData;

            var periodInMinutes = staticData.Configs.Gifts.PeriodInMinutes;
            var dailyCount = staticData.Configs.Gifts.DailyCount;
            var simpleArtefactsCount = staticData.Configs.Gifts.SimpleArtefactsCount;
            var royalArtefactsCount = staticData.Configs.Gifts.RoyalArtefactsCount;
            var maxArtefactsCount = (long)staticData.Configs.Dungeon.TierOpenArtefactsCost;

            var now = TimeManager.Instance.NowUnixSeconds;

            var lastOpenTime = App.Instance.Player.Gifts.LastOpenTime;

            var openedCount = App.Instance.Player.Gifts.OpenedCount;

            if (lastOpenTime + 60 * 60 * 24 < now)
            {
                openedCount = 0;
            }
            else if (lastOpenTime + 60 * periodInMinutes < now)
            {
                if (App.Instance.Player.Gifts.OpenedCount > dailyCount)
                {
                    openedCount = 0;
                }
            }

            openedCount++;

            var artefactsCount = openedCount == dailyCount
                ? royalArtefactsCount
                : simpleArtefactsCount;

            long dropped = 0;

            if (App.Instance.Player.Artefacts.Amount + artefactsCount > maxArtefactsCount)
                dropped = maxArtefactsCount - App.Instance.Player.Artefacts.Amount;

            App.Instance.Player.Artefacts.Add(dropped);

            App.Instance.Player.Gifts.OpenedCount = openedCount;
            App.Instance.Player.Gifts.LastOpenTime = TimeManager.Instance.NowUnixSeconds;

            OpenedCount++;
            LastOpenTime = TimeManager.Instance.NowUnixSeconds;

            var pack = StaticHelper.GetGiftRandomDrop(giftNumber);
            pack.Add(dropped);

            var giftOpenEvent = new GiftOpenEvent();
            EventManager.Instance.Publish(giftOpenEvent);

            onComplete?.Invoke(true, pack);
        }
    }
}