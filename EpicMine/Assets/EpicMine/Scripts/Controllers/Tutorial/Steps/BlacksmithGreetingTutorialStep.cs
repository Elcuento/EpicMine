using System.Linq;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class BlacksmithGreetingTutorialStep : TutorialStepBase
    {
        public BlacksmithGreetingTutorialStep(bool isComplete) : base(TutorialStepIds.BlacksmithGreeting, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }

        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.LearnMiningBasic))
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
            var window = WindowManager.Instance.Show<WindowDialogue>();
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_blacksmith_greeting");
            window.Initialize(dialogue, OnCloseWindowDialogue);
        }

        protected override void OnComplete()
        {
            if (!WindowManager.Instance.IsOpen<WindowBlacksmith>())
            {
                var villageSceneController = Object.FindObjectOfType<VillageSceneController>();
                if (villageSceneController != null)
                    villageSceneController.ScrollToBlacksmith();
            }
            Unsubscribe();
        }


        private void OnSceneChange(string from, string to)
        {
            if (to != ScenesNames.Village)
                return;

            CheckReady();
        }

        private void OnCloseWindowDialogue()
        {
            SetComplete();
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