using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using BlackTemple.EpicMine.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class TiersSceneController : MonoBehaviour
    {
        [SerializeField] private TiersSceneTier _tierPrefab;
        [SerializeField] private float _tiersScrollSize;

        [Header("Tiers")]
        [SerializeField] private float _scrollSpeed;
        [SerializeField] private RectTransform _tiersRectTransform;
        [SerializeField] private ScrollRect _tiersScrollRect;
        [SerializeField] private ScrollViewEventHandler _scrollHandler;
        [SerializeField] private GameObject _scrollUpButtons;
        [SerializeField] private GameObject _scrollDownButtons;
        [SerializeField] private Transform _tiersContainer;

        [Header("Mine info window")]
        [SerializeField] private TextMeshProUGUI _mineInfoTierName;
        [SerializeField] private TextMeshProUGUI _mineInfoMineNumber;
        [SerializeField] private TierSceneDropItem[] _dropItems;

        [Header("Hardcore")]
        [SerializeField] private Checkbox _hardcoreCheckbox;
        [SerializeField] private GameObject _hardcoreHintPopup;
        [SerializeField] private TextMeshProUGUI _hardcoreHintText;

        [Header("Quests")]
        [SerializeField] private ScrollRect _questScroll;
        [SerializeField] private TierSceneQuestPanelItem _questItemPrefab;

        [Header("AutoMiner")]
        [SerializeField] private GameObject _autoMinerButton;

        private int _currentTier;
        private bool _hardcoreEnabled;
        private readonly List<TiersSceneTier> _tiers = new List<TiersSceneTier>();
        private TiersSceneMine _selectedMine;
        private List<List<List<Quest>>> _minesQuests;


        public void GoToMine()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.IsHardcoreMode, _hardcoreEnabled);
            SceneManager.Instance.LoadScene(ScenesNames.Mine);
        }

        public void GoToVillage()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            SceneManager.Instance.LoadScene(ScenesNames.Village);
        }


        public void ScrollToTop()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            if (_currentTier <= 0)
                return;

            _currentTier = 0;
            FadeOutTiers();
            ScrollToCurrentTier();
        }

        public void ScrollUp()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _currentTier--;

            if (_currentTier < 0)
            {
                _currentTier = 0;
                return;
            }

            FadeOutTiers();
            ScrollToCurrentTier();
        }

        public void ScrollToBottom()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            var lastOpenedTier = _tiers.LastOrDefault(t => t.Tier.IsOpen);
            if (lastOpenedTier == null || _currentTier >= lastOpenedTier.Tier.Number)
                return;

            _currentTier = lastOpenedTier.Tier.Number;
            FadeOutTiers();
            ScrollToCurrentTier();
        }

        public void ScrollDown()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            var lastOpenedTier = _tiers.LastOrDefault(t => t.Tier.IsOpen);
            if (lastOpenedTier == null)
                return;

            _currentTier++;
            if (_currentTier > lastOpenedTier.Tier.Number)
            {
                _currentTier = lastOpenedTier.Tier.Number;
                return;
            }

            FadeOutTiers();
            ScrollToCurrentTier();
        }


        public void ShowHardcoreHintPopup()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _hardcoreHintPopup.SetActive(true);
        }

        public void CloseHardcoreHintPopup()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _hardcoreHintPopup.SetActive(false);
        }

        public void OnClickOpenAutoMiner()
        {
            if (!App.Instance.Player.AutoMiner.IsOpen)
                return;

            SceneManager.Instance.LoadScene(ScenesNames.AutoMiner);
        }

        private void Awake()
        {
            EventManager.Instance.Subscribe<TierOpenEvent>(OnTierOpen);
        }


        private void CalculateQuestHelperAsync(bool complete)
        {
            foreach (var tiersSceneTier in _tiers)
            {
                tiersSceneTier.SetQuests(_minesQuests[tiersSceneTier.Tier.Number]);
            }
        }

        private void Start()
        {
            _minesQuests = new List<List<List<Quest>>>();

            _autoMinerButton.SetActive(App.Instance.Player.AutoMiner.IsOpen);

            _scrollHandler.OnDragEnded += OnScrollEnded;
            _scrollHandler.OnDragBegin += OnScrollBegin;
            _hardcoreCheckbox.OnChange += OnHardcoreToggle;

            var locale = LocalizationHelper.GetLocale("tiers_hardcore_hint");
            _hardcoreHintText.text = string.Format(locale, App.Instance.StaticData.Configs.Dungeon.Mines.Walls.HardcoreDropItemAmountCoefficient);

            foreach (var blTier in App.Instance.Player.Dungeon.Tiers)
            {
                var tier = Instantiate(_tierPrefab, _tiersContainer, false);
                tier.Initialize(blTier, OnMineClick);
                _tiers.Add(tier);
            }

            _tiersScrollRect.verticalNormalizedPosition = 1f;


            AsyncHelper.StartAsync(() =>
            {
                _minesQuests = new List<List<List<Quest>>>();

                for (var i = 0; i < App.Instance.StaticData.Tiers.Count; i++)
                {
                    var tierMines = new List<List<Quest>>();
                    for (var j = 0; j < LocalConfigs.TierMinesCount; j++)
                    {
                        var mineQuests = QuestHelper.GetMineQuests(i, j);
                        tierMines.Add(mineQuests);
                    }

                    _minesQuests.Add(tierMines);
                }
            }, CalculateQuestHelperAsync);

            var lastTier = _tiers.LastOrDefault();
            if (lastTier != null)
                lastTier.HideLinesToNextTier();

       
            var lastOpenedTier = _tiers.LastOrDefault(t => t.Tier.IsOpen);

            if (lastOpenedTier != null)
            {
                var allMinesComplete = lastOpenedTier.Mines.All(m => m.Mine.IsComplete);
                if (allMinesComplete)
                {
                    var nextTier = _tiers.FirstOrDefault(t => !t.Tier.IsOpen);
                    if (nextTier != null)
                        nextTier.ShowOpenButton();
                }
                //s

                FadeOutTiers();

                var selectedTier = App.Instance
                    .Services
                    .RuntimeStorage
                    .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

                var selectedMine = App.Instance
                    .Services
                    .RuntimeStorage
                    .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

                if (selectedTier != null && selectedMine != null)
                {
                    _currentTier = selectedTier.Number;
                    ScrollToCurrentTier(immediately: true);

                    var tier = _tiers.FirstOrDefault(t => t.Tier.Number == selectedTier.Number);
                    if (tier == null)
                        return;

                    var mine = tier.Mines.LastOrDefault(m => m.Mine.Number == selectedMine.Number);
                    if (mine == null)
                        return;

                    mine.Click();
                }
                else
                {
                    _currentTier = lastOpenedTier.Tier.Number;
                    ScrollToCurrentTier(immediately: false);

                    var lastOpenedMine = lastOpenedTier.Mines.LastOrDefault(m => m.Mine.IsOpen);
                    if (lastOpenedMine == null)
                        return;

                    lastOpenedMine.Click();
                }
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<TierOpenEvent>(OnTierOpen);

            _hardcoreCheckbox.OnChange -= OnHardcoreToggle;
        }


        private void OnHardcoreToggle(bool isOn)
        {
            _hardcoreEnabled = isOn;
            _selectedMine.ToggleHardcore(_hardcoreEnabled);
        }

        private void SetHardcore(bool isOn)
        {
            _hardcoreEnabled = isOn;
            _hardcoreCheckbox.SetOn(isOn);
            _selectedMine.ToggleHardcore(_hardcoreEnabled);
        }

        private void OnTierOpen(TierOpenEvent eventData)
        {
            _currentTier = eventData.Tier.Number;
            FadeOutTiers();
            ScrollToCurrentTier();

            var tier = _tiers.FirstOrDefault(t => t.Tier.Number == eventData.Tier.Number);
            if (tier != null)
            {
                var mine = tier.Mines.FirstOrDefault();
                if (mine != null)
                    mine.Click();
            }
        }

        private void OnScrollEnded()
        {
            var nearestNumber = Mathf.RoundToInt(_tiersScrollRect.verticalNormalizedPosition / _tiersScrollSize);
            var tiersCount = _tiers.Count - 1;
            var newTierNumber = tiersCount - nearestNumber;

            var maxNumber = 0;
            var lastOpenedTier = _tiers.LastOrDefault(t => t.Tier.IsOpen);
            if (lastOpenedTier != null)
                maxNumber = lastOpenedTier.Tier.Number;

            _currentTier = Mathf.Clamp(newTierNumber, 0, maxNumber);
            FadeOutTiers();
            ScrollToCurrentTier();
        }

        private void OnScrollBegin()
        {
            _tiersScrollRect.DOKill();
        }

        private void FadeOutTiers()
        {
            foreach (var tier in _tiers)
                tier.FadeOut();
        }

        private void ScrollToCurrentTier(bool immediately = false)
        {
            var tier = _tiers[_currentTier].GetComponent<RectTransform>();
            ScrollTo(tier, () => { _tiers[_currentTier].FadeIn(); }, immediately);

            /*UpdateScrollButtons();*/
        }

        /*private void UpdateScrollButtons()
        {
            var firstTier = _tiers.FirstOrDefault(t => t.BlTier.IsUnlocked);
            var lastTier = _tiers.LastOrDefault(t => t.BlTier.IsUnlocked);

            if (firstTier == null || lastTier == null)
            {
                _scrollUpButtons.SetActive(true);
                _scrollDownButtons.SetActive(true);
                return;
            }

            _scrollUpButtons.SetActive(_currentTier > firstTier.BlTier.Number);
            _scrollDownButtons.SetActive(_currentTier < lastTier.BlTier.Number);
        }*/


        private void OnMineClick(TiersSceneTier tier, TiersSceneMine mine)
        {
            if (_currentTier != tier.Tier.Number)
            {
                _currentTier = tier.Tier.Number;
                FadeOutTiers();
                ScrollToCurrentTier();
            }

            if (_selectedMine != null && _selectedMine == mine)
            {
                GoToMine();
                return;
            }
            
            _selectedMine = mine;

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedTier, tier.Tier);
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedMine, mine.Mine);
            
            if (mine.Mine.Rating < 3)
            {
                _hardcoreCheckbox.SetInteractable(false);
                SetHardcore(false);
            }
            else
            {
                _hardcoreCheckbox.SetInteractable(true);
                SetHardcore(mine.Mine.IsHardcoreOn);
            }

            foreach (var t in _tiers)
                t.RemoveMineSelections();

            var staticTier = StaticHelper.GetTier(tier.Tier.Number);
            var staticMine = StaticHelper.GetMine(tier.Tier.Number, mine.Mine.Number);
            var mineCommonSettings = StaticHelper.GetMineCommonSettings(tier.Tier.Number, mine.Mine.Number);

            var tierName = LocalizationHelper.GetLocale("tier_" + tier.Tier.Number);
            var mineLocale = LocalizationHelper.GetLocale("mine");

            _mineInfoTierName.text = $"{tierName}";
            _mineInfoMineNumber.text = $"{mine.Mine.Number + 1} {mineLocale}";

            var firstItemChance = staticMine.Item1Percent ?? mineCommonSettings.Item1Percent;
            var secondItemChance = staticMine.Item2Percent ?? mineCommonSettings.Item2Percent;
            var thirdItemChance = staticMine.Item3Percent ?? mineCommonSettings.Item3Percent;

            // exclude chest percent
            var itemsChanceSum = firstItemChance + secondItemChance + thirdItemChance;
            firstItemChance = Mathf.RoundToInt(firstItemChance / itemsChanceSum * 100);
            secondItemChance = Mathf.RoundToInt(secondItemChance / itemsChanceSum * 100);
            thirdItemChance = Mathf.RoundToInt(thirdItemChance / itemsChanceSum * 100);

            _dropItems[0].Initialize(
                staticTier.WallItem1Id, 
                firstItemChance,
                tier.Tier.IsDropItemUnblocked(staticTier.WallItem1Id), 
                mine.Mine.IsComplete);

            _dropItems[1].Initialize(
                staticTier.WallItem2Id, 
                secondItemChance, 
                tier.Tier.IsDropItemUnblocked(staticTier.WallItem2Id), 
                mine.Mine.IsComplete);

            _dropItems[2].Initialize(
                staticTier.WallItem3Id, 
                thirdItemChance, 
                tier.Tier.IsDropItemUnblocked(staticTier.WallItem3Id), 
                mine.Mine.IsComplete);


            if (_minesQuests.Count > 0)
            {
                var hasQuests = false;

                _questScroll.content.ClearChildObjects();

                var alreadyExist = new List<string>();

                if (mine.Mine.Number < _minesQuests[tier.Tier.Number].Count)
                {
                    var quests = _minesQuests[tier.Tier.Number][mine.Mine.Number];

                    foreach (var quest in quests)
                    {
                        if (alreadyExist.Contains(quest.StaticQuest.Id))
                            continue;

                        Instantiate(_questItemPrefab, _questScroll.content)
                            .Initialize(quest);

                        alreadyExist.Add(quest.StaticQuest.Id);
                    }

                    hasQuests = quests.Count > 0;
                }

                _questScroll.gameObject.SetActive(hasQuests);
            }
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
                /*.SetEase(Ease.Linear)*/
                .OnComplete(onComplete);
        }
    }
}