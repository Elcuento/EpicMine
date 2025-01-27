using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowInventoryItem : MonoBehaviour, IPointerClickHandler
    {
        public string StaticItemId { get; private set; }

        public int Amount { get; private set; }

        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private GameObject _redDot;

        private Action<WindowInventoryItem> _onClick;


        public void Initialize(string staticItemId, int amount, Action<WindowInventoryItem> onClick = null)
        {
            StaticItemId = staticItemId;
            Amount = amount;
            _onClick = onClick;

            _amountText.text = Amount.ToString();
            _icon.sprite = SpriteHelper.GetIcon(StaticItemId);
        }

        public void UpdateAmount(int newAmount)
        {
            Amount = newAmount;
            _amountText.text = Amount.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke(this);
        }

        public void ShowRedDot()
        {
            _redDot.SetActive(true);
        }

        public void HideRedDot()
        {
            _redDot.SetActive(false);
        }
    }
}