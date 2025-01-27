using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowUpgradeAbilitiesPanel : MonoBehaviour
    {
        [SerializeField] private WindowUpgrade _window;

        [SerializeField] private Transform _container;

        [SerializeField] private WindowUpgradeAbilitiesPanelAbility _abilityPrefab;

        [SerializeField] private GameObject _lockedAbilityPrefab;

        public void Initialize()
        {
            _container.ClearChildObjects();

            var lastOpenedTierNumber = App.Instance.Player.Dungeon.LastOpenedTier.Number + 1;
            if (lastOpenedTierNumber < MineLocalConfigs.ExplosiveStrikeAbilityOpenedAtTier)
            {
                AddLockedAbilities(3);
                return;
            }

            var firstAbility = Instantiate(_abilityPrefab, _container, false);
            firstAbility.Initialize(App.Instance.Player.Abilities.ExplosiveStrike, OnClickHint);

            if (lastOpenedTierNumber < MineLocalConfigs.FreezingAbilityOpenedAtTier)
            {
                AddLockedAbilities(2);
                return;
            }

            var secondAbility = Instantiate(_abilityPrefab, _container, false);
            secondAbility.Initialize(App.Instance.Player.Abilities.Freezing, OnClickHint);

            if (lastOpenedTierNumber < MineLocalConfigs.AcidAbilityOpenedAtTier)
            {
                AddLockedAbilities(1);
                return;
            }

            var thirdAbility = Instantiate(_abilityPrefab, _container, false);
            thirdAbility.Initialize(App.Instance.Player.Abilities.Acid, OnClickHint);

            
        }

        private void OnClickHint(string text)
        {
            _window.ShowPopup(text);
        }


        private void AddLockedAbilities(int count)
        {
            for (var i = 0; i < count; i++)
                Instantiate(_lockedAbilityPrefab, _container, false);
        }
    }
}