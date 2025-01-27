using System;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowCraftResourcesFirstPartTutorialStepAssistant : WindowBase
    {
        [SerializeField] private GameObject _background;

        [SerializeField] private GameObject _container0;
        [SerializeField] private GameObject _container1;
        [SerializeField] private GameObject _container2;
        [SerializeField] private GameObject _container3;

        private Action _onClose;


        public void Initialize(Action onClose)
        {
            _onClose = onClose;
            Clear();
            ShowButtonOpenWorkshop();

       }


        public void ClickOpenWorkshop()
        {
            WindowManager.Instance.Show<WindowWorkshop>();
            ShowButtonShowRecipes();
        }

        public void ClickButtonShowRecipes()
        {
            var slots = FindObjectsOfType<WindowWorkshopSlot>().ToList().OrderBy(x=>x.WorkshopSlot.Number).ToList();

            foreach (var windowWorkshopSlot in slots)
            {
                Debug.Log(windowWorkshopSlot.WorkshopSlot.Number +":" + windowWorkshopSlot.WorkshopSlot.IsUnlocked);
            }
            var slot = slots.First(s => s.WorkshopSlot.IsUnlocked);
            slot.ShowWindowRecipes();
            Clear();

            _background.SetActive(false);
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_craft_resources_first_part");
            var windowDialogue = WindowManager.Instance.Show<WindowDialogue>();
            windowDialogue.Initialize(dialogue, ShowButtonChooseRecipe);
        }

        public void ClickButtonChooseRecipe()
        {
            var recipes = FindObjectsOfType<WindowRecipesRecipe>();
            var recipe = recipes.First(r => r.Recipe.StaticRecipe.Id == "stone_ingot");
            recipe.Create();

            Clear();
            ShowButtonChooseRecipeAmount();
        }

        public void ClickButtonChooseRecipeAmount()
        {
            var window = FindObjectOfType<WindowChooseRecipeAmount>();
            window.Create();
            Clear();
        }


        private void Clear()
        {
            _container0.SetActive(false);
            _container1.SetActive(false);
            _container2.SetActive(false);
            _container3.SetActive(false);
        }


        private void ShowButtonOpenWorkshop()
        {
            _container0.SetActive(true);
        }

        private void ShowButtonShowRecipes()
        {
            _container1.SetActive(true);
        }

        private void ShowButtonChooseRecipe()
        {
            _background.SetActive(true);
            _container2.SetActive(true);
        }

        private void ShowButtonChooseRecipeAmount()
        {
            _container3.SetActive(true);
        }

        private void OnSlotStartMelting(WorkshopSlotStartMeltingEvent eventData)
        {
            _onClose?.Invoke();
            WindowManager.Instance.Close(this, withSound: false);
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            var slot = App.Instance.Player.Workshop.Slots.FirstOrDefault();

            if (slot?.StaticRecipe != null)
            {
                if (slot.IsComplete)
                {
                    _onClose();
                    return;
                }
            }

            EventManager.Instance.Subscribe<WorkshopSlotStartMeltingEvent>(OnSlotStartMelting);
        }

        public override void OnClose()
        {
            base.OnClose();
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<WorkshopSlotStartMeltingEvent>(OnSlotStartMelting);
            }
        }
    }
}