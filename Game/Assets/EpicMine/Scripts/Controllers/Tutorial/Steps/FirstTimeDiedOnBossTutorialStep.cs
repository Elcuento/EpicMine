using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;

namespace BlackTemple.EpicMine
{
    public class FirstTimeDiedOnBossTutorialStep : TutorialStepBase
    {
        public int PickaxeDestroyedCount { get; private set; }

        private const int TargetDamageLevel = 2;
        private const string IsGoldTakenPrefsKey = "FIRST_TIME_DIED_ON_BOSS_IS_GOLD_TAKEN";
        private int _hitCount;
        private readonly bool _isGoldTaken;


        public FirstTimeDiedOnBossTutorialStep(bool isComplete) : base(TutorialStepIds.FirstTimeDiedOnBoss, isComplete)
        {
            if (!IsComplete)
                Subscribe();

            _isGoldTaken = PlayerPrefs.GetInt(IsGoldTakenPrefsKey, 0) == 1;
        }


        public override void CheckReady()
        {
            if (IsComplete || IsReady)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting))
                return;

            if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier))
                return;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (selectedMine == null || !selectedMine.IsLast)
                return;

            if (App.Instance.Player.Skills.Damage.Number >= TargetDamageLevel)
            {
                SetComplete();
                return;
            }

            if (PickaxeDestroyedCount == TutorialLocalConfigs.FirstTimeDiedOnBossCount || _hitCount >= TutorialLocalConfigs.FirstTimeDiedOnBossHitCount || _isGoldTaken)
                SetReady();
        }

        public override void Clear()
        {
            base.Clear();
            Unsubscribe();
        }


        protected override void OnReady()
        {
            if (!_isGoldTaken)
            {
                var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_first_time_died_on_boss");
                var windowDialogue = WindowManager.Instance.Show<WindowDialogue>(withPause: true);
                windowDialogue.Initialize(dialogue, OnDialogueClose);
                return;
            }
            
            ShowWindowAssistant();
        }

        private void OnDialogueClose()
        {
            var giftedGold = new Currency(CurrencyType.Gold, TutorialLocalConfigs.FirstTimeDiedOnBossGiftedGold);
            var windowGift = WindowManager.Instance.Show<WindowCustomGift>(withPause: true);
            windowGift.Initialize(giftedGold, "tutorial_step_first_time_died_on_boss_4", onClose: () =>
            {
                App.Instance.Player.Wallet.Add(giftedGold, IncomeSourceType.FromCustomGift);
                PlayerPrefs.SetInt(IsGoldTakenPrefsKey, 1);
                ShowWindowAssistant();
            });
        }

        private void ShowWindowAssistant()
        {
            var windowAssistant = WindowManager.Instance.Show<WindowFirstTimeDiedOnBossTutorialStepAssistant>(withPause: true);
            windowAssistant.Initialize(SetComplete);
            FixCheck();
        }

        public void FixCheck()
        {
            App.Instance.Services.LogService.Log("Fix " + TutorialStepIds.FirstTimeDiedOnBoss);
            if (App.Instance.Player.Skills.Damage.NextStaticLevel.CostCurrencyType != null)
            {
                var gold = new Currency(CurrencyType.Gold,
                    App.Instance.Player.Skills.Damage.NextStaticLevel.CostCurrencyAmount);
                if (!App.Instance.Player.Wallet.Has(gold))
                    App.Instance.Player.Wallet.Add(gold, IncomeSourceType.FromCustomGift);
            }

            if (!string.IsNullOrEmpty(App.Instance.Player.Skills.Damage.NextStaticLevel.CostItemId))
            {
                var item = new Item(App.Instance.Player.Skills.Damage.NextStaticLevel.CostItemId,
                    App.Instance.Player.Skills.Damage.NextStaticLevel.CostItemAmount);
                if (!App.Instance.Player.Inventory.Has(item))
                    App.Instance.Player.Inventory.Add(item, IncomeSourceType.FromCustomGift);
            }
        }

        protected override void OnComplete()
        {
            Unsubscribe();
            PickaxeDestroyedCount = 0;
        }


        private void OnWindowOpen(WindowOpenEvent eventData)
        {
            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting))
                return;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (selectedMine == null || !selectedMine.IsLast)
                return;

            if (eventData.Window is WindowPickaxeDestroyed)
            {
                PickaxeDestroyedCount++;
                CheckReady();
            }
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<WindowOpenEvent>(OnWindowOpen);
            SceneManager.Instance.OnSceneChange += OnSceneChange;
        }

        private void OnSceneChange(string from, string to)
        {
            if (IsComplete)
                return;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            _hitCount = 0;
            if (to == ScenesNames.Mine && selectedMine != null && selectedMine.IsLast)
            {
                EventManager.Instance.Subscribe<MineSceneWallSectionHitEvent>(OnHit);
                CheckReady();
            }
            else
                EventManager.Instance.Unsubscribe<MineSceneWallSectionHitEvent>(OnHit);
        }

        private void OnHit(MineSceneWallSectionHitEvent eventData)
        {
            _hitCount++;
            CheckReady();
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<WindowOpenEvent>(OnWindowOpen);
                EventManager.Instance.Unsubscribe<MineSceneWallSectionHitEvent>(OnHit);
            }

            if (SceneManager.Instance != null)
                SceneManager.Instance.OnSceneChange -= OnSceneChange;
        }
    }
}