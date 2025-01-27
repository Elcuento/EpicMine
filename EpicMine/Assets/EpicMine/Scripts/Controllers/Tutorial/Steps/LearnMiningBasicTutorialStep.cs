using BlackTemple.Common;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public sealed class LearnMiningBasicTutorialStep : TutorialStepBase
    {
        private int _deathsInARow;

        public LearnMiningBasicTutorialStep(bool isComplete) : base(TutorialStepIds.LearnMiningBasic, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowCinematic))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Mine)
                return;

            if (_deathsInARow > 2)
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
            WindowManager.Instance.Show<WindowLearnMiningBasicTutorialStepAssistant>(withSound: false);
        }

        protected override void OnComplete()
        {
            Unsubscribe();
        }


        private void OnTutorialStepComplete(TutorialStepCompleteEvent eventData)
        {
            if (eventData.Step.Id == TutorialStepIds.ShowCinematic)
                CheckReady();
        }

        private void OnSceneChange(string from, string to)
        {
            if (to != ScenesNames.Mine)
                return;

            _deathsInARow++;
            CheckReady();
        }

        private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            if (eventData.Section is MineSceneBlacksmithSection)
                SetComplete();
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnTutorialStepComplete);
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            SceneManager.Instance.OnSceneChange += OnSceneChange;
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnTutorialStepComplete);
                EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            }

            if (SceneManager.Instance != null)
                SceneManager.Instance.OnSceneChange -= OnSceneChange;
        }
    }
}