using System;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiDamageInscriptionItem : MonoBehaviour
    {

        private Action<MineSceneUiDamageInscriptionItem> _onEnd;


        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private Color _defaultColor;

        [SerializeField] private Color _missColor;

        private const int _defaultSize = 120;

        private const int _critSize = 180;

        private const float _fadeDuration = 1f;

        private const float _critFadeDuration = 2f;

        private const float _moveDistance = 100f;


        private void Clear()
        {
            transform.DOKill();
            _text.text = "";
        }

        private void OnEnd()
        {
            Clear();
            _onEnd?.Invoke(this);
        }

        public void Initialize(MineSceneWallSectionDamageEvent eventData, Vector3 startPosition, Action<MineSceneUiDamageInscriptionItem> onEnd)
        {
            _onEnd = onEnd;

            _text.text = eventData.IsImmunity ? LocalizationHelper.GetLocale("Immunity") : eventData.Damage.ToString("F0");
            _text.fontSize = eventData.IsCritical ? _critSize : _defaultSize;

            switch (eventData.Source)
            {
                case AttackDamageType.Pickaxe:
                    _text.color = _defaultColor;
                    break;
                case AttackDamageType.Ability:
                    _text.color = Color.black;
                    break;
                case AttackDamageType.FireAbility:
                    _text.color = AbilitiesHelper.GetTextAbilityColor(AbilityType.ExplosiveStrike);
                    break;
                case AttackDamageType.FrostAbility:
                    _text.color = AbilitiesHelper.GetTextAbilityColor(AbilityType.Freezing);
                    break;
                case AttackDamageType.AcidAbility:
                    _text.color = AbilitiesHelper.GetTextAbilityColor(AbilityType.Acid);
                    break;
                case AttackDamageType.Item:
                    _text.color = App.Instance.ReferencesTables.Colors.DefaultTextColor;
                    break;
            }

            _rectTransform.anchoredPosition = startPosition;

            _text.DOFade(0f, eventData.IsCritical ? _critFadeDuration : _fadeDuration).SetUpdate(isIndependentUpdate: true);
            _rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + _moveDistance, eventData.IsCritical ? _critFadeDuration : _fadeDuration)
                .SetUpdate(isIndependentUpdate: true)
                .OnComplete(OnEnd);
        }
    }
}