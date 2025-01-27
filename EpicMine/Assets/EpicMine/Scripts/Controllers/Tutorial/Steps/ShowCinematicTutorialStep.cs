using BlackTemple.Common;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public sealed class ShowCinematicTutorialStep : TutorialStepBase
    {
        public ShowCinematicTutorialStep(bool isComplete) : base(TutorialStepIds.ShowCinematic, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Mine)
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
            App.Instance.Services.AnalyticsService.StartTutorial();
            AudioManager.Instance.SetMusicMuted(true);
            AudioManager.Instance.SetSoundsMuted(true);

            var windowCinematic = WindowManager.Instance.Show<WindowVideo>(withSound: false);
            windowCinematic.Initialize("Cinematic", OnVideoEnded);
        }

        protected override void OnComplete()
        {
            Unsubscribe();
            AudioManager.Instance.SetMusicMuted(false);
            AudioManager.Instance.SetSoundsMuted(false);
        }


        private void OnVideoEnded()
        {
            OnOfferClosedOrRejected();
            // ShopHelper.ShowTutorialCrystalsOffer(OnOfferClosedOrRejected);
        }

        private void OnOfferClosedOrRejected()
        {
            SetComplete();
        }

        private void OnSceneChange(string from, string to)
        {
            if (to != ScenesNames.Mine)
                return;

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