using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneSectionExplosiveStrikeBuff : MineSceneSectionBuff
    {
        [SerializeField] private SpriteRenderer[] _fx;

        [SerializeField] private SpriteRenderer _glow;

        [SerializeField] private ParticleSystem _particles;

        private bool _noAfterEffect;

        public override void Initialize(MineSceneSection section, AbilityType type)
        {
            base.Initialize(section, type);

            var torch = App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch;
            
            var wallSection =  _section as MineSceneAttackSection;
            var abilityDamage = App.Instance.Player.Abilities.ExplosiveStrike.StaticLevel.Damage;

            var additionalDamage = 0f;
            var frostDamage = 0f;

            var existBuffToSubtract = _section.Buffs.FirstOrDefault(b => b is MineSceneSectionFreezingBuff);
            if (existBuffToSubtract != null)
            {
                var existFreezingBuffToSubtract = (MineSceneSectionFreezingBuff)existBuffToSubtract;

                var additionalDamagePercent = App.Instance.Player.Abilities.Freezing.StaticLevel.AdditionalParameter;

                additionalDamagePercent += torch.FreezingAdditionalParameter ?? 0;

                additionalDamage = abilityDamage / 100f * additionalDamagePercent * existFreezingBuffToSubtract.Stacks;

                frostDamage = (App.Instance.Player.Abilities.Freezing.StaticLevel.Damage + (torch.FreezingDamage ?? 0)) * (existFreezingBuffToSubtract.Stacks);

                existFreezingBuffToSubtract.SubtractStack();
            }

             additionalDamage += torch.ExplosiveStrikeDamage ?? 0;
      
             wallSection.TakeDamage(abilityDamage + additionalDamage, withSectionBuffsAffect: false, type: AttackDamageType.FireAbility);

            if (frostDamage > 0)
                wallSection.TakeDamage(frostDamage, withSectionBuffsAffect: false, type: AttackDamageType.FrostAbility);
            

            var randomFxIndex = Random.Range(0, _fx.Length);
            PlayFx(_fx[randomFxIndex]);

            var randomInstantSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.InstantDamages.Length);
            var randomInstantSound = App.Instance.ReferencesTables.Sounds.InstantDamages[randomInstantSoundIndex];
            AudioManager.Instance.PlaySound(randomInstantSound);

            if (section is MineSceneMonsterSection)
            {
                _noAfterEffect = true;
                transform.localPosition = new Vector3(0,0,-2.2f);
            }
        }


        private void PlayFx(SpriteRenderer fx)
        {
            fx.DOColor(Color.white, 0)
                .OnComplete(() =>
                {
                    if (_section.IsPassed)
                        fx.DOFade(0, 0.25f);
                    else
                    {
                        if (_noAfterEffect)
                        {
                            fx.DOFade(0, 0.5f);
                        }
                        else
                        {
                            fx.DOColor(Color.black, 0.5f)
                                .OnComplete(() => { fx.DOFade(0, 0.5f); });
                        }
                     
                    }
                });

            _particles.Play();

            _glow.DOFade(0.7f, 0f)
                .OnComplete(() =>
                {
                    if (_section.IsPassed)
                        _glow.DOFade(0, 0.25f);
                    _glow
                        .DOFade(0, 0.5f)
                        .OnComplete(() => { _glow.DOFade(0, 0.5f); });
                });
        }
    }
}