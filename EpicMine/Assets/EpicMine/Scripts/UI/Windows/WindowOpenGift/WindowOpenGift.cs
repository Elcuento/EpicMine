using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using DragonBones;
using TMPro;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace BlackTemple.EpicMine
{
    public class WindowOpenGift : WindowBase
    {
        [SerializeField] private GameObject _closedPanel;
        [SerializeField] private CanvasGroup _openedPanel;

        [Space]
        [SerializeField] private GameObject _lights;

        [Space]
        [SerializeField] private GameObject _simpleHeader;
        [SerializeField] private GameObject _royalHeader;

        [Space]
        [SerializeField] private UnityArmatureComponent _simpleGift;
        [SerializeField] private UnityArmatureComponent _royalGift;

        [Space]
        [SerializeField] private TextMeshProUGUI _closedPanelHeader;

        [Space]
        [SerializeField] private ItemView _dropItemPrefab;
        [SerializeField] private Transform _dropItemsContainer;

        private bool _isOpened;
        private int _giftNumber;
        private bool _isRoyal;

        public void Initialize(int giftNumber)
        {
            Clear();

            _giftNumber = giftNumber;

            if (_giftNumber >= App.Instance.StaticData.Configs.Gifts.DailyCount - 1)
            {
                _isRoyal = true;
                _closedPanelHeader.text = LocalizationHelper.GetLocale("royal_gift");
                _royalHeader.gameObject.SetActive(true);
                _royalGift.gameObject.SetActive(true);
                _royalGift.animation?.GotoAndStopByFrame("open");
            }
            else
            {
                _isRoyal = false;
                _closedPanelHeader.text = LocalizationHelper.GetLocale("simple_gift");
                _simpleHeader.gameObject.SetActive(true);
                _simpleGift.gameObject.SetActive(true);
                _simpleGift.animation?.GotoAndStopByFrame("open");
            }
        }

        public void Open()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            if (_isOpened)
            {
                Close();
                return;
            }

            App.Instance.Player.Gifts.Open((success, pack) =>
            {
                if (success)
                {
                    _lights.SetActive(true);

                    App.Instance.Player.Artefacts.Add(pack.Artefacts);
                    App.Instance.Player.Inventory.Add(pack, IncomeSourceType.FromGift);

                    foreach (var item in pack.Items)
                    {
                        var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                        itemView.Initialize(item);
                    }

                    if (pack.Artefacts > 0)
                    {
                        var artefactsView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                        var artefactsSprite = App.Instance.ReferencesTables.Sprites.ArtefactIcon;
                        artefactsView.Initialize(artefactsSprite, pack.Artefacts);
                    }

                    AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.OpenGift);
                    _isOpened = true;
                    _closedPanel.SetActive(false);
                    _openedPanel.gameObject.SetActive(true);
                    _openedPanel.DOFade(1, 0.1f)
                        .SetDelay(0.5f)
                        .SetUpdate(true);

                    if (_isRoyal) _royalGift.animation.Play("open",1);
                    else _simpleGift.animation.Play("open", 1);

                    var parameters = new Dictionary<string, int> { { "number", App.Instance.Player.Gifts.OpenedCount } };
                    var customEventParameters = new CustomEventParameters { Int = parameters };
                    App.Instance.Services.AnalyticsService.CustomEvent("open_gift", customEventParameters);
                }
            });
        }


        public override void OnClose()
        {
            base.OnClose();

            if (_giftNumber >= App.Instance.StaticData.Configs.Gifts.DailyCount - 1)
            {
                WindowManager
                    .Instance
                    .Show<WindowInformation>()
                    .Initialize("window_gift_info_header", "window_gift_info_description", "window_gift_info_button");
            }

            Clear();
        }


        private void Clear()
        {
            _isOpened = false;
            _giftNumber = 0;

            _lights.gameObject.SetActive(false);

            _closedPanel.SetActive(true);
            _openedPanel.gameObject.SetActive(false);
            _openedPanel.DOKill();
            _openedPanel.alpha = 0;

            _royalHeader.SetActive(false);
            _simpleHeader.SetActive(false);

            _royalGift.gameObject.SetActive(false);
            _simpleGift.gameObject.SetActive(false);

            _dropItemsContainer.ClearChildObjects();
        }
    }
}