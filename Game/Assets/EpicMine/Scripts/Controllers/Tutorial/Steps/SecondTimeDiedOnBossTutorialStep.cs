using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public class SecondTimeDiedOnBossTutorialStep : TutorialStepBase
    {
        public int PickaxeDestroyedCount { get; private set; }

        public SecondTimeDiedOnBossTutorialStep(bool isComplete) : base(TutorialStepIds.SecondTimeDiedOnBoss, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier))
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.FirstTimeDiedOnBoss))
                return;

            if (PickaxeDestroyedCount < TutorialLocalConfigs.SecondTimeDiedOnBossCount)
                return;

            SetReady();
        }

        public override void Clear()
        {
            base.Clear();
            Unsubscribe();
        }

        protected override void OnReady()
        {
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_second_time_died_on_boss");
            var windowDialogue = WindowManager.Instance.Show<WindowDialogue>(withPause: true);
            windowDialogue.Initialize(dialogue, OnDialogueClose);
        }

        private void OnDialogueClose()
        {
            var tnt = App.Instance.StaticData.Tnt.First();
            var giftedTnt = new Item(tnt.Id, TutorialLocalConfigs.SecondTimeDiedOnBossGiftedTnt);
            var windowGift = WindowManager.Instance.Show<WindowCustomGift>(withPause: true);
            windowGift.Initialize(giftedTnt, "tutorial_step_second_time_died_on_boss_3", onClose: SetComplete);
        }

        protected override void OnComplete()
        {
            Unsubscribe();

            var tnt = App.Instance.StaticData.Tnt.First();
            var giftedTnt = new Item(tnt.Id, TutorialLocalConfigs.SecondTimeDiedOnBossGiftedTnt);

            App.Instance.Player.Inventory.Add(giftedTnt, IncomeSourceType.FromCustomGift);
            PickaxeDestroyedCount = 0;
        }


        private void OnWindowOpen(WindowOpenEvent eventData)
        {
            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.FirstTimeDiedOnBoss))
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
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<WindowOpenEvent>(OnWindowOpen);
        }
    }
}