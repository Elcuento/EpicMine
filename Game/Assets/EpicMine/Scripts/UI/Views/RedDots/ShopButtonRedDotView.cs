using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public class ShopButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            App.Instance.Controllers.RedDotsController.OnShopChange += OnShopChange;
        }

        private void OnShopChange(List<string> unlockedShopPacks)
        {
            Show(unlockedShopPacks.Count);
        }

        private void Start()
        {
            Show(App.Instance.Controllers.RedDotsController.NewShopPacks.Count);
        }

        private void OnDestroy()
        {
            if (App.Instance != null)
                App.Instance.Controllers.RedDotsController.OnShopChange -= OnShopChange;
        }
    }
}