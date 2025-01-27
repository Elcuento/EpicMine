using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MineSceneUiTorchButton : MonoBehaviour
{
    [SerializeField] private MineSceneHero _hero;

    [Space]
    [SerializeField] private Sprite _gray;
    [SerializeField] private Sprite _normal;

    [Space]
    [SerializeField] private RectTransform _button;
    [SerializeField] private TextMeshProUGUI _energyCost;
    [SerializeField] private Image _glow;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _fade;

    [Space]
    [SerializeField] private GameObject _root;

    private bool _isPressed;

    private void Start()
    {
        if (!App.Instance.Player.Features.Exist((CommonDLL.Static.FeaturesType)FeaturesType.TorchAbility))
            return;

        EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
        EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
        EventManager.Instance.Subscribe<MineSceneSectionStartActionEvent>(OnMonsterStartAction);
        EventManager.Instance.Subscribe<MineSceneSectionEndActionEvent>(OnMonsterEndAction);
        EventManager.Instance.Subscribe<MineSceneEnergyChangeEvent>(OnEnergyChange);

        _energyCost.text = MineLocalConfigs.TorchUseMomentCoast.ToString();

        CheckEnergy(_hero.EnergySystem.Value);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
        EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
        EventManager.Instance.Unsubscribe<MineSceneSectionStartActionEvent>(OnMonsterStartAction);
        EventManager.Instance.Unsubscribe<MineSceneSectionEndActionEvent>(OnMonsterEndAction);
        EventManager.Instance.Unsubscribe<MineSceneEnergyChangeEvent>(OnEnergyChange);
    }


    public void OnPress()
    {
        if (_hero.EnergySystem.Value < MineLocalConfigs.TorchUseMomentCoast)
            return;

        _hero.Torch.UseTorchContinuous();
        _button.DOKill();
        _button.DOAnchorPosY(120, 0.2f);
        _isPressed = true;
    }

    public void OnRelease()
    {
        _hero.Torch.UseTorchEnd();
        _button.DOKill();
        _button.DOAnchorPosY(140, 0.2f);
        _isPressed = false;
    }

    private void OnEnergyChange(MineSceneEnergyChangeEvent data)
    {
       CheckEnergy(data.Energy);
    }

    private void CheckEnergy(int energy)
    {
        if ((_isPressed && energy > MineLocalConfigs.TorchUseSecCoast) || (!_isPressed && energy < MineLocalConfigs.TorchUseMomentCoast))
        {
            _icon.sprite = _gray;
            _fade.gameObject.SetActive(true);
        }
        else
        {
            _icon.sprite = _normal;
            _fade.gameObject.SetActive(false);
        }
    }


    private void OnMonsterEndAction(MineSceneSectionEndActionEvent data)
    {
        _glow.DOKill();
        _glow.gameObject.SetActive(false);
        _glow.color = new Color(_glow.color.r, _glow.color.g, _glow.color.b, 1);
    }

    private void OnMonsterStartAction(MineSceneSectionStartActionEvent data)
    {
        if (data.MonsterAction == MonsterActionType.Damage)
        {
            _glow.gameObject.SetActive(true);
            _glow.DOFade(0.5f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
    {
        _root.SetActive(false);
    }

    private void OnSectionReady(MineSceneSectionReadyEvent eventData)
    {
        _root.SetActive(eventData.Section is MineSceneMonsterSection);
    }

}
