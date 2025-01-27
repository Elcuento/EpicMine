using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using BlackTemple.EpicMine.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoMinerSceneController : MonoBehaviour
{
    [SerializeField] private AutoMinerSceneTier _tierPrefab;
    [SerializeField] private float _tiersScrollSize;

    [Header("Tiers")]
    [SerializeField] private float _scrollSpeed;
    [SerializeField] private RectTransform _tiersRectTransform;
    [SerializeField] private ScrollRect _tiersScrollRect;
    [SerializeField] private ScrollViewEventHandler _scrollHandler;
    [SerializeField] private GameObject _scrollUpButtons;
    [SerializeField] private GameObject _scrollDownButtons;
    [SerializeField] private Transform _tiersContainer;

    [Header("Info Panels")]
    [SerializeField] private AutoMinerSceneCollectInfoPanel _collectInfo;
    [SerializeField] private AutoMinerSceneStatusInfoPanel _statusInfo;

    private Tier _currentTier;
    private readonly List<AutoMinerSceneTier> _tiers = new List<AutoMinerSceneTier>();
    private AutoMinerSceneTier _selectedTier;


    private void Start()
    {

        _scrollHandler.OnDragEnded += OnScrollEnded;
        _scrollHandler.OnDragBegin += OnScrollBegin;

        EventManager.Instance.Subscribe<AutoMinerChangeEvent>(OnAutoMinerChanged);

        foreach (var blTier in App.Instance.Player.Dungeon.Tiers)
        {
            var tier = Instantiate(_tierPrefab, _tiersContainer, false);
            tier.Initialize(blTier, SelectTier);
            _tiers.Add(tier);
        }

        if (App.Instance.Player.AutoMiner.Started)
        {
            _tiers[App.Instance.Player.AutoMiner.Tier].SetSelection();
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_tiersContainer.GetComponent<RectTransform>());

        SelectTier(App.Instance.Player.AutoMiner.Tier);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<AutoMinerChangeEvent>(OnAutoMinerChanged);
    }

    public void OnClickClose()
    {
        SceneManager.Instance.LoadScene(ScenesNames.Village);
    }

    private void SelectTier(int tier, bool immediately = false)
    {
        _currentTier = App.Instance.Player.Dungeon.Tiers.Find(x => x.Number == tier);

        _collectInfo.Initialize(_currentTier);
        _statusInfo.Initialize(_currentTier);

        FadeOutTiers();
        ScrollToCurrentTier(immediately);
    }


    public void ScrollToTop()
    {
        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

        if (_currentTier.Number <= 0)
            return;


        SelectTier(0);
    }

    public void ScrollUp()
    {
        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

        if (_currentTier.Number <= 0)
            return;

        SelectTier(_currentTier.Number - 1);
    }

    public void ScrollToBottom()
    {
        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
        var lastOpenedTier = _tiers.LastOrDefault(x => x.Tier.IsOpen);
        if (lastOpenedTier == null || _currentTier.Number >= lastOpenedTier.Tier.Number)
            return;

        SelectTier(lastOpenedTier.Tier.Number);
    }

    public void ScrollDown()
    {
        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

        var lastOpenedTier = _tiers.LastOrDefault();
        if (lastOpenedTier == null)
            return;

        if (_currentTier != lastOpenedTier.Tier)
        {
            SelectTier(_currentTier.Number + 1);
        }
    }


    private void OnAutoMinerChanged(AutoMinerChangeEvent eventData)
    {
        foreach (var autoMinerSceneTier in _tiers)
        {
            if(autoMinerSceneTier.Tier.Number != _currentTier.Number)
            autoMinerSceneTier.FadeOut();
        }

        _tiers[App.Instance.Player.AutoMiner.Tier].FadeIn();
        _tiers[App.Instance.Player.AutoMiner.Tier].SetSelection();
    }


    private void FadeOutTiers()
    {
        foreach (var tier in _tiers)
        {
            if (tier.Tier.Number == App.Instance.Player.AutoMiner.Tier && App.Instance.Player.AutoMiner.Started)
            {
                tier.SetSelection();
            }
            else
            {
                tier.RemoveSelection();
                tier.FadeOut();
            }
            
        }

    }

    private void OnScrollBegin()
    {
        _tiersScrollRect.DOKill();
    }
    private void OnScrollEnded()
    {
        var nearestNumber = Mathf.RoundToInt(_tiersScrollRect.verticalNormalizedPosition / _tiersScrollSize);
        var tiersCount = _tiers.Count - 1;
        var newTierNumber = tiersCount - nearestNumber;

        var maxNumber = 0;
        var lastOpenedTier = _tiers.LastOrDefault();
        if (lastOpenedTier != null)
            maxNumber = lastOpenedTier.Tier.Number;

        var tierNumber = Mathf.Clamp(newTierNumber, 0, maxNumber);


        SelectTier(tierNumber);
    }

    private void ScrollToCurrentTier(bool immediately = false)
    {
        var tier = _tiers[_currentTier.Number].GetComponent<RectTransform>();
        ScrollTo(tier, ()=>
        {
            _tiers[_currentTier.Number].FadeIn();
        }, immediately);
    }

    private void ScrollTo(RectTransform target, TweenCallback onComplete = null, bool immediately = false)
    {
        _tiersScrollRect.content.DOKill();

        var screenHalfHeight = _tiersRectTransform.sizeDelta.y / 2f;
        var position = Mathf.Abs(target.anchoredPosition.y) - screenHalfHeight;

        /*var positionElasticOffset = _tiersRectTransform.sizeDelta.y / 10f;
        var clampedPosition = Mathf.Clamp(position, -positionElasticOffset, position + positionElasticOffset);*/

        var distance = Mathf.Abs(_tiersScrollRect.content.anchoredPosition.y - position);
        var duration = Mathf.Clamp01(distance / _scrollSpeed);

        _tiersScrollRect.content
            .DOAnchorPosY(position, immediately ? 0f : duration)
            .SetDelay(0.1f)
            .OnComplete(onComplete);
    }

    private void OnMineScrolled()
    {

    }

}
