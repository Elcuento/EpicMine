using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class UpgradeSkillsAndAbilitiesButtonView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        [SerializeField] private Sprite _canUpgradeSprite;

        [SerializeField] private Sprite _cantUpgradeSprite;

        [SerializeField] private GameObject _redDot;

        [SerializeField] private GameObject _glow;


        private void Start()
        {
            EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnInventoryItemAdd);
            EventManager.Instance.Subscribe<InventoryItemRemoveEvent>(OnInventoryItemRemove);
            EventManager.Instance.Subscribe<CurrencyAddEvent>(OnCurrencyAdd);
            EventManager.Instance.Subscribe<CurrencyChangeEvent>(OnCurrencyChange);

            App.Instance.Controllers.RedDotsController.OnSkillsAndAbilitiesChange += OnSkillsAndAbilitiesChange;


            CheckUpdate();
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnInventoryItemAdd);
                EventManager.Instance.Unsubscribe<InventoryItemRemoveEvent>(OnInventoryItemRemove);
                EventManager.Instance.Unsubscribe<CurrencyAddEvent>(OnCurrencyAdd);
                EventManager.Instance.Unsubscribe<CurrencyChangeEvent>(OnCurrencyChange);
            }

            if (App.Instance != null)
            {
                App.Instance.Controllers.RedDotsController.OnSkillsAndAbilitiesChange -= OnSkillsAndAbilitiesChange;
   
            }
        }


        private void OnInventoryItemAdd(InventoryItemAddEvent eventData)
        {
            CheckUpdate();
        }

        private void OnInventoryItemRemove(InventoryItemRemoveEvent eventData)
        {
            CheckUpdate();
        }

        private void OnCurrencyChange(CurrencyChangeEvent eventData)
        {
            CheckUpdate();
        }

        private void OnCurrencyAdd(CurrencyAddEvent eventData)
        {
            CheckUpdate();
        }

        private void CheckUpdate()
        {
            var canUpgrade = CanUpgrade();
     
            _icon.sprite = canUpgrade ? _canUpgradeSprite : _cantUpgradeSprite;
            var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number + 1;

            if (!canUpgrade)
            {
                App.Instance.Controllers.RedDotsController.SetSkillsUpgradeState(new Dictionary<SkillType, bool>
                {
                    { SkillType.Damage, false },
                    { SkillType.Crit, false },
                    { SkillType.Fortune, false }
                });
                App.Instance.Controllers.RedDotsController.SetAbilitiesUpgradeState(new Dictionary<AbilityType, bool>
                {
                    { AbilityType.ExplosiveStrike, false },
                    { AbilityType.Freezing, false },
                    { AbilityType.Acid, false }
                });
                return;
            }
            else
            {
                App.Instance.Controllers.RedDotsController.SetSkillsUpgradeState(new Dictionary<SkillType, bool>()
                {
                    { SkillType.Damage, App.Instance.Player.Skills.Damage.CanUpgrade },
                    { SkillType.Crit, App.Instance.Player.Skills.Crit.CanUpgrade },
                    { SkillType.Fortune, App.Instance.Player.Skills.Fortune.CanUpgrade }
                });
                App.Instance.Controllers.RedDotsController.SetAbilitiesUpgradeState(new Dictionary<AbilityType, bool>
                {
                    { AbilityType.ExplosiveStrike, App.Instance.Player.Abilities.ExplosiveStrike.CanUpgrade
                                                   && tier >= MineLocalConfigs.ExplosiveStrikeAbilityOpenedAtTier },

                    { AbilityType.Freezing, App.Instance.Player.Abilities.Freezing.CanUpgrade
                                            && tier >= MineLocalConfigs.FreezingAbilityOpenedAtTier },

                    { AbilityType.Acid, App.Instance.Player.Abilities.Acid.CanUpgrade
                                        && tier >= MineLocalConfigs.AcidAbilityOpenedAtTier }
                });
            }

            _redDot.SetActive( !App.Instance.Controllers.RedDotsController.SkillAbilitiesAble.Viewed
                               && App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier));

            _glow.SetActive(_redDot.activeSelf);
        }

     

        private void OnSkillsAndAbilitiesChange(RedDotSimple dot)
        {
            var canUpgrade = CanUpgrade();
      
            _icon.sprite = canUpgrade ? _canUpgradeSprite : _cantUpgradeSprite;

            _redDot.SetActive(canUpgrade 
                                  && !dot.Viewed
                                  && App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier));
            _glow.SetActive(_redDot.activeSelf);
        }    

        private bool CanUpgrade()
        {
            var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number + 1;

            if (App.Instance.Player.Skills.Damage.CanUpgrade)
                return true;

            if (App.Instance.Player.Skills.Crit.CanUpgrade)
                return true;

            if (App.Instance.Player.Skills.Fortune.CanUpgrade)
                return true;

            if (App.Instance.Player.Abilities.ExplosiveStrike.CanUpgrade
                && tier >= MineLocalConfigs.ExplosiveStrikeAbilityOpenedAtTier)
                return true;

            if (App.Instance.Player.Abilities.Freezing.CanUpgrade
                && tier >= MineLocalConfigs.FreezingAbilityOpenedAtTier)
                return true;

                return App.Instance.Player.Abilities.Acid.CanUpgrade
                && tier >= MineLocalConfigs.AcidAbilityOpenedAtTier;
        }
    }
}