using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class CreatePickaxeFirstPartTutorialStep : TutorialStepBase
    {
        public CreatePickaxeFirstPartTutorialStep(bool isComplete) : base(TutorialStepIds.CreatePickaxeFirstPart, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }

        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BlacksmithGreeting))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Village)
                return;

            if (!WindowManager.Instance.IsOpen<WindowBlacksmith>())
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
            var window = WindowManager.Instance.Show<WindowCreatePickaxeFirstPartTutorialStepAssistant>(withSound: false);
            window.Initialize(SetComplete);
        }

        protected override void OnComplete()
        {
            Unsubscribe();
        }


        private void OnWindowOpen(WindowOpenEvent eventData)
        {
            if (eventData.Window is WindowBlacksmith)
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