using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class TierSceneDropItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Image _unknownIcon;
        [SerializeField] private TextMeshProUGUI _chanceText;

        [SerializeField] private float _unknownIconTopPosition;
        [SerializeField] private float _unknownIconCenterPosition;

        private string _itemId;
        private bool _isItemUnlocked;


        public void Initialize(string itemStaticId, float chance, bool itemUnlocked, bool mineComplete)
        {
            _itemId = itemStaticId;
            _isItemUnlocked = itemUnlocked;
            
            if (_isItemUnlocked)
            {
                _itemIcon.sprite = SpriteHelper.GetIcon(_itemId);
                _itemIcon.gameObject.SetActive(true);
                _unknownIcon.gameObject.SetActive(false);

                _chanceText.text = mineComplete ? $"{chance}%" : "???";
            }
            else
            {
                _itemIcon.gameObject.SetActive(false);
                _unknownIcon.gameObject.SetActive(true);
                _unknownIcon.rectTransform.DOAnchorPosY(mineComplete ? _unknownIconTopPosition : _unknownIconCenterPosition, 0);

                _chanceText.text = mineComplete ? $"{chance}%" : "";
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isItemUnlocked)
                return;

            var window = WindowManager.Instance.Show<WindowItemPopup>(withSound: false);
            window.Initialize(_itemId, transform.position);
        }
    }
}