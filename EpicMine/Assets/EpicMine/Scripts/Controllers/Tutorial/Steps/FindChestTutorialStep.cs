using BlackTemple.Common;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public class FindChestTutorialStep : TutorialStepBase
    {
        public FindChestTutorialStep(bool isComplete) : base(TutorialStepIds.FindChest, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.CreatePickaxeSecondPart))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Mine)
                return;

            if (!WindowManager.Instance.IsOpen<WindowMineChest>())
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
            var window = WindowManager.Instance.Show<WindowFindChestTutorialStepAssistant>(withSound: false);
            window.Initialize(SetComplete);
        }

        protected override void OnComplete()
        {
            Unsubscribe();
        }


        private void OnWindowOpen(WindowOpenEvent eventData)
        {
            if (eventData.Window is WindowMineChest)
                CheckReady();
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