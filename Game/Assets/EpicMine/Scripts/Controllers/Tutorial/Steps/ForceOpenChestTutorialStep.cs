using BlackTemple.Common;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public class ForceOpenChestTutorialStep : TutorialStepBase
    {
        public ForceOpenChestTutorialStep(bool isComplete) : base(TutorialStepIds.ForceOpenChest, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.StartChestBreaking))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Village)
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
            var window = WindowManager.Instance.Show<WindowForceOpenChestTutorialStepAssistant>(withSound: false);
            window.Initialize(SetComplete);
        }

        protected override void OnComplete()
        {
            Unsubscribe();
        }


        private void OnSceneChange(string from, string to)
        {
            if (to == ScenesNames.Village)
                CheckReady();
        }

        private void OnStepComplete(TutorialStepCompleteEvent eventData)
        {
            if (eventData.Step.Id == TutorialStepIds.StartChestBreaking)
                CheckReady();
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnStepComplete);
            SceneManager.Instance.OnSceneChange += OnSceneChange;
        }

        private void Unsubscribe()
        {
            if (SceneManager.Instance != null)
                SceneManager.Instance.OnSceneChange -= OnSceneChange;

            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnStepComplete);
        }
    }
}