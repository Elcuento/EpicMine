using System;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;
using CommonDLL.Static;
using Newtonsoft.Json;

public class ShopTriggerBuyGold : ShopTrigger
{
    public long CurrentGoldSpent;
    public long RequireGoldSpent;
    public long StartTime;

    public long TimeLeft => TimeManager.Instance != null
        ? StartTime - TimeManager.Instance.NowUnixSeconds
        : long.MaxValue;
    
    [JsonConstructor]
    public ShopTriggerBuyGold(long requireGoldSpent, long currentGoldSpent, long startTime, string offerId, bool isCompleted = false)
        : base(offerId, isCompleted)
    {
        RequireGoldSpent = requireGoldSpent;
        CurrentGoldSpent = currentGoldSpent;
        StartTime = startTime;
    }

    public ShopTriggerBuyGold(int requireGoldSpent, string offerId, bool isCompleted = false) 
        : base(offerId, isCompleted)
    {
        RequireGoldSpent = ShopHelper.GetCurrentGoldShopPackAmount(requireGoldSpent);
        var pack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == ShopPackId);

        StartTime = TimeManager.Instance.NowUnixSeconds +
                    (pack.Time ?? 72) * 60 * 60;
    }

    public override void OnStart()
    {
        EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuyShopPackEvent);
        EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBuyShopPackEvent);
    }

    public override void OnCheck()
    {
        var timeNow = TimeManager.Instance.NowUnixSeconds;
        if (timeNow > StartTime)
        {
            SetReset();
            return;
        }

        if (CurrentGoldSpent >= RequireGoldSpent)
        {
            SetCompleted();
        }
    }

    public override void OnCompleted()
    {}

    public override void OnReset()
    {
        CurrentGoldSpent = 0;
        RequireGoldSpent = ShopHelper.GetCurrentGoldShopPackAmount(ShopLocalConfig.BuyGoldTriggerRequireGold);

        var pack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == ShopPackId);
        StartTime = TimeManager.Instance.NowUnixSeconds + (pack.Time ?? 72) * 60 * 60;
    }

    public void OnBuyShopPackEvent(ShopBuyShopPackEvent eventData)
    {
        if (eventData.ShopPack.Id == ShopPackId)
        {
            SetReset();
        }
        else if (eventData.ShopPack.Type == ShopPackType.Gold)
        {
            var gold = eventData.ShopPack.Currency.FirstOrDefault(x => x.Key == CurrencyType.Gold);
            CurrentGoldSpent += ShopHelper.GetCurrentGoldShopPackAmount(gold.Value);
            SetCheck();
        }
    }

}
