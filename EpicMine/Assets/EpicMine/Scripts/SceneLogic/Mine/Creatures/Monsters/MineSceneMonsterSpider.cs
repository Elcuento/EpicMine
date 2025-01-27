using System;
using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable IdentifierTypo

public class MineSceneMonsterSpider : MineSceneMonster
{
    [SerializeField] private Animator _eyeAnimation;
    [SerializeField] private MonsterSpiderDataReferences _data;

    [SerializeField] private GameObject _abilityParticle;

    private Tweener _acidBuffBlinkingTween;
    private Tweener _moveTween;
    private Tweener _takeDamageTween;

    private int _takeDamageSoundId;

    protected void Start()
    {
        ClearMaterial();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _material.DOKill();
    }

    public override void Initialize(Monster staticMonster, MineSceneHero hero, MineSceneMonsterSection section,
        Action<float> onAttack)
    {
        base.Initialize(staticMonster, hero, section, onAttack);

   //     _meshRender.enabled = false;

        var size = 6;
        transform.localScale = new Vector3(size, size, size);

        _stayDistance = _data.StayDistance + (_meshRender.bounds.size.z - 9.5f * (size / 6f));
        _awayDistance = _stayDistance + 3;
        transform.localPosition = new Vector3(transform.position.x, transform.localPosition.y, _stayDistance);
    }

    protected override IEnumerator AttackState()
    {
        var attackDelay = new WaitForSeconds(MonstersHelper.GetMonsterAttackDelayTime(_staticMonster));

        while (true)
        {
            yield return null;
            yield return attackDelay;
            yield return new WaitUntil(() => !_isActionLock && !IsScared && !IsAway && !IsStunned);

            SetReadyAttack();
        }
    }


    protected override IEnumerator AbilityState(bool immediately = false)
    {
        var usingAbilities = new List<MonsterAbility>();

        var hasSelfCast = false;
        var hasOtherCast = false;

        if (_staticMonster.Abilities == null || _staticMonster.Abilities.Count == 0)
        {
           //  App.Instance.Services.LogService.LogError("no monster ability");
            yield break;
        }

        foreach (var staticMonsterAbility in _staticMonster.Abilities)
        {
            var ability = App.Instance.StaticData.MonstersAbilities.Find(x => x.Id == staticMonsterAbility);
            if (ability == null)
            {
                App.Instance.Services.LogService.Log($"[{staticMonsterAbility}] ability not exist!");
            }
            else
            {
                if (ability.Type == MonsterAbilityType.SelfApply)
                    hasSelfCast = true;
                else if (ability.Type == MonsterAbilityType.OtherApply)
                    hasOtherCast = true;

                usingAbilities.Add(ability);
            }
        }

        while (true)
        {
            MonsterAbility ability = null;

            if (hasSelfCast && (_section.Health / _section.HealthMax <= 0.5f)) //
            {
                if (Random.Range(0, 100) > 30 || hasOtherCast == false) //
                {
                    ability = usingAbilities.FindAll(x => x.Type == MonsterAbilityType.SelfApply)
                        .RandomElement();
                }
                else
                {
                    ability = usingAbilities.FindAll(x => x.Type == MonsterAbilityType.OtherApply)
                        .RandomElement();

                }
            }
            else if (hasOtherCast)
            {
                ability = usingAbilities.FindAll(x => x.Type == MonsterAbilityType.OtherApply)
                    .RandomElement();
            }

            if (ability == null)
            {
                yield return new WaitForSeconds(0.1f);
                SetUnLock();
                App.Instance.Services.LogService.LogError("no monster ability");
                yield break;
            }

            yield return new WaitForSeconds(MonstersHelper.GetMonsterAbilityDelayTime(ability));
            yield return new WaitUntil(() => !_isActionLock && !IsStunned);

            SetReadyAbility(ability, immediately);
        }
    }


    protected override void OnStartAttack()
    {
        AudioManager.Instance.PlaySound(_data.AttackEnd.RandomElement());

        SetAnimationLock();
        _animation.Play($"attack1");
    }

    protected override void OnEndAttack()
    {
        DotWeenHelper.MonsterStrikeShake(Camera.main);
        WindowManager.Instance.Show<WindowVignette>()
            .Initialize(Color.red);

        SetAnimationUnLock();

        AudioManager.Instance.PlaySound(_data.Waiting.RandomElement());

        if (IsAway && !IsScared)
            OnUnScared();
        else if (!IsAway && IsScared)
            OnScared();
    }

    protected override void OnReadyUseAbility()
    {
        _eyeAnimation?.Play("Start");

        if (_castingAbility.Type == MonsterAbilityType.OtherApply)
        {
            _abilityParticle.SetActive(true);
          //  _animation.Play("castAttackStart");
        }
        else
        {
            _abilityParticle.SetActive(true);
           // _animation.Play("castAttackStart");
        }
    }

    protected override void OnStartUseAbility()
    {
        SetAnimationLock();
        AudioManager.Instance.PlaySound(_data.AbilityEnd);
        _animation.Rebind();
        _animation.Play("castAbilityEnd");
        _eyeAnimation?.Play("End");
    }

    protected override void OnEndUseAbility()
    {
        if (_castingAbility.Type == MonsterAbilityType.OtherApply)
        {
            DotWeenHelper.MonsterStrikeShake(Camera.main);
            WindowManager.Instance.Show<WindowVignette>()
                .Initialize(Color.red);

            _abilityParticle.SetActive(false);
        }
        else
        {
            _abilityParticle.SetActive(false);
        }

        AudioManager.Instance.PlaySound(_data.Waiting.RandomElement());
        SetAnimationUnLock();

        if (IsAway && !IsScared)
            OnUnScared();
        else if (!IsAway && IsScared)
            OnScared();
    }


