using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class ItemView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amount;
        [SerializeField] private bool _showPopupInfo;

        private string _itemId;
        

        public void Initialize(Item dtoItem)
        {
            _itemId = dtoItem.Id;
            _icon.sprite = SpriteHelper.GetIcon(dtoItem.Id);
            _amount.text = dtoItem.Amount.ToString();
        }

        public void Initialize(FeaturesType feature)
        {
            _itemId = feature.ToString();
            _icon.sprite = SpriteHelper.GetFeaturePicture(feature);
            _amount.text = "";
            _showPopupInfo = false;
        }

        public void Initialize(Item dtoItem, Color textColor)
        {
            _itemId = dtoItem.Id;
            _icon.sprite = SpriteHelper.GetIcon(dtoItem.Id);
            _amount.text = dtoItem.Amount.ToString();
            _amount.color = textColor;
        }

        public void Initialize(string itemStaticId, string amount)
        {
            _itemId = itemStaticId;
            _icon.sprite = SpriteHelper.GetIcon(itemStaticId);
            _amount.text = amount;
        }
        public void Initialize(string itemStaticId, long amount)
        {
            _itemId = itemStaticId;
            _icon.sprite = SpriteHelper.GetIcon(itemStaticId);
            _amount.text = amount.ToString();
        }

        public void Initialize(string itemStaticId, int amount)
        {
            _itemId = itemStaticId;
            _icon.sprite = SpriteHelper.GetIcon(itemStaticId);
            _amount.text = amount.ToString();
        }

        public void Initialize(string itemStaticId, int amount, string hexCol)
        {
            _itemId = itemStaticId;
            _icon.sprite = SpriteHelper.GetIcon(itemStaticId);
            _amount.text = amount.ToString();
            Color color;
            ColorUtility.TryParseHtmlString(hexCol, out color);
            _amount.color = color;
        }

        public void Initialize(Dto.Currency dtoCurrency)
        {
            _icon.sprite = SpriteHelper.GetCurrencyIcon(dtoCurrency.Type);
            _amount.text = dtoCurrency.Amount.ToString();
        }

        public void Initialize(Dto.Currency dtoCurrency, Color textColor)
        {
            _icon.sprite = SpriteHelper.GetCurrencyIcon(dtoCurrency.Type);
            _amount.text = dtoCurrency.Amount.ToString();
            _amount.color = textColor;
        }
        public void Initialize(Sprite sprite, long amount)
        {
            _icon.sprite = sprite;
            _amount.text = amount.ToString();
        }
        public void Initialize(Sprite sprite, int amount)
        {
            _icon.sprite = sprite;
            _amount.text = amount.ToString();
        }

        public void Initialize(Sprite sprite, string text)
        {
            _icon.sprite = sprite;
            _amount.text = text;
        }

        public void SetColor(Color color)
        {
            _amount.color = color;
        }

        public void EnableRaycast(bool enable)
        {
            var raycastTarget = GetComponent<Image>();
            raycastTarget.raycastTarget = enable;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_showPopupInfo)
                return;

            if (string.IsNullOrEmpty(_itemId))
                return;

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            var window = WindowManager.Instance.Show<WindowItemPopup>(withSound: false);
            window.Initialize(_itemId, transform.position);
        }
    }
}