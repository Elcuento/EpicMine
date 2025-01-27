using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class CreatePickaxeSecondPartTutorialStep : TutorialStepBase
    {
        public CreatePickaxeSecondPartTutorialStep(bool isComplete) : base(TutorialStepIds.CreatePickaxeSecondPart, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }

        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.CraftResourcesSecondPart))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Village)
                return;

            if (!WindowManager.Instance.IsOpen<WindowBlacksmith>())
            {
                var villageSceneController = Object.FindObjectOfType<VillageSceneController>();
                if (villageSceneController != null)
                    villageSceneController.ScrollToBlacksmith();
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
            WindowManager.Instance.Show<WindowBlacksmith>();

            var pickaxe = App.Instance.Player.Blacksmith.Pickaxes.First(
                p => p.StaticPickaxe.Type == PickaxeType.Blacksmith
                && p.StaticPickaxe.RequiredTierNumber == 1
                && p.StaticPickaxe.Hilt != string.Empty);

  
            if (!pickaxe.IsCreated)
            {
                var ingredients = StaticHelper.GetIngredients(pickaxe.StaticPickaxe);

                if (!App.Instance.Player.Inventory.Has(ingredients))
                {
                    foreach (var ing in ingredients)
                    {
                        if (!App.Instance.Player.Inventory.Has(ing))
                        {
                            App.Instance.Player.Inventory.Add(new Item(ing.Id, ing.Amount),
                                IncomeSourceType.FromTutorial);
                        }
                    }
                }

                var staticHilt = App.Instance.StaticData.Hilts.First(h => h.Id == pickaxe.StaticPickaxe.Hilt);
                var hilt = new Item(staticHilt.Id, 1);

                if (!App.Instance.Player.Inventory.Has(hilt))
                {
                    App.Instance.Player.Inventory.Add(hilt, IncomeSourceType.FromCustomGift);
                    var windowGift = WindowManager.Instance.Show<WindowCustomGift>();
                    windowGift.Initialize(hilt, "tutorial_step_create_pickaxe_first_part_4", onClose: ShowWindowAssistant);
                    return;
                }
            }

            ShowWindowAssistant();
        }

        private void ShowWindowAssistant()
        {
            var window = WindowManager.Instance.Show<WindowCreatePickaxeSecondPartTutorialStepAssistant>(withSound: false);
            window.Initialize(OnPickaxeSelected);
        }

        private void OnPickaxeSelected()
        {
            SetComplete();
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_create_pickaxe_second_part");
            var windowDialogue = WindowManager.Instance.Show<WindowDialogue>();
            windowDialogue.Initialize(dialogue, OnCloseDialogue);
        }

        private void OnCloseDialogue()
        {
            var villageSceneController = Object.FindObjectOfType<VillageSceneController>();
            if (villageSceneController != null)
                villageSceneController.ScrollToMine();
        }

        protected override void OnComplete()
        {
            Unsubscribe();
            WindowManager.Instance.Close<WindowWorkshop>();
            WindowManager.Instance.Close<WindowBlacksmith>();
        }


        private void OnStepComplete(TutorialStepCompleteEvent eventData)
        {
            if (eventData.Step.Id == TutorialStepIds.CraftResourcesSecondPart)
                CheckReady();
        }

        private void OnSceneChange(string from, string to)
        {
            if (to == ScenesNames.Village)
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