using System.Linq;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public class StartChestBreakingTutorialStep : TutorialStepBase
    {
        public StartChestBreakingTutorialStep(bool isComplete) : base(TutorialStepIds.StartChestBreaking, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.FindChest))
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

        private void FixCheck()
        {
            var chest = App.Instance.Player.Burglar.Chests.FirstOrDefault();
            if (chest == null)
            {
                App.Instance.Player.Burglar.AddChest(ChestType.Simple,0, (isOk) =>
                {
                    OnReady();
                });
            }
        }

        protected override void OnReady()
        {
            var chest = App.Instance.Player.Burglar.Chests.FirstOrDefault();
            if (chest == null)
            {
                FixCheck();
                return;
            }
            var window = WindowManager.Instance.Show<WindowStartChestBreakingTutorialStepAssistant>(withSound: false);
            window.Initialize(SetComplete);
        }

        protected override void OnComplete()
        {
            Unsubscribe();
            WindowManager.Instance.Close<WindowChestInfo>();
        }


        private void OnSceneChange(string from, string to)
        {
            if (to == ScenesNames.Village)
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