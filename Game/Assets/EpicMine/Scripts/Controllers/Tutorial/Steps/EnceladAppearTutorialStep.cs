using System.Linq;
using CommonDLL.Static;

// ReSharper disable IdentifierTypo

namespace BlackTemple.EpicMine
{
    public class EnceladAppearTutorialStep : TutorialStepBase
    {
        public EnceladAppearTutorialStep(bool isComplete) : base(TutorialStepIds.EnceladAppear, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier) 
                )
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Village)
                return;

            var lastTier = App.Instance.Player.Dungeon.LastOpenedTier;
            
            if (lastTier == null)
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
            
            var dialogue =
                App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_encelad_appear");
            var windowDialogue = WindowManager.Instance.Show<WindowDialogue>(withPause: true);
            windowDialogue.Initialize(dialogue, SetComplete);
        }

        private void OnSceneChange(string from, string to)
        {
            if (to == ScenesNames.Village)
                CheckReady();
        }

        protected override void OnComplete()
        {
            Unsubscribe();
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