using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public class BlacksmithButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            App.Instance.Controllers.RedDotsController.OnBlacksmithChange += OnRedDotBlacksmithChange;
        }

        private void Start()
        {
            Show(App.Instance.Controllers.RedDotsController.NewPickaxes.Count);
        }

        private void OnDestroy()
        {
            if (App.Instance != null)
                App.Instance.Controllers.RedDotsController.OnBlacksmithChange -= OnRedDotBlacksmithChange;
        }

        private void OnRedDotBlacksmithChange(List<string> unlockedPickaxes)
        {
            Show(unlockedPickaxes.Count);
        }
    }
}