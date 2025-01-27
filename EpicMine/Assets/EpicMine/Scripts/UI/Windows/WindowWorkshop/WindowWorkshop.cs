using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using WorkshopSlot = BlackTemple.EpicMine.Core.WorkshopSlot;

namespace BlackTemple.EpicMine
{
    public class WindowWorkshop : WindowBase
    {
        [SerializeField] private WindowWorkshopSlot _slotPrefab;
        [SerializeField] private Transform _slotsContainer;

        [Space]
        [SerializeField] private RedDotBaseView _recipesRedDot;

        [Space]
        [SerializeField] private Toggle _ores;
        [SerializeField] private Toggle _shards;

        [Space]
        [SerializeField] private TextMeshProUGUI _oresText;
        [SerializeField] private TextMeshProUGUI _shardsText;

        [Space]
        [SerializeField] private Color _activeFilterTabColor;
        [SerializeField] private Color _inactiveFilterTabColor;


        private WorkshopSlot _nextClosedSlot;
        private float _soundTime;


        public void Filter()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            SetFilterTabsColors();
        }

        public void OnClickTab()
        {

            if (_ores.isOn)
                InitializeOresSlots();
            else
                InitializeShardsSlots();

            Filter();
        }

        private void SetFilterTabsColors()
        {
            _oresText.color = _ores.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
            _shardsText.color = _shards.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
        }

        public void OnClickBoost()
        {
            WindowManager.Instance.Show<WindowShop>()
                .OpenBoost();
        }

        public void ShowRecipes()
        {
            if(_ores.isOn) WindowManager.Instance.Show<WindowRecipes>(withCurrencies: true).Initialize();
            else WindowManager.Instance.Show<WindowShardRecipes>(withCurrencies: false).Initialize();
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            OnClickTab();

            EventManager.Instance.Unsubscribe<WorkShopReload>(OnReload);
            EventManager.Instance.Subscribe<WorkShopReload>(OnReload);

            if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier))
            {
                App.Instance.Controllers.RedDotsController.OnRecipesChange += OnRedDotRecipesChange;
                OnRedDotRecipesChange(App.Instance.Controllers.RedDotsController.NewRecipes);
            }
        }

        private void InitializeShardsSlots()
        {
            Clear();

            var unlockedSlots = App.Instance.Player.Workshop.SlotsShard.Where(s => s.IsUnlocked).ToList();
            unlockedSlots.Sort(SortByStartedAndCompleted);

            foreach (var workshopSlot in unlockedSlots)
            {
                var slot = Instantiate(_slotPrefab, _slotsContainer, false);
                slot.Initialize(workshopSlot);
            }

            ShowNextShardClosedSlot(WorkShopSlotType.Shard);
            EventManager.Instance.Subscribe<WorkshopSlotUnlockEvent>(OnSlotUnlock);
        }

        private void InitializeOresSlots()
        {
            Clear();

            var unlockedSlots = App.Instance.Player.Workshop.Slots.Where(s => s.IsUnlocked).ToList();
            unlockedSlots.Sort(SortByStartedAndCompleted);

            foreach (var workshopSlot in unlockedSlots)
            {
                var slot = Instantiate(_slotPrefab, _slotsContainer, false);
                slot.Initialize(workshopSlot);
            }

            ShowNextShardClosedSlot();
            EventManager.Instance.Subscribe<WorkshopSlotUnlockEvent>(OnSlotUnlock);
        }

        private int SortByStartedAndCompleted(WorkshopSlot x, WorkshopSlot y)
        {
            if (x.StaticRecipe != null)
            {
                if (y.StaticRecipe == null)
                    return -1;

                if (x.CompleteAmount >= x.NecessaryAmount)
                    return y.CompleteAmount >= y.NecessaryAmount ? 0 : -1;

                return y.CompleteAmount >= y.NecessaryAmount ? 1 : 0;
            }

            return y.StaticRecipe != null ? 1 : 0;
        }

        public override void OnClose()
        {
            base.OnClose();
            Clear();

            if(EventManager.Instance != null)
            EventManager.Instance.Unsubscribe<WorkShopReload>(OnReload);
        }


        private void Update()
        {
            if (_soundTime > 0)
            {
                _soundTime -= Time.deltaTime;
                return;
            }

            _soundTime = Random.Range(3f, 8f);
            var randomSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.BlacksmithSounds.Length);
            var randomSound = App.Instance.ReferencesTables.Sounds.BlacksmithSounds[randomSoundIndex];
            AudioManager.Instance.PlaySound(randomSound);
        }

        private IEnumerator OnApplicationPause(bool isPaused)
        {
            yield return new WaitForSeconds(0.3f);

            if (!isPaused)
            {
                if (_ores.isOn)
                    InitializeOresSlots();
                else
                    InitializeShardsSlots();
            }
        }

        private void OnRedDotRecipesChange(List<string> unlockedRecipes)
        {
            _recipesRedDot.Show(unlockedRecipes.Count);
        }

        private void Clear()
        {
            _slotsContainer.ClearChildObjects();

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<WorkshopSlotUnlockEvent>(OnSlotUnlock);
  
            }

            if (App.Instance != null)
            {
                App.Instance.Controllers.RedDotsController.OnRecipesChange -= OnRedDotRecipesChange;
            }
        }

        private void OnReload(WorkShopReload data)
        {
            WindowManager.Instance.Close<WindowWorkshop>();
            WindowManager.Instance.Show<WindowWorkshop>();
        }

        private void ShowNextShardClosedSlot(WorkShopSlotType type = WorkShopSlotType.Ore)
        {
          
            if(type == WorkShopSlotType.Ore) _nextClosedSlot = App.Instance.Player.Workshop.Slots.FirstOrDefault(s => !s.IsUnlocked);
            else _nextClosedSlot = App.Instance.Player.Workshop.SlotsShard.FirstOrDefault(s => !s.IsUnlocked);

            if (_nextClosedSlot == null)
                return;

            var closedSlot = Instantiate(_slotPrefab, _slotsContainer, false);
            closedSlot.Initialize(_nextClosedSlot);

        }

        private void OnSlotUnlock(WorkshopSlotUnlockEvent eventData)
        {
            if (eventData.WorkshopSlot == _nextClosedSlot)
                ShowNextShardClosedSlot(eventData.WorkshopSlot.SlotType);
        }
    }
}