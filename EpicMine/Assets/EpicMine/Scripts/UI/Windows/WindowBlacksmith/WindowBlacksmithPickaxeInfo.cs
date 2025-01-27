using System;
using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowBlacksmithPickaxeInfo : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _damageTitle;

        [Space]
        [SerializeField] private Toggle _craftTab;
        [SerializeField] private Toggle _bonusesTab;
        [SerializeField] private TextMeshProUGUI _craftTabText;
        [SerializeField] private TextMeshProUGUI _bonusesTabText;
        [SerializeField] private Color _activeTabColor;
        [SerializeField] private Color _inactiveTabColor;

        [Space]
        [SerializeField] private GameObject _craftPanel;

        [Space]
        [SerializeField] private GameObject _craftLockedPanel;
        [SerializeField] private TextMeshProUGUI _craftLockedPanelText;

        [Space]
        [SerializeField] private GameObject _craftDonatePanel;
        [SerializeField] private TextMeshProUGUI _craftDonatePanelCost;

        [Space]
        [SerializeField] private GameObject _craftAdPanel;
        [SerializeField] private TextMeshProUGUI _craftAdPanelDamageLevelText;
        [SerializeField] private TextMeshProUGUI _craftAdPanelDescriptionText;
        [SerializeField] private TextMeshProUGUI _craftAdPanelCost;

        [Space]
        [SerializeField] private GameObject _craftSelectPanel;
        [SerializeField] private TextMeshProUGUI _craftSelectPanelDescription;
        [SerializeField] private Image _craftSelectPanelButton;
        [SerializeField] private TextMeshProUGUI _craftSelectPanelDamageLevelText;

        [Space]
        [SerializeField] private GameObject _craftCreatePanel;
        [SerializeField] private TextMeshProUGUI _craftCreatePanelCostText;
        [SerializeField] private ItemView _craftCreatePanelIngredientPrefab;
        [SerializeField] private Transform _craftCreatePanelIngredientsContainer;
        [SerializeField] private Image _craftCreatePanelButton;
        [SerializeField] private TextMeshProUGUI _craftCreatePanelDamageLevelText;

        [Space]
        [SerializeField] private GameObject _bonusesPanel;
        [SerializeField] private WindowBlacksmithPickaxeInfoBonusItem _bonusesPanelItemPrefab;
        [SerializeField] private Transform _bonusesPanelContainer;

        private Core.Pickaxe _pickaxe;


        public void Initialize(Core.Pickaxe pickaxe)
        {
            Clear();

            _pickaxe = pickaxe;

            _icon.sprite = SpriteHelper.GetIcon(_pickaxe.StaticPickaxe.Id);
            _icon.color = _pickaxe.IsCreated || _pickaxe.IsUnlocked ? Color.white : Color.black;
            _title.text = LocalizationHelper.GetLocale(_pickaxe.StaticPickaxe.Id);

            string damageText;
            var damageLocale = LocalizationHelper.GetLocale("damage");

            if (_pickaxe.IsCreated || _pickaxe.IsUnlocked)
            {
                damageText = $"{damageLocale}: <color=#C3A57B>{_pickaxe.StaticPickaxe.Damage}</color>";

                if (App.Instance.Player.Blacksmith.SelectedPickaxe != _pickaxe)
                {
                    var damageDiff = _pickaxe.StaticPickaxe.Damage - App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Damage;
                    if (Math.Abs(damageDiff) > 0)
                    {
                        var color = damageDiff > 0 ? "68c134" : "c93636";
                        var diffText = damageDiff > 0 ? $"+{damageDiff}" : damageDiff.ToString();
                        damageText += $" <color=#{color}>({diffText})</color>";
                    }
                }
            }
            else
                damageText = $"{damageLocale}: <color=#C3A57B>???</color>";

            _damageTitle.text = damageText;

            UpdateRequiredDamageLevelTexts();
            UpdateCraftPanel();
            UpdateBonusesPanel();
        }

        public void ReInitialize()
        {
            Initialize(_pickaxe);
        }

        public void Select()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            if (App.Instance.Player.Skills.Damage.Number + 1 >= _pickaxe.StaticPickaxe.RequiredDamageLevel)
            {
                App.Instance.Player.Blacksmith.Select(_pickaxe);
                UpdateCraftSelectPanel();
            }
        }

        public void Create()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _pickaxe.Create();
        }

        public void OnTabClick()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            _craftTabText.color = _inactiveTabColor;
            _bonusesTabText.color = _inactiveTabColor;
            _craftPanel.SetActive(false);
            _bonusesPanel.SetActive(false);

            if (_craftTab.isOn)
            {
                _craftTab.transform.SetAsLastSibling();
                _craftTabText.color = _activeTabColor;
                _craftPanel.SetActive(true);
            }
            else
            {
                _bonusesTab.transform.SetAsLastSibling();
                _bonusesTabText.color = _activeTabColor;
                _bonusesPanel.SetActive(true);
            }
        }


        private void UpdateRequiredDamageLevelTexts()
        {
            var text = App.Instance.Player.Skills.Damage.Number + 1 >= _pickaxe.StaticPickaxe.RequiredDamageLevel
                ? _pickaxe.StaticPickaxe.RequiredDamageLevel.ToString()
                : "<color=" + Colors.ResourceNotEnoughAmountColor + ">" + _pickaxe.StaticPickaxe.RequiredDamageLevel + "</color>";

            _craftAdPanelDamageLevelText.text = text;
            _craftSelectPanelDamageLevelText.text = text;
            _craftCreatePanelDamageLevelText.text = text;
        }

        private void UpdateCraftPanel()
        {
            if (_pickaxe.IsCreated)
            {
                _craftSelectPanel.SetActive(true);
                UpdateCraftSelectPanel();
                return;
            }

            if (!_pickaxe.IsUnlocked)
            {
                string text;

                if (_pickaxe.StaticPickaxe.Type == PickaxeType.Mythical)
                    text = LocalizationHelper.GetLocale("window_blacksmith_mythical_locked");
                else
                {
                    var locale = LocalizationHelper.GetLocale("window_blacksmith_locked");
                    text = string.Format(locale, _pickaxe.StaticPickaxe.RequiredTierNumber);
                }

                _craftLockedPanelText.text = text;
                _craftLockedPanel.SetActive(true);
                return;
            }

            if (_pickaxe.StaticPickaxe.Type == PickaxeType.Donate)
            {
                _craftDonatePanel.SetActive(true);
                _craftDonatePanelCost.text = _pickaxe.StaticPickaxe.Cost.ToString();
                return;
            }

            if (_pickaxe.StaticPickaxe.Type == PickaxeType.Ad)
            {
                int currentVal;
                App.Instance.Player.Blacksmith.AdPickaxes.TryGetValue(_pickaxe.StaticPickaxe.Id, out currentVal);
                var leftVal = _pickaxe.StaticPickaxe.Cost - currentVal;

                _craftAdPanel.SetActive(true);
                _craftAdPanelCost.text = leftVal.ToString();
                var descriptionLocale = LocalizationHelper.GetLocale("window_blacksmith_ad_description");
                _craftAdPanelDescriptionText.text = string.Format(descriptionLocale, leftVal);
                return;
            }

            _craftCreatePanel.SetActive(true);
            UpdateCraftCreatePanel();
        }

        private void UpdateCraftSelectPanel()
        {
            if (_pickaxe == App.Instance.Player.Blacksmith.SelectedPickaxe)
            {
                _craftSelectPanelDescription.text = LocalizationHelper.GetLocale("window_blacksmith_already_in_use");
                _craftSelectPanelButton.sprite = App.Instance.ReferencesTables.Sprites.ButtonGrey;
            }
            else
            {
                if (App.Instance.Player.Skills.Damage.Number + 1 >= _pickaxe.StaticPickaxe.RequiredDamageLevel)
                {
                    _craftSelectPanelDescription.text = LocalizationHelper.GetLocale("window_blacksmith_available_for_select");
                    _craftSelectPanelButton.sprite = App.Instance.ReferencesTables.Sprites.ButtonGreen;
                }
                else
                {
                    _craftSelectPanelDescription.text = LocalizationHelper.GetLocale("window_blacksmith_invalid_damage_level");
                    _craftSelectPanelButton.sprite = App.Instance.ReferencesTables.Sprites.ButtonGrey;
                }
            }
        }

        private void UpdateCraftCreatePanel()
        {
            _craftCreatePanelIngredientsContainer.ClearChildObjects();

            var ingredients = StaticHelper.GetIngredients(_pickaxe.StaticPickaxe);
            var hasAllIngredients = true;

            foreach (var dtoItem in ingredients)
            {
                var ingredient = Instantiate(_craftCreatePanelIngredientPrefab, _craftCreatePanelIngredientsContainer,
                    false);
                var existAmount = App.Instance.Player.Inventory.GetExistAmount(dtoItem.Id);
                string amount;

                if (existAmount >= dtoItem.Amount)
                    amount = dtoItem.Amount.ToString();
                else
                {
                    amount = "<color=" + Colors.ResourceNotEnoughAmountColor + ">" + dtoItem.Amount + "</color>";
                    hasAllIngredients = false;
                }

                ingredient.Initialize(dtoItem.Id, amount);
            }

            var goldExist = App.Instance.Player.Wallet.Has(new Dto.Currency(CurrencyType.Gold, _pickaxe.StaticPickaxe.Cost));
            _craftCreatePanelCostText.text = goldExist
                ? _pickaxe.StaticPickaxe.Cost.ToString()
                : "<color=" + Colors.ResourceNotEnoughAmountColor + ">" + _pickaxe.StaticPickaxe.Cost + "</color>";

            if (!goldExist)
                hasAllIngredients = false;

            _craftCreatePanelButton.sprite = hasAllIngredients
                ? App.Instance.ReferencesTables.Sprites.ButtonGrown
                : App.Instance.ReferencesTables.Sprites.ButtonGrey;
        }

        private void UpdateBonusesPanel()
        {
            _bonusesPanelContainer.ClearChildObjects();

            if (_pickaxe.IsCreated || _pickaxe.IsUnlocked)
            {
                if (_pickaxe.StaticPickaxe.BonusGoldPercent != null && _pickaxe.StaticPickaxe.BonusGoldPercent > 0)
                {
                    var goldLocale = LocalizationHelper.GetLocale("window_blacksmith_bonus_gold");
                    AddBonusItem(SpriteHelper.GetCurrencyIcon(CurrencyType.Gold), string.Format(goldLocale, _pickaxe.StaticPickaxe.BonusGoldPercent));
                }

                if (_pickaxe.StaticPickaxe.BonusDamagePercent != null && _pickaxe.StaticPickaxe.BonusDamagePercent > 0)
                {
                    var powerLocale = LocalizationHelper.GetLocale("power_description");
                    AddBonusItem(App.Instance.ReferencesTables.Sprites.DamageIcon, $"{powerLocale} +{_pickaxe.StaticPickaxe.BonusDamagePercent}%");
                }

                if (_pickaxe.StaticPickaxe.BonusFortunePercent != null && _pickaxe.StaticPickaxe.BonusFortunePercent > 0)
                {
                    var fortuneLocale = LocalizationHelper.GetLocale("fortune_description");
                    AddBonusItem(App.Instance.ReferencesTables.Sprites.FortuneIcon, $"{fortuneLocale} +{_pickaxe.StaticPickaxe.BonusFortunePercent}%");
                }

                if (_pickaxe.StaticPickaxe.BonusCritPercent != null && _pickaxe.StaticPickaxe.BonusCritPercent > 0)
                {
                    var critLocale = LocalizationHelper.GetLocale("crit_description");
                    AddBonusItem(App.Instance.ReferencesTables.Sprites.CritIcon, $"{critLocale} +{_pickaxe.StaticPickaxe.BonusCritPercent}%");
                }

                if (!string.IsNullOrEmpty(_pickaxe.StaticPickaxe.BonusDropItemId) && _pickaxe.StaticPickaxe.BonusDropItemPercent != null && _pickaxe.StaticPickaxe.BonusDropItemPercent > 0)
                {
                    var locale = LocalizationHelper.GetLocale("window_blacksmith_bonus_resource_chance");
                    var itemLocale = LocalizationHelper.GetLocale(_pickaxe.StaticPickaxe.BonusDropItemId).ToLower();
                    AddBonusItem(SpriteHelper.GetIcon(_pickaxe.StaticPickaxe.BonusDropItemId), string.Format(locale, itemLocale, _pickaxe.StaticPickaxe.BonusDropItemPercent));
                }

                if (!string.IsNullOrEmpty(_pickaxe.StaticPickaxe.HitEffect))
                {
                    var hitEffect =
                        App.Instance.StaticData.PickaxeEffects.Find(x => x.Id == _pickaxe.StaticPickaxe.HitEffect);

                    if (hitEffect != null)
                    {
                        var locale = LocalizationHelper.GetLocale($"{hitEffect.Id}_description");

                        Sprite icon = null;

                        if (hitEffect.Type == PickaxeEffectType.HealthRecover)
                            icon = App.Instance.ReferencesTables.Sprites.HeartIcon;
                        else if (hitEffect.Type == PickaxeEffectType.Freeze)
                            icon = App.Instance.ReferencesTables.Sprites.FreezingAbilityBuffIcon;

                        AddBonusItem(icon, locale);
                    }
         
                }
            }
        }

        private void AddBonusItem(Sprite sprite, string text)
        {
            var item = Instantiate(_bonusesPanelItemPrefab, _bonusesPanelContainer, false);
            item.Initialize(sprite, text);
        }

        private void Clear()
        {
            _craftLockedPanel.SetActive(false);
            _craftDonatePanel.SetActive(false);
            _craftAdPanel.SetActive(false);
            _craftCreatePanel.SetActive(false);
            _craftSelectPanel.SetActive(false);
        }
    }
}