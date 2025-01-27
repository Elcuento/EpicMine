using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public class InventoryButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            App.Instance.Controllers.RedDotsController.OnInventoryChange += OnRedDotInventoryChange;
        }

        private void Start()
        {
            Calculate();
        }

        private void OnDestroy()
        {
            if (App.Instance != null)
                App.Instance.Controllers.RedDotsController.OnInventoryChange -= OnRedDotInventoryChange;
        }

        private void OnRedDotInventoryChange(List<string> viewedItems)
        {
            Calculate();
        }

        private void Calculate()
        {
            var count = 0;

            foreach (var item in App.Instance.Player.Inventory.Items)
            {
                if (!App.Instance.Controllers.RedDotsController.ViewedItems.Contains(item.Id))
                    count++;
            }

            Show(count);
        }
    }
}