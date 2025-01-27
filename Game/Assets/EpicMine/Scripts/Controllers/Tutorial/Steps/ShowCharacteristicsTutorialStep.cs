using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public class ShowCharacteristicsTutorialStep : TutorialStepBase
    {
        public ShowCharacteristicsTutorialStep(bool isComplete) : base(TutorialStepIds.ShowCharacteristics, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.PickUpGiftedArtefacts))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Village)
                return;

            if (WindowManager.Instance.IsOpen<WindowCustomGift>())
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
            var window = WindowManager.Instance.Show<WindowDialogue>();
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_show_characteristics");
            window.Initialize(dialogue, OnCloseDialogue);
        }

        private void OnCloseDialogue()
        {
            var window = WindowManager.Instance.Show<WindowShowCharacteristicsTutorialStepAssistant>(withSound: false);
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

        private void OnWindowClose(WindowCloseEvent eventData)
        {
            if (eventData.Window is WindowCustomGift)
                CheckReady();
        }


        private void Subscribe()
        {
            SceneManager.Instance.OnSceneChange += OnSceneChange;
            EventManager.Instance.Subscribe<WindowCloseEvent>(OnWindowClose);
        }

        private void Unsubscribe()
        {
            if (SceneManager.Instance != null)
                SceneManager.Instance.OnSceneChange -= OnSceneChange;

            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<WindowCloseEvent>(OnWindowClose);
        }
    }
}