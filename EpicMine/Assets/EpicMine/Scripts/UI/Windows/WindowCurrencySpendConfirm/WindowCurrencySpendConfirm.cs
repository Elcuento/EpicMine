using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowCurrencySpendConfirm : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _okButtonText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Image _costIcon;
        [SerializeField] private Button _okButton;

        private Action _onClickOk;

        public void Initialize(Dto.Currency cost, Action onClickOk, string description, string okButtonText, bool isNeedLocalizeDescription = true, bool isNeedLocalizeOkButtonText = true)
        {
            Clear();

            _onClickOk = onClickOk;

            _description.text = isNeedLocalizeDescription ? LocalizationHelper.GetLocale(description) : description;
            _okButtonText.text = isNeedLocalizeOkButtonText ? LocalizationHelper.GetLocale(okButtonText) : okButtonText;

            _costText.text = cost.Amount.ToString();
            _costIcon.sprite = SpriteHelper.GetCurrencyIcon(cost.Type);

            StartCoroutine(UpdateCanvas());
        }

        private void Start()
        {
            _okButton.onClick.AddListener(OnClickOk);
        }

        private void OnClickOk()
        {
            _onClickOk?.Invoke();
            Close();
        }

        private void Clear()
        {
            _onClickOk = null;
            
            _costText.text = string.Empty;
            _costIcon.sprite = null;
            _description.text = string.Empty;
            _okButtonText.text = string.Empty;
        }

        private IEnumerator UpdateCanvas()
        {
            yield return new WaitForEndOfFrame();
            _costText.text += " ";
        }
    }
}