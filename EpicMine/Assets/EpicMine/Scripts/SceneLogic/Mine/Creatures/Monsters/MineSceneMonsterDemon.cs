using System;
using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MineSceneMonsterDemon : MineSceneMonster
{
    [SerializeField] private Animator _eyeAnimation;
    [SerializeField] private MonsterDemonDataReferences _data;

    [SerializeField] private GameObject _attackAbilityParticle;
    [SerializeField] private GameObject _healAbilityParticle;

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

        _meshRender.enabled = false;

        var size = 6;
        transform.localScale = new Vector3(size, size, size);

        _stayDistance = (_meshRender.bounds.size.z - 9.5f * (size / 6f));
        _awayDistance = _stayDistance + 3;
        transform.localPosition = new Vector3(transform.localPosition.x, _stayDistance);
    }

    protected override IEnumerator AttackState()
    {
        var delay = new WaitForSeconds(MonstersHelper.GetMonsterAttackDelayTime(_staticMonster));

        while (true)
        {
            yield return null;
            yield return delay;
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
            App.Instance.Services.LogService.LogError("no monster ability");
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
        _animation.Play($"attack{Random.Range(1,3)}");
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
            _attackAbilityParticle.SetActive(true);
            _animation.Play("castAttackStart");
            AudioManager.Instance.PlaySound(_data.AbilityStart);
            AudioManager.Instance.PlaySound(_data.AbilityProgress);
        }
        else
        {
            _healAbilityParticle.SetActive(true);
            _animation.Play("castHealStart");
            AudioManager.Instance.PlaySound(_data.Ability2Start);
            AudioManager.Instance.PlaySound(_data.Ability2Progress);
        }
    }

    protected override void OnStartUseAbility()
    {
        SetAnimationLock();
        _animation.Rebind();

        if (_castingAbility.Type == MonsterAbilityType.OtherApply)
        {
            AudioManager.Instance.PlaySound(_data.AbilityEnd );
            _animation.Play("castAttackEnd");
        }
        else
        {
            AudioManager.Instance.PlaySound(_data.Ability2End);
            _animation.Play("castHealEnd");
        }

        _eyeAnimation?.Play("End");
    }

    protected override void OnEndUseAbility()
    {
        if (_castingAbility.Type == MonsterAbilityType.OtherApply)
        {
            DotWeenHelper.MonsterStrikeShake(Camera.main);
            WindowManager.Instance.Show<WindowVignette>()
                .Initialize(Color.red);

            _attackAbilityParticle.SetActive(false);
        }
        else
        {
            _healAbilityParticle.SetActive(false);
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

        _attackAbilityParticle.SetActive(false);
        _healAbilityParticle.SetActive(false);
    }

    protected override void OnInterruptAttack()
    {
        SetAnimationUnLock();
       // _animation.Play("idle");
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

        var damageType = Random.Range(0, 3);

        if (!AudioManager.Instance.IsPlaying(_takeDamageSoundId))
            _takeDamageSoundId = AudioManager.Instance.PlaySound(_data.TakeDamage.RandomElement());


        if (_isAnimationLock || IsStunned)
            return;

        _animation.Play("damage" + (damageType + 1));
        
        CancelInvoke("OnHitAnimationEnd");
        Invoke("OnHitAnimationEnd",0.5f);
    }

    private void OnHitAnimationEnd()
    {
        if (!IsAlive)
            return;

        if (_isAnimationLock)
            return;

        if (State == MonsterStateType.Ability)
            _animation.Play(_castingAbility.Type == MonsterAbilityType.OtherApply ? "castAttackStart" : "castHealStart");
    }

    protected override void OnStunEnd()
    {
        _material.SetColor("_ColorFrozen", new Color(1, 1, 1, 0));
        _animation.CrossFade("idle",0.5f);
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
        _takeDamageSoundId = AudioManager.Instance.PlaySound(_data.JumpAway);

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
                else
                {
                    if (State == MonsterStateType.Ability)
                    {
                        if (_isAnimationLock)
                            return;

                        _animation.Play(_castingAbility.Type == MonsterAbilityType.OtherApply ? "castAttackStart" : "castHealStart");
                    }
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
        _takeDamageSoundId = AudioManager.Instance.PlaySound(_data.JumpAway);

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
                else
                {
                    if (_isAnimationLock)
                        return;

                    if (State == MonsterStateType.Ability)
                    {
                        _animation.Play(_castingAbility.Type == MonsterAbilityType.OtherApply ? "castAttackStart" : "castHealStart");
                    }
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
                _material.SetColor("_ColorFrozen", new Color(1,1,1,0));
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
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
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

    public void OnGetAbility1Callback()
    {
        SetEndUseAbility();
    }

    public void OnGetAbility2Callback()
    {
        SetEndUseAbility();
    }
}
