using System;
using BlackTemple.Common;
using CommonDLL.Dto;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiItemsPanelItem : MonoBehaviour
    {
        public string ItemStaticId { get; private set; }

        public int Amount { get; private set; }

        [SerializeField] private Image _icon;

        [SerializeField] private TextMeshProUGUI _amount;

        [SerializeField] private GameObject _openedView;

        [SerializeField] private GameObject _selectedView;

        private Action<MineSceneUiItemsPanelItem> _onClick;


        public void Initialize(Item dtoItem, Action<MineSceneUiItemsPanelItem> onClick)
        {
            ItemStaticId = dtoItem.Id;
            _icon.sprite = SpriteHelper.GetIcon(dtoItem.Id);
            _onClick = onClick;
            UpdateAmount(dtoItem.Amount);
        }

        public void UpdateAmount(int amount)
        {
            Amount = amount;
            _amount.text = Amount < 100
                ? Amount.ToString()
                : "99+";
        }

        public void UpdateState(bool isOpened, bool isSelected = false)
        {
            _openedView.SetActive(isOpened);
            _selectedView.SetActive(isSelected);
        }

        public void OnClick()
        {
            _onClick?.Invoke(this);
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
        }
    }
}