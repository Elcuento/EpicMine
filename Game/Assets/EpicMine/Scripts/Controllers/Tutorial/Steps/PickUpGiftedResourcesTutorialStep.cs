using CommonDLL.Dto;
using CommonDLL.Static;
using System.Linq;
using UnityEngine;


namespace BlackTemple.EpicMine
{
    public class PickUpGiftedResourcesTutorialStep : TutorialStepBase
    {
        public PickUpGiftedResourcesTutorialStep(bool isComplete) : base(TutorialStepIds.PickUpGiftedResources, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }

        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.CompleteSecondMine))
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
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_gift_resources");
            window.Initialize(dialogue, OnCloseWindowDialogue);
        }

        protected override void OnComplete()
        {
            Unsubscribe();
            var resource = App.Instance.StaticData.Resources.First();
            App.Instance.Player.Inventory.Add(new Item(resource.Id, 50), IncomeSourceType.FromCustomGift);

            var villageSceneController = Object.FindObjectOfType<VillageSceneController>();
            if (villageSceneController != null)
                villageSceneController.ScrollToWorkshop();
        }


        private void OnSceneChange(string from, string to)
        {
            if (to != ScenesNames.Village)
                return;

            CheckReady();
        }

        private void OnCloseWindowDialogue()
        {
            var window = WindowManager.Instance.Show<WindowCustomGift>();
            var resource = App.Instance.StaticData.Resources.First();
            window.Initialize(new Item(resource.Id, 50), "tutorial_step_gift_resources_2", onClose: SetComplete);
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