using System;
using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowUpgradeAbilitiesPanelAbility : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;

        [SerializeField] private TextMeshProUGUI _energyCost;

        [SerializeField] private Image _icon;

        [SerializeField] private GameObject _upgradeButton;

        [SerializeField] private Image _upgradeButtonImage;

        [SerializeField] private Image _upgradeButtonArrowIcon;

        [SerializeField] private ItemView _upgradeButtonCostIcon;

        [SerializeField] private TextMeshProUGUI _upgradeButtonCostText;

        [SerializeField] private Sprite _canUpgradeArrowSprite;

        [SerializeField] private Sprite _cantUpgradeArrowSprite;

        [SerializeField] private Transform _parametersContainer;

        [SerializeField] private WindowUpgradeAbilitiesPanelAbilityParameter _parameterPrefab;

        private Core.AbilityLevel _abilityLevel;

        private Action<string> _onClickHint;


        public void Initialize(Core.AbilityLevel abilityLevel, Action<string> onClickHint)
        {
            _abilityLevel = abilityLevel;
            _onClickHint = onClickHint;
            UpdateView();
        }

        public void Upgrade()
        {
            _abilityLevel.Up();
        }

        public void ShowHint()
        {
            var locale = LocalizationHelper.GetLocale($"{_abilityLevel.Type}_ability_description");
            string hint;

            switch (_abilityLevel.Type)
            {
                case AbilityType.ExplosiveStrike:
                    hint = string.Format(
                        locale,
                        App.Instance.Player.Abilities.ExplosiveStrike.StaticLevel.Damage,
                        App.Instance.Player.Abilities.ExplosiveStrike.StaticLevel.Cooldown);
                    break;
                case AbilityType.Freezing:
                    hint = string.Format(
                        locale,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Damage,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.AdditionalParameter,
                        MineLocalConfigs.FreezingAbilityMaxStacks,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Duration,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Cooldown);
                    break;
                case AbilityType.Acid:
                    hint = string.Format(
                        locale,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Damage,
                        MineLocalConfigs.AcidAbilityMaxStacks,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Duration,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Cooldown);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_abilityLevel.Type), _abilityLevel.Type, null);
            }

            _onClickHint?.Invoke(hint);
        }


        private void Awake()
        {
            EventManager.Instance.Subscribe<AbilityLevelChangeEvent>(OnAbilityLevelChange);
            EventManager.Instance.Subscribe<CurrencyChangeEvent>(OnCurrencyChange);
            EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
            EventManager.Instance.Subscribe<InventoryItemRemoveExistEvent>(OnItemRemove);
        }

        private void OnDestroy()
        {
            _parametersContainer.ClearChildObjects();

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<AbilityLevelChangeEvent>(OnAbilityLevelChange);
                EventManager.Instance.Unsubscribe<CurrencyChangeEvent>(OnCurrencyChange);
                EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
                EventManager.Instance.Unsubscribe<InventoryItemRemoveExistEvent>(OnItemRemove);
            }
        }


        private void UpdateView()
        {
            _parametersContainer.ClearChildObjects();

            var abilityLocale = LocalizationHelper.GetLocale($"{_abilityLevel.Type.ToString().ToLower()}_ability");
            var levelLocale = LocalizationHelper.GetLocale("window_upgrade_ability_level");
            _title.text = $"{abilityLocale} <color=#7D6A4F>{_abilityLevel.Number + 1} {levelLocale}</color>";

            _energyCost.text = _abilityLevel.StaticLevel.EnergyCost.ToString();

            switch (_abilityLevel.Type)
            {
                case AbilityType.ExplosiveStrike:
                    InitForExplosiveStrike();
                    break;
                case AbilityType.Freezing:
                    InitForFreezing();
                    break;
                case AbilityType.Acid:
                    InitForAcid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateButton();
        }

        private void UpdateButton()
        {
            if (_abilityLevel.NextStaticLevel == null)
            {
                _upgradeButton.SetActive(false);
                return;
            }

            if (_abilityLevel.NextStaticLevel.CostCurrencyType != null && _abilityLevel.NextStaticLevel.CostCurrencyAmount > 0)
            {
                _upgradeButtonCostIcon.Initialize(SpriteHelper.GetCurrencyIcon(_abilityLevel.NextStaticLevel.CostCurrencyType.Value), string.Empty);
                _upgradeButtonCostText.text = _abilityLevel.NextStaticLevel.CostCurrencyAmount.ToString();
                _upgradeButtonCostIcon.EnableRaycast(false);
            }
            else if (!string.IsNullOrEmpty(_abilityLevel.NextStaticLevel.CostItemId) && _abilityLevel.NextStaticLevel.CostItemAmount > 0)
            {
                _upgradeButtonCostIcon.Initialize(_abilityLevel.NextStaticLevel.CostItemId, string.Empty);
                _upgradeButtonCostText.text = _abilityLevel.NextStaticLevel.CostItemAmount.ToString();
                _upgradeButtonCostIcon.EnableRaycast(true);
            }

            _upgradeButtonImage.sprite = _abilityLevel.CanUpgrade
                ? App.Instance.ReferencesTables.Sprites.ButtonGreen
                : App.Instance.ReferencesTables.Sprites.ButtonGrey;

            _upgradeButtonArrowIcon.sprite = _abilityLevel.CanUpgrade
                ? _canUpgradeArrowSprite
                : _cantUpgradeArrowSprite;
        }


        private void InitForExplosiveStrike()
        {
            _icon.sprite = App.Instance.ReferencesTables.Sprites.ExplosiveStrikeAbilityIcon;
            AddDamageParameter("window_upgrade_ability_damage");
            AddCooldownParameter();
        }

        private void InitForFreezing()
        {
            _icon.sprite = App.Instance.ReferencesTables.Sprites.FreezingAbilityIcon;
            AddDamageParameter("window_upgrade_ability_additional_damage");
            AddAdditionalParameter(App.Instance.ReferencesTables.Sprites.FreezingAbilityAdditionalIcon, "freezing_ability_additional_parameter_title", "%");
            AddDurationParameter(App.Instance.ReferencesTables.Sprites.FreezingAbilityBuffIcon);
            AddCooldownParameter();
        }

        private void InitForAcid()
        {
            _icon.sprite = App.Instance.ReferencesTables.Sprites.AcidAbilityIcon;
            AddDamageParameter("window_upgrade_ability_damage_per_second");
            AddDurationParameter(App.Instance.ReferencesTables.Sprites.AcidAbilityBuffIcon);
            AddCooldownParameter();
        }


        private void AddDamageParameter(string localeKey)
        {
            var icon = App.Instance.ReferencesTables.Sprites.AbilityDamageIcon;
            var title = LocalizationHelper.GetLocale(localeKey) + ":";
            var currentValue = _abilityLevel.StaticLevel.Damage;
            var nextValue = _abilityLevel.NextStaticLevel?.Damage ?? currentValue;
            var isIncreased = nextValue > currentValue;
            var value = isIncreased
                ? $"{currentValue} <color=#79C131>(+{nextValue - currentValue})</color>"
                : $"{currentValue}";

            Instantiate(_parameterPrefab, _parametersContainer, false).Initialize(icon, title, value, isIncreased);
        }

        private void AddAdditionalParameter(Sprite sprite, string localeKey, string valueChar, bool increaseIsPositive = true)
        {
            var title = LocalizationHelper.GetLocale(localeKey) + ":";
            var currentValue = _abilityLevel.StaticLevel.AdditionalParameter;
            var nextValue = _abilityLevel.NextStaticLevel?.AdditionalParameter ?? currentValue;
            var isIncreased = increaseIsPositive ? nextValue > currentValue : nextValue < currentValue;
            var value = isIncreased
                ? $"{currentValue}{valueChar} <color=#79C131>(+{nextValue - currentValue})</color>"
                : $"{currentValue}{valueChar}";

            Instantiate(_parameterPrefab, _parametersContainer, false).Initialize(sprite, title, value, isIncreased);
        }

        private void AddCooldownParameter()
        {
            var icon = App.Instance.ReferencesTables.Sprites.AbilityCooldownIcon;
            var title = LocalizationHelper.GetLocale("window_upgrade_ability_cooldown") + ":";
            var currentValue = _abilityLevel.StaticLevel.Cooldown;
            var nextValue = _abilityLevel.NextStaticLevel?.Cooldown ?? currentValue;
            var isIncreased = nextValue < currentValue;
            var secondsLocale = LocalizationHelper.GetLocale("window_upgrade_ability_time");
            var value = isIncreased
                ? $"{currentValue} {secondsLocale} <color=#79C131>(-{currentValue - nextValue})</color>"
                : $"{currentValue} {secondsLocale}";

            Instantiate(_parameterPrefab, _parametersContainer, false).Initialize(icon, title, value, isIncreased);
        }

        private void AddDurationParameter(Sprite sprite)
        {
            var title = LocalizationHelper.GetLocale("window_upgrade_ability_duration") + ":";
            var currentValue = _abilityLevel.StaticLevel.Duration;
            var nextValue = _abilityLevel.NextStaticLevel?.Duration ?? currentValue;
            var isIncreased = nextValue > currentValue;
            var secondsLocale = LocalizationHelper.GetLocale("window_upgrade_ability_time");
            var value = isIncreased
                ? $"{currentValue} {secondsLocale} <color=#79C131>(-{currentValue - nextValue})</color>"
                : $"{currentValue} {secondsLocale}";

            Instantiate(_parameterPrefab, _parametersContainer, false).Initialize(sprite, title, value, isIncreased);
        }


        private void OnAbilityLevelChange(AbilityLevelChangeEvent eventData)
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.CharacteristicUpgrade);

            if (eventData.AbilityLevel.Type == _abilityLevel.Type)
                UpdateView();
        }

        private void OnCurrencyChange(CurrencyChangeEvent eventData)
        {
            UpdateButton();
        }

        private void OnItemAdd(InventoryItemAddEvent eventData)
        {
            UpdateButton();
        }

        private void OnItemRemove(InventoryItemRemoveExistEvent existEventData)
        {
            UpdateButton();
        }
    }
}