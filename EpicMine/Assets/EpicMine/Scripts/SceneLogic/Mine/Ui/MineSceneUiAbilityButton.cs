using System;
using System.Collections;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AbilityLevel = BlackTemple.EpicMine.Core.AbilityLevel;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiAbilityButton : MonoBehaviour
    {
        [SerializeField] private MineSceneHero _hero;

        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private GameObject _button;

        [SerializeField] private GameObject _activeBackground;

        [SerializeField] private Image _icon;

        [SerializeField] private Sprite _activeIcon;

        [SerializeField] private Sprite _inactiveIcon;

        [SerializeField] private TextMeshProUGUI _energyCostText;

        [SerializeField] private GameObject _cooldownBackground;

        [SerializeField] private Image _cooldownFill;

        [SerializeField] private TextMeshProUGUI _cooldownText;

        [SerializeField] private Animator _cooldownAnimator;

        [SerializeField] private Image _glow;

        public AbilityLevel AbilityLevel { get; private set; }

        private float _cooldown;

        private float _timeEndCooldown;




        public void Initialize(AbilityLevel abilityLevel)
        {
            AbilityLevel = abilityLevel;

            _canvasGroup.alpha = 1f;
            _button.SetActive(true);
            _energyCostText.text = abilityLevel.StaticLevel.EnergyCost.ToString();

            UpdateView();
        }

        public void OnDisable()
        {
            if (_cooldown > 0)
                _timeEndCooldown = Time.unscaledTime + _cooldown;
        }

        public void OnEnable()
        {
            _cooldownAnimator.Rebind();

            if (_cooldown > 0)
            {
                _cooldown = _timeEndCooldown - Time.unscaledTime + _cooldown;
                if (_cooldown < 0)
                    _cooldown = 0;

                StartCoroutine(Cooldown());

                _timeEndCooldown = 0;
            }
        }

        public void Use()
        {
            if (_cooldown > 0)
                return;

            if (_hero.UseAbility(AbilityLevel.Type))
            {
                var extraCoolDown = 0f;
                var torch = App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch;
                switch (AbilityLevel.Type)
                {
                    case AbilityType.ExplosiveStrike:
                        if (torch.ExplosiveStrikeCooldown != null)
                            extraCoolDown = torch.ExplosiveStrikeCooldown.Value;
                        break;
                    case AbilityType.Freezing:
                        if (torch.FreezingCooldown != null)
                            extraCoolDown = torch.FreezingCooldown.Value;
                        break;
                    case AbilityType.Acid:
                        if (torch.AcidCooldown != null)
                            extraCoolDown = torch.AcidCooldown.Value;
                        break;
                }


                _cooldown = AbilityLevel.StaticLevel.Cooldown - extraCoolDown;
                _timeEndCooldown = Time.unscaledTime + _cooldown;

                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(Cooldown());
                }
 
            }
        }


        private void Awake()
        {
            _button.SetActive(false);
            _canvasGroup.alpha = 0.5f;

            EventManager.Instance.Subscribe<MineSceneEnergyChangeEvent>(OnEnergyChange);
            EventManager.Instance.Subscribe<MineSceneSectionStartActionEvent>(OnMonsterStartAction);
            EventManager.Instance.Subscribe<MineSceneSectionEndActionEvent>(OnMonsterEndAction);
        }


        private void OnMonsterStartAction(MineSceneSectionStartActionEvent data)
        {
            if (AbilityLevel == null)
                return;

            if (AbilityLevel.Type != AbilityType.Freezing)
                return;

          //  if (_abilityLevel.StaticLevel.EnergyCost > _hero.EnergySystem.Value)
          //      return;

                _glow.gameObject.SetActive(true);
                _glow.DOFade(0.5f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnMonsterEndAction(MineSceneSectionEndActionEvent data)
        {
            if (AbilityLevel == null)
                return;

            if (AbilityLevel.Type != AbilityType.Freezing)
                return;

            _glow.DOKill();
            _glow.gameObject.SetActive(false);
            _glow.color = new Color(_glow.color.r, _glow.color.g, _glow.color.b, 1);
        }

        private IEnumerator Cooldown()
        {
            _cooldownBackground.SetActive(true);
            _cooldownFill.DOKill();
            _cooldownFill.DOFillAmount(1, 0);
            _cooldownFill.DOFillAmount(0, _cooldown)
                .SetEase(Ease.Linear);

            while (_cooldown > 0)
            {
                var color = _cooldown > 3
                    ? "b6b6b6"
                    : "ff0000";

                _cooldownText.text = $"<color=#{color}>{_cooldown:F0}</color>";

                yield return new WaitForSeconds(1);
                _cooldown--;
            }

            _cooldownBackground.SetActive(false);
            _cooldownText.text = string.Empty;
            _cooldownFill.fillAmount = 0f;
            _cooldownAnimator.Play("Ready");
        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

              EventManager.Instance.Unsubscribe<MineSceneEnergyChangeEvent>(OnEnergyChange);
              EventManager.Instance.Unsubscribe<MineSceneSectionStartActionEvent>(OnMonsterStartAction);
              EventManager.Instance.Unsubscribe<MineSceneSectionEndActionEvent>(OnMonsterEndAction);
        }

        private void OnEnergyChange(MineSceneEnergyChangeEvent eventData)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (AbilityLevel == null)
                return;

            var isAvailable = _hero.EnergySystem.Value >= AbilityLevel.StaticLevel.EnergyCost;
            _activeBackground.SetActive(isAvailable);
            _icon.sprite = isAvailable ? _activeIcon : _inactiveIcon;
        }
    }
}