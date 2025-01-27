using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class VillageSceneCheckCanTakeUnlockSecondTierGift : MonoBehaviour
    {

        private void OnTutorialCompleted(TutorialStepCompleteEvent eventData)
        {
            if (eventData.Step.Id == TutorialStepIds.EnceladAppear)
            {
                TakeGift();
            }
        }

        private void Start()
        {
            EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnTutorialCompleted);

            if (App.Instance.Player.AdditionalInfo.IsUnlockSecondTierGiftTaken)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.EnceladAppear))
                return;

            TakeGift();
        }

        private void TakeGift()
        {
            var staticData = App.Instance.StaticData;

            if (App.Instance.Player.AdditionalInfo.IsUnlockSecondTierGiftTaken)
            {
                Debug.LogError("Gift already taken");
                return;
            }

            var giftCrystalsAmount = staticData.Configs.CustomGifts.UnlockSecondTierCrystalsAmount;

            App.Instance.Player.AdditionalInfo.IsUnlockSecondTierGiftTaken = true;
            App.Instance.Player.Wallet.Add(CurrencyType.Crystals, giftCrystalsAmount,IncomeSourceType.FromGift);

            var currency = new Currency(CurrencyType.Crystals, giftCrystalsAmount);

            var window = WindowManager.Instance.Show<WindowCustomGift>();
            window.Initialize(currency, "unlock_second_tier_gift_header");
        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnTutorialCompleted);
        }
    }
}