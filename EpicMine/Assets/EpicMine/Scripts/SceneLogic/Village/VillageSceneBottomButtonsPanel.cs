using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VillageSceneBottomButtonsPanel : MonoBehaviour {

    [SerializeField] private RectTransform _arrowIcon;
    [SerializeField] private RedDotBaseView _arrowRedDot;

    [SerializeField] private RectTransform _innerPanel;
    [SerializeField] private RectTransform _outerPanel;
    [SerializeField] private GameObject _leftGradient;
    [SerializeField] private GameObject _rightGradient;
    [SerializeField] private RectTransform _buttonsContainer;
    
    [SerializeField] private ScrollRect _scrollRect;

    private const float SlideTime = 0.3f;

    private float _maxWidth;
    private float _maxScroll;

    private bool _isOpened;
    private bool _staticState;

    public void Awake()
    {
        _maxWidth = _outerPanel.rect.width;
    }

    public void OnScroll()
    {
        if (_staticState)
        {
            _scrollRect.horizontalNormalizedPosition = 0.5f;
            return;
        }

        var maxRight = (_maxWidth - _buttonsContainer.sizeDelta.x) - 150;
        var maxLeft = 150;

        if (_buttonsContainer.anchoredPosition.x > maxLeft)
        {
            _buttonsContainer.anchoredPosition = new Vector2(maxLeft, _buttonsContainer.anchoredPosition.y);
        }
        else if (_buttonsContainer.anchoredPosition.x < maxRight)
        {
            _buttonsContainer.anchoredPosition = new Vector2(maxRight, _buttonsContainer.anchoredPosition.y);
        }
    }

    public void Toggle()
    {
        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

        _innerPanel.DOKill();

        _maxWidth = _outerPanel.rect.width < _buttonsContainer.sizeDelta.x
            ? _outerPanel.rect.width
            : _buttonsContainer.sizeDelta.x;

        _staticState = _outerPanel.rect.width + 50 > _buttonsContainer.sizeDelta.x;

        _isOpened = !_isOpened;

        _arrowIcon.DOScaleX(_isOpened ? -1 : 1, 0f);
        _arrowRedDot.gameObject.SetActive(!_isOpened);

        _leftGradient.SetActive(!_staticState);
        _rightGradient.SetActive(!_staticState);

        var width = _isOpened
            ? (_maxWidth > _buttonsContainer.sizeDelta.x ? _buttonsContainer.sizeDelta.x : _maxWidth)
            : 0;

        if (_isOpened)
            _scrollRect.viewport.transform.localScale = new Vector3(1, 1, 1);

        _innerPanel.DOSizeDelta(new Vector2(width, _innerPanel.sizeDelta.y), SlideTime).SetUpdate(true)
            .OnComplete(() =>
        {
            if(!_isOpened)
            _scrollRect.viewport.transform.localScale = new Vector3(0, 0, 0);
        });
    }

}
