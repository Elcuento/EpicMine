using System;
using System.Collections;
using BlackTemple.Common;
using CommonDLL.Dto;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowSellConfirm : WindowBase
    {
        [SerializeField] private ItemView _itemPrefab;
        [SerializeField] private RectTransform _itemsContainer;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Image _costIcon;
        [SerializeField] private Button _okButton;

        private Action _onClickOk;
        
        public void Initialize(Item item, Dto.Currency cost, Action onClickOk)
        {
            Clear();
            _onClickOk = onClickOk;
            Instantiate(_itemPrefab, _itemsContainer, false).Initialize(item);
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
            _itemsContainer.ClearChildObjects();
        }

        private IEnumerator UpdateCanvas()
        {
            yield return new WaitForEndOfFrame();
            _costText.text += " ";
        }
    }
}