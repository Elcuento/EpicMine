using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class CraftResourcesFirstPartTutorialStep : TutorialStepBase
    {
        public CraftResourcesFirstPartTutorialStep(bool isComplete) : base(TutorialStepIds.CraftResourcesFirstPart, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }

        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.PickUpGiftedResources))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Village)
                return;

            if (!WindowManager.Instance.IsOpen<WindowWorkshop>())
            {
                var villageSceneController = Object.FindObjectOfType<VillageSceneController>();
                if (villageSceneController != null)
                    villageSceneController.ScrollToWorkshop();
            }

            SetReady();
        }

        public override void Clear()
        {
            base.Clear();
            Unsubscribe();
        }

        protected override void OnReady()
        {
            var window = WindowManager.Instance.Show<WindowCraftResourcesFirstPartTutorialStepAssistant>(withSound: false);
            window.Initialize(SetComplete);
            FixCheck();
        }

        private void FixCheck()
        {
            var resource = App.Instance.StaticData.Resources.First();
            if (!App.Instance.Player.Inventory.Has(new Item(resource.Id, 50)))
            {
                App.Instance.Services.LogService.Log(Id + " fix ");
                App.Instance.Player.Inventory.Add(new Item(resource.Id, 50), IncomeSourceType.FromCustomGift);
            }
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
            if (eventData.Step.Id == TutorialStepIds.PickUpGiftedResources)
                CheckReady();
        }


        private void Subscribe()
        {
            SceneManager.Instance.OnSceneChange += OnSceneChange;
            EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnStepComplete);
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