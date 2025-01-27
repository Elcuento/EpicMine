using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public class PvpButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            App.Instance.Controllers.RedDotsController.OnPvpWindowChange += OnRedDotPvpWindowChange;
        }

        private void Start()
        {
            Show(App.Instance.Controllers.RedDotsController.IsPvpWindowShowed ? 0 : 1);
        }

        private void OnDestroy()
        {
            if (App.Instance != null)
                App.Instance.Controllers.RedDotsController.OnPvpWindowChange -= OnRedDotPvpWindowChange;
        }

        private void OnRedDotPvpWindowChange(bool viewed)
        {
            Show(viewed ? 0 : 1);
        }
    }
}