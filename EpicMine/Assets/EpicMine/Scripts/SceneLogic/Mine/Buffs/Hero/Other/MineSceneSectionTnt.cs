using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

public class MineSceneSectionTnt : MineSceneSectionBuff
{

    [SerializeField] private SpriteRenderer[] _fx;

    [SerializeField] private SpriteRenderer _glow;

    [SerializeField] private ParticleSystem _particles;

    private bool _noAfterEffect;

    public override void Initialize(MineSceneSection section, AbilityType type)
    {
        base.Initialize(section, type);

        _section = section;

        var attackSection = section as MineSceneAttackSection;

        if (attackSection != null)
        {
            var tnt = App.Instance.StaticData.Tnt.FirstOrDefault();

            if (tnt != null)
            {
                var pickaxeDamage = MineHelper.GetCurrentPickaxeDamage();

                attackSection.TakeDamage(pickaxeDamage * tnt.DamagePercent, AttackDamageType.Item);
            }
        }

        var randomFxIndex = Random.Range(0, _fx.Length);
        PlayFx(_fx[randomFxIndex]);

        var randomInstantSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.InstantDamages.Length);
        var randomInstantSound = App.Instance.ReferencesTables.Sounds.InstantDamages[randomInstantSoundIndex];
        AudioManager.Instance.PlaySound(randomInstantSound);


        if (section is MineSceneMonsterSection)
        {
            _noAfterEffect = true;
            transform.localPosition = new Vector3(0, 0, -2.2f);
        }
    }

    private void PlayEnd()
    {
        _section.RemoveBuff(AbilityType.Tnt);
    }

    private void PlayFx(SpriteRenderer fx)
    {
        fx.DOColor(Color.white, 0)
            .OnComplete(() =>
            {
                if (_section.IsPassed)
                {
                    fx.DOFade(0, 0.25f);
                }
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
                    _glow.DOFade(0, 0.25f)
                        .OnComplete(PlayEnd);
                _glow
                    .DOFade(0, 0.5f)
                    .OnComplete(() => { _glow.DOFade(0, 0.5f).OnComplete(PlayEnd); });
            });
    }
}
