using System;
using System.Globalization;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowInventoryItemInfoPanel : MonoBehaviour
    {
        public string StaticItemId { get; private set; }
        
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _currentAmountText;
        
        [SerializeField] private GameObject _sellPanel;
        [SerializeField] private GameObject _usePanel;
        
        [SerializeField] private TextMeshProUGUI _sellPriceText;
        [SerializeField] private Slider _sellAmountSlider;
        [SerializeField] private TextMeshProUGUI _sellAmountText;
        [SerializeField] private TextMeshProUGUI _sellSummText;
        [SerializeField] private Image _sellButton;

        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private GameObject _useButton;

        private int _currentAmount;
        private int _lastSellAmount;
        private int _sellPrice;

        
        public void Initialize(string staticItemId, int currentAmount)
        {
            Clear();
            
            EventManager.Instance.Subscribe<InventoryItemChangeEvent>(OnItemChange);
            EventManager.Instance.Subscribe<InventoryItemRemoveExistEvent>(OnItemRemove);

            StaticItemId = staticItemId;
            
            _title.text = LocalizationHelper.GetLocale(StaticItemId);
            _icon.sprite = SpriteHelper.GetIcon(StaticItemId);
            _icon.gameObject.SetActive(true);

            UpdateCurrentAmount(currentAmount);
            App.Instance.Controllers.RedDotsController.ViewItem(StaticItemId);

            var resource = App.Instance.StaticData.Resources.FirstOrDefault(r => r.Id == StaticItemId);
            if (resource != null)
            {
                if (resource.Type == ResourceType.Shard)
                    return;
                
                _sellPrice = resource.Price;
                ShowSellPanel();
                return;
            }

            var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(r => r.Id == StaticItemId);
            if (hilt != null)
            {
                _sellPrice = hilt.Price;
                ShowSellPanel();
                return;
            }
            

            ShowUsePanel();
        }

        public void IncreaseSellingAmount()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            if (_sellAmountSlider.value >= _currentAmount)
                return;

            _sellAmountSlider.value += 1;
            OnSellAmountChange();
        }

        public void DecreaseSellingAmount()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            if (_sellAmountSlider.value <= 1)
                return;

            _sellAmountSlider.value -= 1;
            OnSellAmountChange();
        }

        public void ChangeSellingAmountFromSlider(float value)
        {
           OnSellAmountChange();
        }

        public void Sell()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            if (string.IsNullOrEmpty(StaticItemId))
                return;

            var sellAmount = (int) _sellAmountSlider.value;
            var sum = _sellPrice * sellAmount;
            var dtoItem = new Item(StaticItemId, sellAmount);
            var dtoCurrency = new Dto.Currency(CurrencyType.Gold, sum);

            var windowConfirm = WindowManager.Instance.Show<WindowSellConfirm>();
            windowConfirm.Initialize(dtoItem, dtoCurrency, () =>
            {
                var removed = App.Instance.Player.Inventory.Remove(dtoItem, SpendType.Selling);
                if (!removed)
                    return;

                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Sell);
                App.Instance.Player.Wallet.Add(dtoCurrency, IncomeSourceType.FromSell, $"{StaticItemId}x{sellAmount}={sum}");
                _lastSellAmount = sellAmount;
            });
        }

        public void Use()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            var shopChest = App.Instance.StaticData.ShopChests.FirstOrDefault(i => i.Id == StaticItemId);
            var pvpSimpleChest = StaticItemId == PvpLocalConfig.PvpSimpleChestItemId; // TODO chest list
            var pvpRoyalChest = StaticItemId == PvpLocalConfig.PvpRoyalChestItemId;
            var pvpWinnerChest = StaticItemId == PvpLocalConfig.PvpWinnerChestItemId;

            if (shopChest == null && !pvpRoyalChest  && !pvpSimpleChest && !pvpWinnerChest)
                return;

            if (shopChest != null)
            {
                var windowChestBreaked = WindowManager.Instance.Show<WindowOpenShopChest>();
                windowChestBreaked.Initialize(shopChest);
            }
            else
            {
                var selectedTier = App.Instance.Player.Dungeon.LastOpenedTier.Number;
                var windowPvpChest = WindowManager.Instance.Show<WindowOpenPvpChest>();
                windowPvpChest.Initialize(pvpSimpleChest ? 
                    PvpChestType.Simple : pvpRoyalChest ?
                        PvpChestType.Royal : PvpChestType.Winner, selectedTier , true);
            }
        }


        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            StaticItemId = string.Empty;
            _sellPanel.SetActive(false);
            _usePanel.SetActive(false);
            _useButton.SetActive(false);
            _currentAmountText.text = string.Empty;
            _lastSellAmount = int.MaxValue;

            _title.text = string.Empty;
            _icon.sprite = null;
            _icon.gameObject.SetActive(false);

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<InventoryItemChangeEvent>(OnItemChange);
                EventManager.Instance.Unsubscribe<InventoryItemRemoveExistEvent>(OnItemRemove);
            }
        }

        private void ShowSellPanel()
        {
            _sellPanel.SetActive(true);
            _sellPriceText.text = _sellPrice.ToString();
            UpdateSellPanel();
        }

        private void ShowUsePanel()
        {
            _usePanel.SetActive(true);
            UpdateUsePanel();
        }

        
        private void UpdateCurrentAmount(int currentAmount)
        {
            _currentAmount = currentAmount;
            _currentAmountText.text = _currentAmount.ToString();

            if (currentAmount <= 0)
            {
                Clear();
                return;
            }
            
            UpdateSellPanel();
        }
        
        private void UpdateSellPanel()
        {
            _sellAmountSlider.maxValue = _currentAmount;
            _sellAmountSlider.value = _lastSellAmount < _currentAmount
                ? _lastSellAmount
                : _currentAmount;

            _sellAmountSlider.direction = _currentAmount <= 1
                ? Slider.Direction.RightToLeft
                : Slider.Direction.LeftToRight;
           
            OnSellAmountChange();
        }
        
        private void OnSellAmountChange()
        {
            _sellAmountText.text = _sellAmountSlider.value.ToString(CultureInfo.InvariantCulture);
            _sellSummText.text = (_sellPrice * _sellAmountSlider.value).ToString(CultureInfo.InvariantCulture);
        }

        
        private void UpdateUsePanel()
        {
            var locale = LocalizationHelper.GetLocale(StaticItemId + "_description");
            
            var potion = App.Instance.StaticData.Potions.FirstOrDefault(i => i.Id == StaticItemId);
            if (potion != null)
            {
                switch (potion.Type)
                {
                    case PotionType.Damage:
                        _description.text = string.Format(locale, potion.Value, potion.Time);
                        break;
                    case PotionType.Health:
                    case PotionType.Energy:
                        _description.text = string.Format(locale, potion.Value);
                        break;
                }
                return;
            }

            var tnt = App.Instance.StaticData.Tnt.FirstOrDefault(i => i.Id == StaticItemId);
            if (tnt != null)
            {
                _description.text = string.Format(locale, tnt.DamagePercent);
                return;
            }

            _description.text = locale;
            _useButton.SetActive(true);
        }
        

        private void OnItemChange(InventoryItemChangeEvent eventData)
        {
            if (string.IsNullOrEmpty(StaticItemId) || StaticItemId != eventData.Item.Id)
                return;

            UpdateCurrentAmount(eventData.Item.Amount);
        }

        private void OnItemRemove(InventoryItemRemoveExistEvent eventData)
        {
            if (string.IsNullOrEmpty(StaticItemId) || StaticItemId != eventData.ItemId)
                return;

            UpdateCurrentAmount(0);
        }
    }
}