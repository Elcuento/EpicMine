using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiWallInfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _wallName;

        [SerializeField] private TextMeshProUGUI _wallNumberText;

        [SerializeField] private TextMeshProUGUI _wallHealthText;

        [SerializeField] private Slider _wallHealthBar;

        [SerializeField] private Slider _wallHealthBarFx;

        [SerializeField] private GameObject _hintButton;

        [SerializeField] private GameObject _hintFirstIcon;

        [SerializeField] private GameObject _hintSecondIcon;

        [SerializeField] private GameObject _hintBubble;

        [SerializeField] private TextMeshProUGUI _hintBubbleText;

        [SerializeField] private Transform _buffItemsContainer;

        [SerializeField] private MineSceneUiBuffIcon _buffItemPrefab;

        private const string DifficultHintShowedPrefsKey = "mine_difficult_hint_showed";

        private bool _isDifficultHintShowed;

        private Core.Tier _tier;

        private Core.Mine _mine;

        private MineSceneSection _section;

        private readonly List<MineSceneUiBuffIcon> _buffItems = new List<MineSceneUiBuffIcon>();


        public void WallHealthHintClick()
        {
            _hintBubble.SetActive(true);
        }

        public void WallHealthHintCloseClick()
        {
            _hintBubble.SetActive(false);
        }


        private void Awake()
        {
            EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Subscribe<MineSceneWallSectionDamageEvent>(OnSectionHealthChange);
            EventManager.Instance.Subscribe<MineSceneWallSectionHealEvent>(OnBossHeal);
            EventManager.Instance.Subscribe<MineSceneSectionBuffsChangeEvent>(OnSectionBuffsChange);

            _isDifficultHintShowed = PlayerPrefs.GetInt(DifficultHintShowedPrefsKey, 0) == 1;

            _tier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            _mine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
                EventManager.Instance.Unsubscribe<MineSceneWallSectionDamageEvent>(OnSectionHealthChange);
                EventManager.Instance.Unsubscribe<MineSceneWallSectionHealEvent>(OnBossHeal);
                EventManager.Instance.Unsubscribe<MineSceneSectionBuffsChangeEvent>(OnSectionBuffsChange);
            }
        }


        private void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            _section = eventData.Section;
            UpdateBuffs();

            var wallSection = _section as MineSceneWallSection;
            if (wallSection != null)
            {
                _wallHealthBar.gameObject.SetActive(true);
                _wallHealthBarFx.gameObject.SetActive(true);

                var isHardcore = App.Instance
                    .Services
                    .RuntimeStorage
                    .Load<bool>(RuntimeStorageKeys.IsHardcoreMode);

                if (_mine.IsLast)
                    _wallName.text = LocalizationHelper.GetLocale($"tier_{ _tier.Number }_boss");
                else
                {
                    var dropItemLocale = LocalizationHelper.GetLocale(wallSection.ItemId);
                    var coloredResource = MineHelper.GetColoredResource(wallSection.ItemId, _tier);
                    _wallName.text = $"<color={coloredResource.Color}>{dropItemLocale}</color>";
                    _wallNumberText.gameObject.SetActive(true); //


                    if (_mine.IsComplete)
                        _wallNumberText.text = string.Format(LocalizationHelper.GetLocale("mine_wall_number_simple"), wallSection.Number + 1);
                    else
                    {
                        int sectionsCount;
                        if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting))
                        {
                            sectionsCount = isHardcore
                                ? RatingHelper.GetSectionsCountForHardcoreRating(1)
                                : MineLocalConfigs.DefaultSectionsCount;
                        }
                        else
                            sectionsCount = MineLocalConfigs.BlacksmithSectionPositionNumber + _mine.Number;

                        var locale = LocalizationHelper.GetLocale("mine_wall_number");
                        _wallNumberText.text = string.Format(locale, wallSection.Number + 1, sectionsCount);
                    }
                }

                ChangeWallHealth(wallSection.Health, wallSection.HealthMax, immediately: true);

                if (!isHardcore)
                    CheckWallDifficult(wallSection.HealthMax);

                return;
            }

            var monsterSection = _section as MineSceneMonsterSection;
            if (monsterSection != null)
            {
                _wallHealthBar.gameObject.SetActive(true);
                _wallHealthBarFx.gameObject.SetActive(true);
                
                _wallName.text = LocalizationHelper.GetLocale(monsterSection.ItemId);

                ChangeWallHealth(monsterSection.Health, monsterSection.HealthMax, immediately: true);
            }


            var chestSection = _section as MineSceneChestSection;
            if (chestSection != null)
            {
                _wallHealthBar.gameObject.SetActive(false);
                _wallHealthBarFx.gameObject.SetActive(false);
                var localeKey = chestSection.ChestType == ChestType.Simple ? "simple_chest" : "royal_chest";
                _wallName.text = LocalizationHelper.GetLocale(localeKey);
            }

            var enchantedChestSection = _section as MineSceneEnchantedChestSection;
            if (enchantedChestSection != null)
            {
                _wallHealthBar.gameObject.SetActive(false);
                _wallHealthBarFx.gameObject.SetActive(false);
                _wallName.text = LocalizationHelper.GetLocale("enchanted_chest");
            }

            var godSection = _section as MineSceneGodSection;
            if (godSection != null)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            _wallHealthBar.gameObject.SetActive(false);
            _wallHealthBarFx.gameObject.SetActive(false);
            _wallNumberText.gameObject.SetActive(false);
            _wallName.text = string.Empty;
            _wallName.color = Color.white;

            _hintButton.SetActive(false);
            _hintBubble.SetActive(false);
            ClearBuffIcons();
        }

        private void OnSectionHealthChange(MineSceneWallSectionDamageEvent eventData)
        {
            ChangeWallHealth(eventData.HealthHandler.Health, eventData.HealthHandler.HealthMax);
        }

        private void OnBossHeal(MineSceneWallSectionHealEvent eventData)
        {
            ChangeWallHealth(eventData.Section.Health, eventData.Section.HealthMax);
        }

        private void OnSectionBuffsChange(MineSceneSectionBuffsChangeEvent data)
        {
            UpdateBuffs();
        }


        private void ChangeWallHealth(float health, float maxHealth, bool immediately = false)
        {
            _wallHealthBar.DOKill();
            _wallHealthBarFx.DOKill();

            var newHealth = health / maxHealth;
            if (_wallHealthBar.value < newHealth)
            {
                _wallHealthBar.DOValue(newHealth, immediately ? 0 : MineLocalConfigs.WallHealthbarFxTime).SetUpdate(true);
                _wallHealthBarFx.DOValue(newHealth, immediately ? 0 : MineLocalConfigs.WallHealthbarFxTime).SetUpdate(true);
            }
            else
            {
                _wallHealthBar.value = newHealth;
                _wallHealthBarFx.DOValue(newHealth, immediately ? 0 : MineLocalConfigs.WallHealthbarFxTime).SetUpdate(true);
            }

            _wallHealthText.text = $"{Mathf.CeilToInt(health)}/{Mathf.CeilToInt(maxHealth)}";
        }

        private void CheckWallDifficult(float wallHealth)
        {
            _hintFirstIcon.SetActive(false);
            _hintSecondIcon.SetActive(false);

            var isFirstDifficult = false;
            var isSecondDifficult = false;
            var difficultValue = App.Instance.Player.Skills.Damage.Value + App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Damage;

            if (_mine.IsLast)
            {
                if (wallHealth > difficultValue * MineLocalConfigs.MineDifficultWallBossHintSecondCoefficient)
                    isSecondDifficult = true;
                else if (wallHealth > difficultValue * MineLocalConfigs.MineDifficultWallBossHintFirstCoefficient)
                    isFirstDifficult = true;
            }
            else
            {
                if (wallHealth > difficultValue * MineLocalConfigs.MineDifficultWallHintSecondCoefficient)
                    isSecondDifficult = true;
                else if (wallHealth > difficultValue * MineLocalConfigs.MineDifficultWallHintFirstCoefficient)
                    isFirstDifficult = true;
            }

            if (isSecondDifficult)
            {
                _hintBubbleText.text = LocalizationHelper.GetLocale("mine_wall_health_hint_2");
                _hintSecondIcon.SetActive(true);
            }
            else if (isFirstDifficult)
            {
                _hintBubbleText.text = LocalizationHelper.GetLocale("mine_wall_health_hint_1");
                _hintFirstIcon.SetActive(true);
            }

            if (isFirstDifficult || isSecondDifficult)
            {
                _hintButton.SetActive(true);
                if (!_isDifficultHintShowed)
                {
                    _isDifficultHintShowed = true;
                    PlayerPrefs.SetInt(DifficultHintShowedPrefsKey, 1);

                    _hintBubble.SetActive(true);
                }
            }
        }

        private void UpdateBuffs()
        {
            foreach (var buffItem in _buffItems.ToList())
            {
                if (!_section.Buffs.Contains(buffItem.Buff))
                {
                    _buffItems.Remove(buffItem);
                    Destroy(buffItem.gameObject);
                }
            }

            foreach (var buff in _section.Buffs)
            {
                var existBuffItem = _buffItems.FirstOrDefault(b => b.Buff == buff);
                if (existBuffItem != null)
                    existBuffItem.Initialize(buff);
                else
                {
                    var buffItem = Instantiate(_buffItemPrefab, _buffItemsContainer, false);
                    buffItem.Initialize(buff);
                    _buffItems.Add(buffItem);
                }
            }
        }

        private void ClearBuffIcons()
        {
            _buffItemsContainer.ClearChildObjects();
            _buffItems.Clear();
        }
    }
}