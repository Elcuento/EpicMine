using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public class UnlockTierTutorialStep : TutorialStepBase
    {
        public UnlockTierTutorialStep(bool isComplete) : base(TutorialStepIds.UnlockTier, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Mine)
                return;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (!selectedMine.IsLast)
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
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_unlock_tier");
            var windowDialogue = WindowManager.Instance.Show<WindowDialogue>(withPause: true);
            windowDialogue.Initialize(dialogue, SetComplete);
        }

        protected override void OnComplete()
        {
            Unsubscribe();
            App.Instance.Controllers.DailyTasksController.FillTasks();
        }


        private void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (selectedMine == null || !selectedMine.IsLast)
                return;

            if (eventData.Section is MineSceneLastDoorSection)
                CheckReady();
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
        }
    }
}