using BlackTemple.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// ReSharper disable IdentifierTypo

namespace BlackTemple.EpicMine
{
    public class Checkbox : MonoBehaviour, IPointerDownHandler
    {
        public event EventHandler<bool> OnChange;

        public bool IsOn => _isOn;

        public bool IsInteractable => _isInteractable;

        [SerializeField] private bool _isOn;
        [SerializeField] private bool _isInteractable;
        [SerializeField] private Sprite _handleDisabledSprite;
        [SerializeField] private Sprite _handleEnabledSprite;
        [SerializeField] private Sprite _backgroundDisabledSprite;
        [SerializeField] private Sprite _backgroundEnabledSprite;

        [Space]
        [SerializeField] private Image _handleImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private float _handleDisabledXPosition;
        [SerializeField] private float _handleEnabledXPosition;

        [Space]
        [SerializeField] private Color _unInterractableColor = new Color(1,1,1, 0.3f);
        [SerializeField] private Color _interractableColor = Color.white;


        public void SetOn(bool value, bool immediately = false)
        {
            _isOn = value;
            UpdateView(immediately);
            OnChange?.Invoke(value);
        }

        public void SetInteractable(bool value)
        {
            _isInteractable = value;
            _handleImage.color = _isInteractable ? _interractableColor : _unInterractableColor;
            _backgroundImage.color = _isInteractable ? _interractableColor : _unInterractableColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable)
                return;

            _isOn = !_isOn;
            UpdateView();
            OnChange?.Invoke(IsOn);
        }


        private void UpdateView(bool immediately = false)
        {
            _handleImage.sprite = _isOn ? _handleEnabledSprite : _handleDisabledSprite;
            _backgroundImage.sprite = _isOn ? _backgroundEnabledSprite : _backgroundDisabledSprite;

            var position = _isOn ? _handleEnabledXPosition : _handleDisabledXPosition;
            var duration = immediately ? 0f : 0.3f;
            _handleImage.rectTransform.DOAnchorPosX(position, duration).SetUpdate(true);
        }
    }
}