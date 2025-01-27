using System;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowInformation : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _buttonText;

        private Action _onClose;


        public void Initialize(string headerText, string descriptionText, string buttonText,
            bool isNeedLocalizeHeader = true, bool isNeedLocalizeDescription = true, bool isNeedLocalizeButton = true,
            Action onClose = null)
        {
            _headerText.text = isNeedLocalizeHeader
                ? LocalizationHelper.GetLocale(headerText)
                : headerText;

            _descriptionText.text = isNeedLocalizeDescription
                ? LocalizationHelper.GetLocale(descriptionText)
                : descriptionText;

            _buttonText.text = isNeedLocalizeButton
                ? LocalizationHelper.GetLocale(buttonText)
                : buttonText;

            _onClose = onClose;
        }

        public void ChangeText(string headerText, string descriptionText)
        {
            _headerText.text = headerText;
            _descriptionText.text = descriptionText;
        }

        public void ChangeText(string descriptionText)
        {
            _descriptionText.text = descriptionText;
        }

        public override void OnClose()
        {
            base.OnClose();

            _headerText.text = string.Empty;
            _descriptionText.text = string.Empty;
            _buttonText.text = string.Empty;

            _onClose?.Invoke();
        }
    }
}