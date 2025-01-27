using System;
using BlackTemple.EpicMine;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowNotEnoughCurrency : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _buttonText;

        private Action _onClose;

        public void Initialize(string descriptionText, string buttonText, Action onClose = null)
        {
            _descriptionText.text = LocalizationHelper.GetLocale(descriptionText);
            _buttonText.text = LocalizationHelper.GetLocale(buttonText);

            _onClose = onClose;
        }

        public void OnClickOk()
        {
            _onClose?.Invoke();
            Close();
        }

        public override void OnClose()
        {
            base.OnClose();

            _descriptionText.text = string.Empty;
            _buttonText.text = string.Empty;
        }
    }
}
