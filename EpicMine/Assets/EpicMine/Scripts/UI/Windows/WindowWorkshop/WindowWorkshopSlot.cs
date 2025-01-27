using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowWorkshopSlot : MonoBehaviour
    {
        public Core.WorkshopSlot WorkshopSlot { get; private set; }

        [SerializeField] private ItemView _ingredientPrefab;

        [Space]
        [SerializeField] private GameObject _lockedPanel;
        [SerializeField] private GameObject _emptyPanel;
        [SerializeField] private GameObject _filledPanel;

        [Space]
        [SerializeField] private TextMeshProUGUI _unlockPriceText;
        [SerializeField] private Image _unlockPriceIcon;

        [Space]
        [SerializeField] private TextMeshProUGUI _itemNameText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _itemAmounts;
        [SerializeField] private RectTransform _underLineTransform;
        [SerializeField] private RectTransform _ingredientsContainer;
        [SerializeField] private TextMeshProUGUI _operationTitle;
        [SerializeField] private Slider _timeSlider;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _allTimeText;
        [SerializeField] private TextMeshProUGUI _sellPriceText;
        [SerializeField] private Image _sellPriceIcon;

        [Space]
        [SerializeField] private Color _oreColor;
        [SerializeField] private Color _shardColor;

        [SerializeField] private Image _filledBackground;
        [SerializeField] private Image _filledIconBackground;
        [SerializeField] private Image _filledFillBackground;

        [SerializeField] private Image _emptyBackground;
        [SerializeField] private Image _emptyIconBackground;

        [SerializeField] private Image _lockedBackground;
        [SerializeField] private Image _lockedIconBackground;

        [Space]
        [SerializeField] private TextMeshProUGUI _collectButtonTitle;
        [SerializeField] private Image _collectButtonImage;
        [SerializeField] private Image _cancelButtonImage;
        [SerializeField] private Image _forceCompleteButtonImage;
        public TextMeshProUGUI ForceCompleteButtonCostText;


        public void Initialize(Core.WorkshopSlot workshopSlot)
        {
            Unsubscribe();

            WorkshopSlot = workshopSlot;
            UpdateState();

            Subscribe();

            SetColors();
        }

        public void SetColors()
        {
            var color = WorkshopSlot.SlotType == WorkShopSlotType.Ore ? _oreColor : _shardColor;

            _filledBackground.color = color;
            _filledIconBackground.color = color;
            _filledFillBackground.color = color;

            _lockedBackground.color = color;
            _lockedIconBackground.color = color;

            _emptyBackground.color = color;
            _emptyIconBackground.color = color;
        }

        public void Unlock()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            WorkshopSlot.Unlock();
        }

        public void ShowWindowRecipes()
        {
            if(WorkshopSlot.SlotType == WorkShopSlotType.Ore)
            WindowManager.Instance.Show<WindowRecipes>(withCurrencies: true).Initialize(OnChooseRecipeInWindowRecipes);
            else WindowManager.Instance.Show<WindowShardRecipes>().Initialize(OnChooseRecipeInWindowRecipes);
        }

        public void CollectCompleted()
        {
            if (WorkshopSlot.CompleteAmount <= 0)
                return;

            WindowManager.Instance.Show<WindowCollectWorkshopSlot>().Initialize(WorkshopSlot);
        }

        public void Stop()
        {
            if (WorkshopSlot.CompleteAmount == WorkshopSlot.NecessaryAmount)
                return;

            WindowManager.Instance.Show<WindowStopWorkshopSlot>().Initialize(WorkshopSlot);
        }

        public void ForceComplete()
        {
            if (WorkshopSlot.CompleteAmount >= WorkshopSlot.NecessaryAmount)
                return;

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            var cost = new Dto.Currency(CurrencyType.Crystals, WorkshopSlot.ForceCompletePrice);
            if (App.Instance.Player.Wallet.Has(cost) || cost.Amount <= 0)
            {
                if (cost.Amount > 0)
                {
                    WindowManager
                        .Instance
                        .Show<WindowCurrencySpendConfirm>()
                        .Initialize(
                            cost,
                            () => { WorkshopSlot.ForceComplete(); },
                            "window_currency_spend_confirm_description_workshop_slot",
                            "window_currency_spend_confirm_ok_workshop_slot");
                }
                else
                    WorkshopSlot.ForceComplete();
            }
            else
            {
                WindowManager.Instance.Show<WindowShop>()
                    .OpenCrystals();
            }
        }


        private void UpdateState()
        {
            Clear();

            if (WorkshopSlot.IsUnlocked)
            {
                if (WorkshopSlot.StaticRecipe == null)
                    _emptyPanel.SetActive(true);
                else
                {
                    _filledPanel.SetActive(true);

                    UpdateItemInfo();
                    UpdateIngredients();
                    UpdateOperationInfo();
                    UpdateItemAmounts();

                    // complete
                    if (WorkshopSlot.CompleteAmount >= WorkshopSlot.NecessaryAmount)
                    {
                        _timeText.text = LocalizationHelper.GetLocale("window_workshop_slot_complete");
                        _cancelButtonImage.sprite = App.Instance.ReferencesTables.Sprites.ButtonGrey;
                        _forceCompleteButtonImage.sprite = App.Instance.ReferencesTables.Sprites.ButtonGrey;
                    }

                    // melting
                    else
                    {
                        UpdateTimers();
                        VisualizeTimeProgressBar();
                    }
                }
            }
            else
            {
                _unlockPriceText.text = WorkshopSlot.StaticSlot.PriceAmount.ToString();
                _unlockPriceIcon.sprite = SpriteHelper.GetCurrencyIcon(WorkshopSlot.StaticSlot.PriceCurrencyType);
                _lockedPanel.SetActive(true);
            }
        }


        private void Clear()
        {
            _timeSlider.DOKill();
            _timeSlider.value = 0;

            _lockedPanel.SetActive(false);
            _emptyPanel.SetActive(false);
            _filledPanel.SetActive(false);

            _cancelButtonImage.sprite = App.Instance.ReferencesTables.Sprites.ButtonRed;
            _forceCompleteButtonImage.sprite = App.Instance.ReferencesTables.Sprites.ButtonGreen;

            _operationTitle.text = "";
            _timeText.text = "";
            _allTimeText.text = "0";
            ForceCompleteButtonCostText.text = "0";

            _unlockPriceText.text = "";
            _unlockPriceIcon.sprite = null;
        }


        private void OnChooseRecipeInWindowRecipes(Core.Recipe recipe, int amount)
        {
            WorkshopSlot.Start(recipe, amount);
        }


        private void UpdateOperationInfo()
        {
            if (_ingredientsContainer.childCount > 2)
                _operationTitle.text = "";
            else _operationTitle.text = WorkshopSlot.StaticRecipe == null ? "" : LocalizationHelper.GetLocale(WorkshopSlot.StaticRecipe.Type.ToString());

            LayoutRebuilder.ForceRebuildLayoutImmediate(_operationTitle.rectTransform);
        }

        private void UpdateItemInfo()
        {
            _itemNameText.text = LocalizationHelper.GetLocale(WorkshopSlot.StaticRecipe.Id);
            _itemIcon.sprite = SpriteHelper.GetIcon(WorkshopSlot.StaticRecipe.Id);

            var resource = App.Instance.StaticData.Resources.FirstOrDefault(r => r.Id == WorkshopSlot.StaticRecipe.Id);
            if (resource != null)
            {
                _sellPriceText.text =  resource.Price == 0 ? "" : resource.Price.ToString();
                _sellPriceIcon.enabled = resource.Price != 0;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_underLineTransform);
        }

        private void UpdateItemAmounts()
        {
            _collectButtonImage.sprite = WorkshopSlot.CompleteAmount > 0
                ? App.Instance.ReferencesTables.Sprites.ButtonGrown
                : App.Instance.ReferencesTables.Sprites.ButtonGrey;

            _itemAmounts.text = $"{WorkshopSlot.CompleteAmount * WorkshopSlot.StaticRecipe.Amount}/{WorkshopSlot.NecessaryAmount * WorkshopSlot.StaticRecipe.Amount}";

            var collectLocale = LocalizationHelper.GetLocale("window_workshop_slot_collect_button");
            _collectButtonTitle.text = $"{collectLocale} ({WorkshopSlot.CompleteAmount * WorkshopSlot.StaticRecipe.Amount})";

            LayoutRebuilder.ForceRebuildLayoutImmediate(_underLineTransform);
        }

        private void UpdateIngredients()
        {
            _ingredientsContainer.ClearChildObjects();

            if (WorkshopSlot.StaticRecipe == null)
                return;

            var ingredients = StaticHelper.GetIngredients(WorkshopSlot.StaticRecipe);
            foreach (var dtoIngredient in ingredients)
            {
                var ingredient = Instantiate(_ingredientPrefab, _ingredientsContainer, false);
                var existAmount = App.Instance.Player.Inventory.GetExistAmount(dtoIngredient.Id);
                var template = existAmount >= dtoIngredient.Amount
                    ? "{0}/{1}"
                    : "<color=" + Colors.ResourceNotEnoughAmountColor + ">{0}</color>/{1}";

                var amount = string.Format(template, existAmount, dtoIngredient.Amount);
                ingredient.Initialize(dtoIngredient.Id, amount);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_underLineTransform);
        }

        private void UpdateTimers()
        {
            if (WorkshopSlot.CompleteAmount >= WorkshopSlot.NecessaryAmount)
            {
                ForceCompleteButtonCostText.text = "0";
                return;
            }

            _timeText.text = TimeHelper.Format(WorkshopSlot.TimeLeft, detailed: true);
            _allTimeText.text = TimeHelper.Format(WorkshopSlot.FullAmountTimeLeft, detailed: true);
            ForceCompleteButtonCostText.text = WorkshopSlot.ForceCompletePrice.ToString();
        }


        private void VisualizeTimeProgressBar()
        {
            if (WorkshopSlot.CompleteAmount >= WorkshopSlot.NecessaryAmount)
                return;

            var timePassed = WorkshopSlot.RecipeCraftTime - WorkshopSlot.TimeLeft;
            var sliderValue = timePassed.TotalSeconds / WorkshopSlot.RecipeCraftTime.TotalSeconds;

            _timeSlider.DOKill();
            _timeSlider.value = (float) sliderValue;
            _timeSlider.DOValue(1, (float)WorkshopSlot.TimeLeft.TotalSeconds).SetEase(Ease.Linear).SetUpdate(true);
        }


        private void OnItemAdd(InventoryItemAddEvent eventData)
        {
            UpdateIngredients();
        }

        private void OnItemChange(InventoryItemChangeEvent eventData)
        {
            UpdateIngredients();
        }

        private void OnItemRemove(InventoryItemRemoveExistEvent existEventData)
        {
            UpdateIngredients();
        }

        private void OnOneComplete()
        {
            UpdateTimers();
            VisualizeTimeProgressBar();
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
            EventManager.Instance.Subscribe<InventoryItemChangeEvent>(OnItemChange);
            EventManager.Instance.Subscribe<InventoryItemRemoveExistEvent>(OnItemRemove);

            EventManager.Instance.Subscribe<WorkshopBoostStartEvent>(OnBoostStart);
            EventManager.Instance.Subscribe<WorkshopBoostStopEvent>(OnBoostStop);

            EventManager.Instance.Subscribe<WorkshopSlotUnlockEvent>(OnSlotUnlock);
            EventManager.Instance.Subscribe<WorkshopSlotStartMeltingEvent>(OnSlotStartMelting);
            EventManager.Instance.Subscribe<WorkshopSlotClearEvent>(OnSlotClear);
            EventManager.Instance.Subscribe<WorkshopSlotMeltingTimeLeftChangeEvent>(OnSlotTimeChange);
            EventManager.Instance.Subscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
            EventManager.Instance.Subscribe<WorkshopSlotChangeEvent>(OnSlotChange);
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<WorkshopBoostStartEvent>(OnBoostStart);
                EventManager.Instance.Unsubscribe<WorkshopBoostStopEvent>(OnBoostStop);

                EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
                EventManager.Instance.Unsubscribe<InventoryItemChangeEvent>(OnItemChange);
                EventManager.Instance.Unsubscribe<InventoryItemRemoveExistEvent>(OnItemRemove);

                EventManager.Instance.Unsubscribe<WorkshopSlotUnlockEvent>(OnSlotUnlock);
                EventManager.Instance.Unsubscribe<WorkshopSlotStartMeltingEvent>(OnSlotStartMelting);
                EventManager.Instance.Unsubscribe<WorkshopSlotClearEvent>(OnSlotClear);

                EventManager.Instance.Unsubscribe<WorkshopSlotMeltingTimeLeftChangeEvent>(OnSlotTimeChange);
                EventManager.Instance.Unsubscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
                EventManager.Instance.Unsubscribe<WorkshopSlotChangeEvent>(OnSlotChange);
            }
        }

        private void OnBoostStart(WorkshopBoostStartEvent eventData)
        {
            if (eventData.BoostType == BuffValueType.Melting)
               VisualizeTimeProgressBar();
        }

        private void OnBoostStop(WorkshopBoostStopEvent eventData)
        {
            if (eventData.BoostType == BuffValueType.Melting)
                VisualizeTimeProgressBar();
        }

        private void OnSlotStartMelting(WorkshopSlotStartMeltingEvent eventData)
        {
            if (eventData.WorkshopSlot == WorkshopSlot)
                UpdateState();
        }

        private void OnSlotUnlock(WorkshopSlotUnlockEvent eventData)
        {
            if (eventData.WorkshopSlot == WorkshopSlot)
                UpdateState();
        }

        private void OnSlotClear(WorkshopSlotClearEvent eventData)
        {
            if (eventData.WorkshopSlot == WorkshopSlot)
                UpdateState();
        }

        private void OnSlotChange(WorkshopSlotChangeEvent eventData)
        {
            if (eventData.WorkshopSlot == WorkshopSlot)
            {
                OnOneComplete();
                UpdateItemAmounts();
            }
        }

        private void OnSlotComplete(WorkshopSlotCompleteEvent eventData)
        {
            if (eventData.WorkshopSlot == WorkshopSlot)
                UpdateState();
        }

        private void OnSlotTimeChange(WorkshopSlotMeltingTimeLeftChangeEvent eventData)
        {
            if (eventData.WorkshopSlot == WorkshopSlot)
                UpdateTimers();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}