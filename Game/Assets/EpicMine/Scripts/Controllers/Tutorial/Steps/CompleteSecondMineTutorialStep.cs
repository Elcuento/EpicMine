using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public sealed class CompleteSecondMineTutorialStep : TutorialStepBase
    {
        public CompleteSecondMineTutorialStep(bool isComplete) : base(TutorialStepIds.CompleteSecondMine, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }

        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.CreatePickaxeFirstPart))
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
            var villageSceneController = Object.FindObjectOfType<VillageSceneController>();
            if (villageSceneController != null)
                villageSceneController.ScrollToMine();
        }

        protected override void OnComplete()
        {
            Unsubscribe();
        }


        private void OnTutorialStepComplete(TutorialStepCompleteEvent eventData)
        {
            if (eventData.Step.Id == TutorialStepIds.CreatePickaxeFirstPart)
                CheckReady();
        }

        private void OnMineComplete(MineCompleteEvent eventData)
        {
            if (eventData.Mine.Tier.Number == 0 && eventData.Mine.Number == 1)
                SetComplete();
        }

        private void OnSceneChange(string prev, string current)
        {
            if (current == ScenesNames.Village)
            {
                if (!IsComplete)
                {
                    var lastTier = App.Instance.Player.Dungeon.Tiers.FirstOrDefault(x=>x.IsOpen);
                    lastTier = lastTier ?? App.Instance.Player.Dungeon.Tiers.FirstOrDefault();

                    if(lastTier == null)
                    {
                        return;
                    }
                    var lastMine = lastTier.Number > 0 || lastTier.Mines.Count >= 1 
                                   && lastTier.Mines[1].IsComplete;

                    if (lastMine)
                        SetComplete();
                }
            }
        }

        private void Subscribe()
        {
            EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnTutorialStepComplete);
            EventManager.Instance.Subscribe<MineCompleteEvent>(OnMineComplete);
            SceneManager.Instance.OnSceneChange += OnSceneChange;
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnTutorialStepComplete);
                EventManager.Instance.Unsubscribe<MineCompleteEvent>(OnMineComplete);
                SceneManager.Instance.OnSceneChange -= OnSceneChange;
            }
        }
    }
}