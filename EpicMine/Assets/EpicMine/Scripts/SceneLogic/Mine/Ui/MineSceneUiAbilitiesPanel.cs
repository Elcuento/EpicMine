using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiAbilitiesPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _energyBarPanel;
        [SerializeField] private GameObject _energyAbilitiesPanel;

        [Space]
        [SerializeField] private TextMeshProUGUI _energyText;
        [SerializeField] private Image _energyBar;
        [SerializeField] private Image _energyBarFx;

        [Space]
        [SerializeField] private MineSceneUiAbilityButton _explosiveStrike;
        [SerializeField] private MineSceneUiAbilityButton _freezing;
        [SerializeField] private MineSceneUiAbilityButton _acid;


        private void Awake()
        {
            EventManager.Instance.Subscribe<MineSceneEnergyChangeEvent>(OnEnergyChange);
        }

        private void Start()
        {
            _energyBarPanel.SetActive(false);
            _energyAbilitiesPanel.SetActive(false);

            var lastOpenedTierNumber = App.Instance.Player.Dungeon.LastOpenedTier.Number + 1;


            if (lastOpenedTierNumber < MineLocalConfigs.ExplosiveStrikeAbilityOpenedAtTier)
                return;

            _energyBarPanel.SetActive(true);
            _energyAbilitiesPanel.SetActive(true);

            _explosiveStrike.Initialize(App.Instance.Player.Abilities.ExplosiveStrike);

            if (lastOpenedTierNumber < MineLocalConfigs.FreezingAbilityOpenedAtTier)
                return;

            if (!App.Instance.Player.AdditionalInfo.IsSecondEnergyAbilityWindowShowed)
            {
                App.Instance.Player.AdditionalInfo.IsSecondEnergyAbilityWindowShowed = true;

                WindowManager
                    .Instance
                    .Show<WindowNewEnergyAbility>()
                    .Initialize(AbilityType.Freezing);
            }

            _freezing.Initialize(App.Instance.Player.Abilities.Freezing);

            if (lastOpenedTierNumber >= MineLocalConfigs.AcidAbilityOpenedAtTier)
            {
                if (!App.Instance.Player.AdditionalInfo.IsThirdEnergyAbilityWindowShowed)
                {
                    App.Instance.Player.AdditionalInfo.IsThirdEnergyAbilityWindowShowed = true;

                    WindowManager
                        .Instance
                        .Show<WindowNewEnergyAbility>()
                        .Initialize(AbilityType.Acid);
                }

                _acid.Initialize(App.Instance.Player.Abilities.Acid);
            }

        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<MineSceneEnergyChangeEvent>(OnEnergyChange);
        }

        private void OnEnergyChange(MineSceneEnergyChangeEvent eventData)
        {
            _energyText.text = eventData.Energy.ToString();
            _energyBar.DOKill();
            _energyBarFx.DOKill();

            var fillAmount = (float)eventData.Energy / App.Instance.StaticData.Configs.Dungeon.Mines.MaxEnergy;

            if (_energyBar.fillAmount > fillAmount)
            {
                _energyBarFx.DOFillAmount(fillAmount, MineLocalConfigs.PickaxeHealthbarFxTime).SetUpdate(true);
                _energyBar.fillAmount = fillAmount;
            }
            else
            {
                _energyBarFx.fillAmount = fillAmount;
                _energyBar.DOFillAmount(fillAmount, MineLocalConfigs.PickaxeHealthbarFxTime).SetUpdate(true);
            }
        }
    }
}