using System;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowNewEnergyAbility : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _abilityTitleText;

        [SerializeField] private TextMeshProUGUI _abilityDescriptionText;

        [SerializeField] private TextMeshProUGUI _abilityCostText;

        [SerializeField] private Image _abilityIconImage;

        private Action _onClose;
        
        public void Initialize(AbilityType abilityType, Action  onClose = null)
        {
            _onClose = onClose;

            var titleLocale = LocalizationHelper.GetLocale($"{abilityType}_ability");
            var descriptionLocale = LocalizationHelper.GetLocale($"window_new_energy_ability_{abilityType}_description");

            _abilityTitleText.text = $"- { titleLocale } -";
            _abilityIconImage.sprite = SpriteHelper.GetAbilityIcon(abilityType);

            string description;
            int cost;

            switch (abilityType)
            {
                case AbilityType.ExplosiveStrike:
                    cost = App.Instance.Player.Abilities.ExplosiveStrike.StaticLevel.EnergyCost;
                    description = string.Format(
                        descriptionLocale,
                        App.Instance.Player.Abilities.ExplosiveStrike.StaticLevel.Damage);
                    break;
                case AbilityType.Freezing:
                    cost = App.Instance.Player.Abilities.Freezing.StaticLevel.EnergyCost;
                    description = string.Format(
                        descriptionLocale,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Damage,
                        MineLocalConfigs.FreezingAbilityMaxStacks,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Duration);
                    break;
                case AbilityType.Acid:
                    cost = App.Instance.Player.Abilities.Acid.StaticLevel.EnergyCost;
                    description = string.Format(
                        descriptionLocale,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Damage,
                        MineLocalConfigs.AcidAbilityMaxStacks,
                        App.Instance.Player.Abilities.Freezing.StaticLevel.Duration);
                    break;
                case AbilityType.Torch:
                    cost = MineLocalConfigs.TorchUseMomentCoast;
                    description = descriptionLocale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }

            _abilityDescriptionText.text = description;
            _abilityCostText.text = cost.ToString();
        }

        public override void OnClose()
        {
            base.OnClose();

            _onClose?.Invoke();
            _onClose = null;
        }
    }
}