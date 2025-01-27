using System;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowUpgradeSkillsPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _damageLevelNumber;
        [SerializeField] private TextMeshProUGUI _damageLevelCurrentValue;
        [SerializeField] private TextMeshProUGUI _damageLevelValueDifference;

        [SerializeField] private Image _damageLevelUpgradeButton;
        [SerializeField] private Image _damageLevelCostButton;

        [SerializeField] private TextMeshProUGUI _damageLevelCostText;
        [SerializeField] private ItemView _damageLevelCostIcon;
        [SerializeField] private Image _damageLevelUpgradeIcon;

        [Space]
        [SerializeField] private TextMeshProUGUI _critLevelNumber;
        [SerializeField] private TextMeshProUGUI _critLevelCurrentValue;
        [SerializeField] private TextMeshProUGUI _critLevelValueDifference;

        [SerializeField] private Image _critLevelUpgradeButton;
        [SerializeField] private Image _critLevelCostButton;

        [SerializeField] private TextMeshProUGUI _critLevelCostText;
        [SerializeField] private ItemView _critLevelCostIcon;
        [SerializeField] private Image _critLevelUpgradeIcon;

        [Space]
        [SerializeField] private TextMeshProUGUI _fortuneLevelNumber;
        [SerializeField] private TextMeshProUGUI _fortuneLevelCurrentValue;
        [SerializeField] private TextMeshProUGUI _fortuneLevelValueDifference;

        [SerializeField] private Image _fortuneLevelUpgradeButton;
        [SerializeField] private Image _fortuneLevelCostButton;

        [SerializeField] private TextMeshProUGUI _fortuneLevelCostText;
        [SerializeField] private ItemView _fortuneLevelCostIcon;
        [SerializeField] private Image _fortuneLevelUpgradeIcon;

        [SerializeField] private Sprite _canUpgradeSprite;
        [SerializeField] private Sprite _cantUpgradeSprite;


        public void Initialize()
        {
            UpdateDamageLevelView();
            UpdateCritLevelView();
            UpdateFortuneLevelView();

            Clear();

            EventManager.Instance.Subscribe<SkillLevelChangeEvent>(OnCharacteristicLevelChange);
            EventManager.Instance.Subscribe<CurrencyChangeEvent>(OnCurrencyChange);
            EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
            EventManager.Instance.Subscribe<InventoryItemRemoveEvent>(OnItemRemove);
        }


        public void DamageLevelUp()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            App.Instance.Player.Skills.Damage.Up();
        }

        public void CritLevelUp()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            App.Instance.Player.Skills.Crit.Up();
        }

        public void FortuneLevelUp()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            App.Instance.Player.Skills.Fortune.Up();
        }


        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<SkillLevelChangeEvent>(OnCharacteristicLevelChange);
                EventManager.Instance.Unsubscribe<CurrencyChangeEvent>(OnCurrencyChange);
                EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
                EventManager.Instance.Unsubscribe<InventoryItemRemoveEvent>(OnItemRemove);
            }
        }

        private void UpdateDamageLevelView()
        {
            var level = App.Instance.Player.Skills.Damage;
            var isLastLevel = level.NextStaticLevel == null;

            _damageLevelCurrentValue.text = $"{level.Value:F2}";
            _damageLevelNumber.text = $"{level.Number + 1} {LocalizationHelper.GetLocale("level")}";
            _damageLevelUpgradeButton.sprite = App.Instance.ReferencesTables.Sprites.ButtonGreen;

            if (isLastLevel)
            {
                _damageLevelUpgradeButton.gameObject.SetActive(false);
                _damageLevelCostButton.gameObject.SetActive(false);
                return;
            }

            if (level.NextStaticLevel.CostCurrencyType != null && level.NextStaticLevel.CostCurrencyAmount > 0)
            {
                _damageLevelCostIcon.Initialize(SpriteHelper.GetCurrencyIcon(level.NextStaticLevel.CostCurrencyType.Value), string.Empty);
                _damageLevelCostText.text = level.NextStaticLevel.CostCurrencyAmount.ToString();
                _damageLevelCostIcon.EnableRaycast(false);

                var dtoCurrency = new Dto.Currency(level.NextStaticLevel.CostCurrencyType.Value, level.NextStaticLevel.CostCurrencyAmount);
                var hasCurrency = App.Instance.Player.Wallet.Has(dtoCurrency);

                _damageLevelUpgradeButton.sprite = hasCurrency
                    ? App.Instance.ReferencesTables.Sprites.ButtonGreen
                    : App.Instance.ReferencesTables.Sprites.ButtonGrey;

                _damageLevelUpgradeIcon.sprite = hasCurrency
                    ? _canUpgradeSprite
                    : _cantUpgradeSprite;

            }
            else if (!string.IsNullOrEmpty(level.NextStaticLevel.CostItemId) && level.NextStaticLevel.CostItemAmount > 0)
            {
                _damageLevelCostIcon.Initialize(level.NextStaticLevel.CostItemId, string.Empty);
                _damageLevelCostText.text = level.NextStaticLevel.CostItemAmount.ToString();
                _damageLevelCostIcon.EnableRaycast(true);

                var dtoItem = new Item(level.NextStaticLevel.CostItemId, level.NextStaticLevel.CostItemAmount);
                var hasItem = App.Instance.Player.Inventory.Has(dtoItem);

                _damageLevelUpgradeButton.sprite = hasItem
                    ? App.Instance.ReferencesTables.Sprites.ButtonGreen
                    : App.Instance.ReferencesTables.Sprites.ButtonGrey;

                _damageLevelUpgradeIcon.sprite = hasItem
                    ? _canUpgradeSprite
                    : _cantUpgradeSprite;
            }
            else
            {
                _damageLevelUpgradeButton.gameObject.SetActive(false);
                return;
            }

            var diff = level.NextStaticLevel.Value - level.Value;
            _damageLevelValueDifference.text = $"+{diff:F2}";
        }

        private void UpdateCritLevelView()
        {
            var level = App.Instance.Player.Skills.Crit;
            var isLastLevel = level.NextStaticLevel == null;

            _critLevelCurrentValue.text = $"{level.Value:F2}%";
            _critLevelNumber.text = $"{level.Number + 1} {LocalizationHelper.GetLocale("level")}";
            _critLevelUpgradeButton.sprite = App.Instance.ReferencesTables.Sprites.ButtonGreen;

            if (isLastLevel)
            {
                _critLevelUpgradeButton.gameObject.SetActive(false);
                _critLevelCostButton.gameObject.SetActive(false);
                return;
            }

            if (level.NextStaticLevel.CostCurrencyType != null && level.NextStaticLevel.CostCurrencyAmount > 0)
            {
                _critLevelCostIcon.Initialize(SpriteHelper.GetCurrencyIcon(level.NextStaticLevel.CostCurrencyType.Value), string.Empty);
                _critLevelCostText.text = level.NextStaticLevel.CostCurrencyAmount.ToString();
                _critLevelCostIcon.EnableRaycast(false);

                var dtoCurrency = new Dto.Currency(level.NextStaticLevel.CostCurrencyType.Value, level.NextStaticLevel.CostCurrencyAmount);
                var hasCurrency = App.Instance.Player.Wallet.Has(dtoCurrency);

                _critLevelUpgradeButton.sprite = hasCurrency
                    ? App.Instance.ReferencesTables.Sprites.ButtonGreen
                    : App.Instance.ReferencesTables.Sprites.ButtonGrey;

                _critLevelUpgradeIcon.sprite = hasCurrency
                    ? _canUpgradeSprite
                    : _cantUpgradeSprite;
            }
            else if (!string.IsNullOrEmpty(level.NextStaticLevel.CostItemId) && level.NextStaticLevel.CostItemAmount > 0)
            {
                _critLevelCostIcon.Initialize(level.NextStaticLevel.CostItemId, string.Empty);
                _critLevelCostText.text = level.NextStaticLevel.CostItemAmount.ToString();
                _critLevelCostIcon.EnableRaycast(true);

                var dtoItem = new Item(level.NextStaticLevel.CostItemId, level.NextStaticLevel.CostItemAmount);
                var hasItem = App.Instance.Player.Inventory.Has(dtoItem);

                _critLevelUpgradeButton.sprite = hasItem
                    ? App.Instance.ReferencesTables.Sprites.ButtonGreen
                    : App.Instance.ReferencesTables.Sprites.ButtonGrey;

                _critLevelUpgradeIcon.sprite = hasItem
                    ? _canUpgradeSprite
                    : _cantUpgradeSprite;
            }
            else
            {
                _critLevelUpgradeButton.gameObject.SetActive(false);
                return;
            }

            var diff = level.NextStaticLevel.Value - level.Value;
            _critLevelValueDifference.text = $"+{diff:F2}%";
        }

        private void UpdateFortuneLevelView()
        {
            var level = App.Instance.Player.Skills.Fortune;
            var isLastLevel = level.NextStaticLevel == null;

            _fortuneLevelCurrentValue.text = $"{level.Value:F2}%";
            _fortuneLevelNumber.text = $"{level.Number + 1} {LocalizationHelper.GetLocale("level")}";
            _fortuneLevelUpgradeButton.sprite = App.Instance.ReferencesTables.Sprites.ButtonGreen;

            if (isLastLevel)
            {
                _fortuneLevelUpgradeButton.gameObject.SetActive(false);
                _fortuneLevelCostButton.gameObject.SetActive(false);
                return;
            }

            if (level.NextStaticLevel.CostCurrencyType != null && level.NextStaticLevel.CostCurrencyAmount > 0)
            {
                _fortuneLevelCostIcon.Initialize(SpriteHelper.GetCurrencyIcon(level.NextStaticLevel.CostCurrencyType.Value), string.Empty);
                _fortuneLevelCostText.text = level.NextStaticLevel.CostCurrencyAmount.ToString();
                _fortuneLevelCostIcon.EnableRaycast(false);

                var dtoCurrency = new Dto.Currency(level.NextStaticLevel.CostCurrencyType.Value, level.NextStaticLevel.CostCurrencyAmount);
                var hasCurrency = App.Instance.Player.Wallet.Has(dtoCurrency);

                _fortuneLevelUpgradeButton.sprite = hasCurrency
                    ? App.Instance.ReferencesTables.Sprites.ButtonGreen
                    : App.Instance.ReferencesTables.Sprites.ButtonGrey;

                _fortuneLevelUpgradeIcon.sprite = hasCurrency
                    ? _canUpgradeSprite
                    : _cantUpgradeSprite;
            }
            else if (!string.IsNullOrEmpty(level.NextStaticLevel.CostItemId) && level.NextStaticLevel.CostItemAmount > 0)
            {
                _fortuneLevelCostIcon.Initialize(level.NextStaticLevel.CostItemId, string.Empty);
                _fortuneLevelCostText.text = level.NextStaticLevel.CostItemAmount.ToString();
                _fortuneLevelCostIcon.EnableRaycast(true);

                var dtoItem = new Item(level.NextStaticLevel.CostItemId, level.NextStaticLevel.CostItemAmount);
                var hasItem = App.Instance.Player.Inventory.Has(dtoItem);

                _fortuneLevelUpgradeButton.sprite = hasItem
                    ? App.Instance.ReferencesTables.Sprites.ButtonGreen
                    : App.Instance.ReferencesTables.Sprites.ButtonGrey;

                _fortuneLevelUpgradeIcon.sprite = hasItem
                    ? _canUpgradeSprite
                    : _cantUpgradeSprite;
            }
            else
            {
                _fortuneLevelUpgradeButton.gameObject.SetActive(false);
                return;
            }

            var diff = level.NextStaticLevel.Value - level.Value;
            _fortuneLevelValueDifference.text = $"+{diff:F2}%";
        }


        private void OnCharacteristicLevelChange(SkillLevelChangeEvent eventData)
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.CharacteristicUpgrade);

            switch (eventData.SkillLevel.Type)
            {
                case SkillType.Damage:
                    UpdateDamageLevelView();
                    break;
                case SkillType.Fortune:
                    UpdateFortuneLevelView();
                    break;
                case SkillType.Crit:
                    UpdateCritLevelView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void OnCurrencyChange(CurrencyChangeEvent eventData)
        {
            UpdateDamageLevelView();
            UpdateCritLevelView();
            UpdateFortuneLevelView();
        }

        private void OnItemAdd(InventoryItemAddEvent eventData)
        {
            UpdateDamageLevelView();
            UpdateCritLevelView();
            UpdateFortuneLevelView();
        }

        private void OnItemRemove(InventoryItemRemoveEvent existEventData)
        {
            UpdateDamageLevelView();
            UpdateCritLevelView();
            UpdateFortuneLevelView();
        }
    }
}