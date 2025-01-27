using System;
using System.Collections;
using System.Linq;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowItemPopup : WindowBase
    {
        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private RectTransform _windowRectTransform;
        [SerializeField] private GameObject _leftArrow;
        [SerializeField] private GameObject _rightArrow;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _additionalDescription;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;

        private float _offset = 75f;
        

        public void Initialize(string itemId, Vector3 globalPosition)
        {
            Clear();

            var viewportPosition = Canvas.worldCamera.WorldToViewportPoint(globalPosition);
            var anchoredPosition = new Vector2(
                viewportPosition.x * _rootRectTransform.sizeDelta.x,
                viewportPosition.y * _rootRectTransform.sizeDelta.y);

            if (anchoredPosition.x < _rootRectTransform.sizeDelta.x / 2f)
            {
                _windowRectTransform.pivot = new Vector2(0, _windowRectTransform.pivot.y);
                _leftArrow.SetActive(true);
                anchoredPosition.x += _offset;
            }
            else
            {
                _windowRectTransform.pivot = new Vector2(1, _windowRectTransform.pivot.y);
                _rightArrow.SetActive(true);
                anchoredPosition.x -= _offset;
            }

            _windowRectTransform.anchoredPosition = anchoredPosition;
            _windowRectTransform.gameObject.SetActive(true);
            
            _icon.sprite = SpriteHelper.GetIcon(itemId);
            _title.text = LocalizationHelper.GetLocale(itemId);

            StartCoroutine(UpdateCanvas());

            var resource = App.Instance.StaticData.Resources.FirstOrDefault(r => r.Id == itemId);
            if (resource != null)
            {
                _priceText.text = resource.Price.ToString();
                switch (resource.Type)
                {
                    case ResourceType.Ore:
                        if (resource.Filter == 1)
                        {
                            var tier = App.Instance.StaticData.TierBosses.FindIndex(x => x.DropItemId == itemId) + 1;
                            _description.text = $"{LocalizationHelper.GetLocale("window_item_popup_resource")} ({tier} {LocalizationHelper.GetLocale("tier")})";
                        }
                        else
                        {
                            _description.text = $"{LocalizationHelper.GetLocale("window_item_popup_resource")}";
                        }

                        break;
                    case ResourceType.Ingot:
                        _description.text = LocalizationHelper.GetLocale("window_item_popup_item");
                        break;
                    case ResourceType.Shard:
                        _description.text = LocalizationHelper.GetLocale("window_item_popup_shard");
                        break;
                    default:
                        App.Instance.Services.LogService.Log(resource.Type +" not implement ");
                        break;
                }
                return;
            }

            var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(h => h.Id == itemId);
            if (hilt != null)
            {
                _priceText.text = hilt.Price.ToString();
                _description.text = LocalizationHelper.GetLocale("window_item_popup_hilt");
            }
        }


        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
                WindowManager.Instance.Close(this, withSound: false);
        }


        public override void OnClose()
        {
            base.OnClose();
            Clear();
        }
        

        private void Clear()
        {
            StopAllCoroutines();

            _windowRectTransform.gameObject.SetActive(false);
            _leftArrow.SetActive(false);
            _rightArrow.SetActive(false);

            _icon.sprite = null;

            _title.text = string.Empty;
            _description.text = string.Empty;
            _additionalDescription.text = string.Empty;
            _priceText.text = string.Empty;
        }

        private IEnumerator UpdateCanvas()
        {
            _verticalLayoutGroup.enabled = false;
            yield return new WaitForEndOfFrame();
            _verticalLayoutGroup.enabled = true;
            _windowRectTransform.gameObject.SetActive(true);
        }
    }
}