    protected override void OnInterruptAbility()
    {
        SetAnimationUnLock();

        _abilityParticle.SetActive(false);
    }

    protected override void OnInterruptAttack()
    {
        SetAnimationUnLock();
    }

    protected override void OnAppear()
    {
        var isEyeBlink = Random.Range(0, 100) > 50;

        if (isEyeBlink)
            _eyeAnimation?.Play("Start");

        _material.color = Color.black;
        _material.SetColor("_Color", new Color(1, 1, 1, 0));

        _material
            .DOColor(Color.white, 0.5f).OnStart(() =>
            {
                _animation.Rebind();
                _animation.SetTrigger("release");
                AudioManager.Instance.PlaySound(_data.Release);
                _meshRender.enabled = true;
            })
            .SetDelay(0.5f);


        transform.DOLocalMoveZ(_stayDistance, 0.5f)
            .SetDelay(0.5f)
            .OnComplete(() =>
            {
                if (isEyeBlink)
                    _eyeAnimation?.Play("End");

                AudioManager.Instance.PlaySound(_data.Waiting.RandomElement());
                StartCoroutine(AttackState());
                StartCoroutine(AbilityState());
                IsAway = false;

            });

    }

    protected override void OnTakeDamage(float damage, AttackDamageType type, bool isMiss)
    {
        _material
             .DOBlendableColor(new Color(1, 1, 1, 0.3f), "_ColorHit", 0.04f)
             .SetLoops(2, LoopType.Yoyo);

        if (_takeDamageTween == null)
        {
            _takeDamageTween = transform.DOLocalMove(new Vector3(transform.localPosition.x,
                    transform.localPosition.y + 0.1f, transform.localPosition.z + 0.1f), 0.04f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => { _takeDamageTween = null; });
        }


        if (!AudioManager.Instance.IsPlaying(_takeDamageSoundId))
            _takeDamageSoundId = AudioManager.Instance.PlaySound(_data.TakeDamage.RandomElement());


        if (_isAnimationLock || IsStunned)
            return;

        _animation.Play($"damage{Random.Range(1, 3)}");
    }



    protected override void OnStunEnd()
    {
        _material.SetColor("_ColorFrozen", new Color(1, 1, 1, 0));
        _animation.CrossFade("idle", 0.5f);
        _outline.enabled = false;
        _moveTween = null;

        if (IsAway && !IsScared)
            OnUnScared();
        else if (!IsAway && IsScared)
            OnScared();
    }

    protected override void OnScared()
    {
        if (IsStunned)
            return;

        if (_isAnimationLock)
            return;

        if (_moveTween != null)
            return;

        _animation.Play("awayOut");

        _moveTween = transform.DOLocalMoveZ(_awayDistance, 0.2f)
            .SetDelay(0.2f)
            .OnComplete(() =>
            {
                IsAway = true;
                _moveTween = null;


                if (!IsScared)
                {
                    OnUnScared();
                }
            });
    }

    protected override void OnUnScared()
    {
        if (IsStunned)
            return;

        if (_isAnimationLock)
            return;

        if (_moveTween != null)
            return;

        _animation.Play("awayIn");

        _moveTween = transform.DOLocalMoveZ(_stayDistance, 0.2f)
            .SetDelay(0.2f)
            .OnComplete(() =>
            {
                IsAway = false;
                _moveTween = null;

                if (IsScared)
                {
                    OnScared();
                }
            });
    }


    protected override void OnRemoveBuff(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.ExplosiveStrike:
                break;
            case AbilityType.Freezing:
                _material.SetColor("_ColorFrozen", new Color(1, 1, 1, 0));
                break;
            case AbilityType.Acid:
                _acidBuffBlinkingTween?.Kill(true);
                _material.SetColor("_ColorAcid", new Color(1, 1, 1, 0));
                break;
            case AbilityType.Tnt:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

    }

    protected override void OnAddBuff(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.ExplosiveStrike:
                break;
            case AbilityType.Freezing:
                SetInterruptAbility();
                SetInterruptAttack();

                _material.SetColor("_ColorFrozen", new Color(1, 1, 1, 1));
                SetStun(true, 3);
                break;
            case AbilityType.Acid:
                _acidBuffBlinkingTween?.Kill(true);
                _acidBuffBlinkingTween = _material.DOColor(new Color(1, 1, 1, 1), "_ColorAcid", 1)
                    .SetLoops(-1, LoopType.Yoyo);
                break;
            case AbilityType.Tnt:
                break;
        }
    }

    protected override void OnUseOtherAbility(MineSceneMonsterOtherBuff buff)
    {
        Camera.main.DOShakePosition(0.4f, new Vector2(1, 1));
        WindowManager.Instance.Show<WindowVignette>()
            .Initialize(Color.red);
    }

    protected override void OnDeath()
    {
        AudioManager.Instance.PlaySound(_data.Death);
        _animation.Rebind();
        _animation.Play("dead");
        _animation.speed = 1;
    }

    // Animation callbacks

    public void OnGetAttackCallback()
    {
        SetEndAttack();
    }

    public void OnGetAbilityCallback()
    {
        SetEndUseAbility();
    }

}
