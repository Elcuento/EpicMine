using System;
using BlackTemple.Common;
using CommonDLL.Dto;
using TMPro;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;

namespace BlackTemple.EpicMine
{
    public class WindowCustomGift : WindowBase
    {
        [SerializeField] private ItemView _item;
        [SerializeField] private TextMeshProUGUI _header;

        private Action _onClose;

        public void Initialize(Item item, string header, bool isNeedLocalizeHeader = true, Action onClose = null)
        {
            _header.text = isNeedLocalizeHeader
                ? LocalizationHelper.GetLocale(header)
                : header;

            _item.Initialize(item);
            _onClose = onClose;
        }

        public void Initialize(Currency currency, string header, bool isNeedLocalizeHeader = true, Action onClose = null)
        {
            _header.text = isNeedLocalizeHeader
                ? LocalizationHelper.GetLocale(header)
                : header;

            _item.Initialize(currency);
            _onClose = onClose;
        }

        public void Initialize(Sprite itemSprite, long itemAmount, string header, bool isNeedLocalizeHeader = true, Action onClose = null)
        {
            _header.text = isNeedLocalizeHeader
                ? LocalizationHelper.GetLocale(header)
                : header;

            _item.Initialize(itemSprite, itemAmount);
            _onClose = onClose;
        }

        public override void OnClose()
        {
            base.OnClose();
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _onClose?.Invoke();
        }
    }
}