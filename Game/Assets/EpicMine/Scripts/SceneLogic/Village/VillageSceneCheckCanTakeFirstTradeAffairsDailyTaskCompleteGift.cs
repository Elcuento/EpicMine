using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class VillageSceneCheckCanTakeFirstTradeAffairsDailyTaskCompleteGift : MonoBehaviour
    {
        private void Start()
        {
            if (App.Instance.Player.AdditionalInfo.IsFirstTradeAffairsDailyTaskCompleteGiftTaken)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks))
                return;

            EventManager.Instance.Subscribe<DailyTaskTakeEvent>(OnDailyTaskTaken);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<DailyTaskTakeEvent>(OnDailyTaskTaken);
        }

        private void OnDailyTaskTaken(DailyTaskTakeEvent eventData)
        {
            if (eventData.DailyTask.StaticTask.Type != DailyTaskType.TradeAffairs)
                return;

            var staticData = App.Instance.StaticData;

            if (App.Instance.Player.AdditionalInfo.IsFirstTradeAffairsDailyTaskCompleteGiftTaken)
            {
                Debug.LogError("Gift already taken");
                return ;
            }

            var giftCrystalsAmount =
                staticData.Configs.CustomGifts.FirstTradeAffairsDailyTaskCompleteCrystalsAmount;

            App.Instance.Player.AdditionalInfo.IsFirstTradeAffairsDailyTaskCompleteGiftTaken = true;

            App.Instance.Player.Wallet.Add(CurrencyType.Crystals, giftCrystalsAmount, IncomeSourceType.FromGift);

            EventManager.Instance.Unsubscribe<DailyTaskTakeEvent>(OnDailyTaskTaken);

            var window = WindowManager.Instance.Show<WindowCustomGift>();

            window.Initialize(new Currency(CurrencyType.Crystals, giftCrystalsAmount), "first_trade_affairs_daily_task_complete_gift_header");
        }

    }
}