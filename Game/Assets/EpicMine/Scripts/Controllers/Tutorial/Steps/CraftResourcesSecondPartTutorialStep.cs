using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class CraftResourcesSecondPartTutorialStep : TutorialStepBase
    {
        public CraftResourcesSecondPartTutorialStep(bool isComplete) : base(TutorialStepIds.CraftResourcesSecondPart, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }

        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.CraftResourcesFirstPart))
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

        private void FixCheck()
        {
           
            var slot = App.Instance.Player.Workshop.Slots.FirstOrDefault();
            if (slot != null && slot.StaticRecipe == null)
            {
                Debug.Log(Id + " fix ");
               // var rec = App.Instance.Player.Workshop.Recipes.FirstOrDefault();
                var resource = App.Instance.StaticData.Resources.First();

                if (!App.Instance.Player.Inventory.Has(new Item(resource.Id, 50)))
                {
                    Debug.Log(Id + " dont have res, added it ");
                    App.Instance.Player.Inventory.Add(new Item(resource.Id, 50), IncomeSourceType.FromCraft);
                }
                else
                {
                    Debug.Log("we have res");
                }

              //  slot.Start(rec, 5);
            }
        }

        protected override void OnReady()
        {
            FixCheck();

            var window = WindowManager.Instance.Show<WindowCraftResourcesSecondPartTutorialStepAssistant>(withSound: false);
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

        private void OnStepComplete(TutorialStepCompleteEvent eventData)
        {
            if (eventData.Step.Id == TutorialStepIds.CraftResourcesFirstPart)
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