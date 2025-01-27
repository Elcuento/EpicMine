using System.Collections.Generic;
using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public class TorchesButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            App.Instance.Controllers.RedDotsController.OnTorchesWindowChange += OnTorchesChange;
        }

        private void Start()
        {

            Show(App.Instance.Controllers.RedDotsController.IsTorchesWindowShowed ? 0 : 1);
        }

        private void OnDestroy()
        {
            if (App.Instance != null)
                App.Instance.Controllers.RedDotsController.OnTorchesWindowChange -= OnTorchesChange;
        }

        private void OnTorchesChange(bool viewed)
        {
            Show(viewed ? 0 : 1);
        }
    }
}