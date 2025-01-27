using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MinePvpSceneUiWallInfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _wallHealthText;

        [SerializeField] private Slider _wallHealthBar;

        [SerializeField] private Slider _wallHealthBarFx;

        [SerializeField] private Transform _buffItemsContainer;

        [SerializeField] private MineSceneUiBuffIcon _buffItemPrefab;


        private MineSceneSection _section;

        private readonly List<MineSceneUiBuffIcon> _buffItems = new List<MineSceneUiBuffIcon>();


        private void Awake()
        {
            EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Subscribe<MineSceneWallSectionDamageEvent>(OnSectionHealthChange);
            EventManager.Instance.Subscribe<MineSceneWallSectionHealEvent>(OnBossHeal);
            EventManager.Instance.Subscribe<MineSceneSectionBuffsChangeEvent>(OnSectionBuffsChange);

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

            var wallSection = _section as MineScenePvpWallSection;
            if (wallSection != null)
            {
                _wallHealthBar.gameObject.SetActive(true);
                _wallHealthBarFx.gameObject.SetActive(true);

                ChangeWallHealth(wallSection.Health, wallSection.HealthMax, immediately: true);

               // if (!isHardcore)  Check PVP
               //     CheckWallDifficult(wallSection.HealthMax);

                return;
            }

            var chestSection = _section as MineScenePvpChestSection;
            if (chestSection != null)
            {
                _wallHealthBar.gameObject.SetActive(false);
                _wallHealthBarFx.gameObject.SetActive(false);
            }

        }

        private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            _wallHealthBar.gameObject.SetActive(false);
            _wallHealthBarFx.gameObject.SetActive(false);

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