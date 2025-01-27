using System.Linq;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class EnterSecondTierTutorialStep : TutorialStepBase
    {
        public EnterSecondTierTutorialStep(bool isComplete) : base(TutorialStepIds.EnterSecondTierTutorialStep, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.EnceladAppear))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Mine)
                return;

            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            if (selectedTier.Number != 1)
                return;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (selectedMine.Number != 0)
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
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_second_tier");
            var windowDialogue = WindowManager.Instance.Show<WindowDialogue>(withPause: true);
            windowDialogue.Initialize(dialogue, SetComplete);
        }

        protected override void OnComplete()
        {
            Unsubscribe();

            App.Instance.Services.AnalyticsService.CompleteTutorial();
        }


        private void OnSceneChange(string from, string to)
        {
            if (to == ScenesNames.Mine)
                CheckReady();
        }


        private void Subscribe()
        {
            SceneManager.Instance.OnSceneChange += OnSceneChange;
        }

        private void Unsubscribe()
        {
            if (SceneManager.Instance != null)
                SceneManager.Instance.OnSceneChange -= OnSceneChange;
        }
    }
}