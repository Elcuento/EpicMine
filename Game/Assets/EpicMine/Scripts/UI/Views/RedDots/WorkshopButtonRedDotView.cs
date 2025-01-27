using System.Collections.Generic;
using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public class WorkshopButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            App.Instance.Controllers.RedDotsController.OnRecipesChange += OnRecipesChange;
            EventManager.Instance.Subscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
            EventManager.Instance.Subscribe<WorkshopSlotClearEvent>(OnSlotClear);
        }

        private void Start()
        {
            Calculate();
        }

        private void OnDestroy()
        {
            if (App.Instance != null)
                App.Instance.Controllers.RedDotsController.OnRecipesChange -= OnRecipesChange;

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
                EventManager.Instance.Unsubscribe<WorkshopSlotClearEvent>(OnSlotClear);
            }
        }

        private void OnRecipesChange(List<string> unlockedRecipes)
        {
            Calculate();
        }

        private void OnSlotClear(WorkshopSlotClearEvent eventData)
        {
            Calculate();
        }

        private void OnSlotComplete(WorkshopSlotCompleteEvent eventData)
        {
            Calculate();
        }

        private void Calculate()
        {
            var slotsMeltingCompletedCount = 0;
            var unlockedRecipesCount = App.Instance.Controllers.RedDotsController.NewRecipes.Count;

            foreach (var workshopSlot in App.Instance.Player.Workshop.Slots)
            {
                if (workshopSlot.IsComplete)
                    slotsMeltingCompletedCount++;
            }

            Show(slotsMeltingCompletedCount + unlockedRecipesCount);
        }
    }
}