using System;
using System.Collections;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;
// ReSharper disable IdentifierTypo

public class MineSceneMonster : MonoBehaviour
{
    protected Action<float> _onAttack;

    [Header("Main")]
    [SerializeField] protected GameObject _root;
    [SerializeField] protected SkinnedMeshRenderer _meshRender;
    [SerializeField] protected Material _material;
    [SerializeField] protected Animator _animation;
    [SerializeField] protected Outline _outline;

    public bool IsAway;
    public bool IsAlive;
    public bool IsScared;
    public bool IsStunned;

    public MonsterStateType State { get; protected set; }
    public MonsterStatusType Status { get; protected set; }

    protected ISectionMonsterBuffFactory _buffFactory;
    protected Monster _staticMonster;

    protected MineSceneHero _hero;
    protected MineSceneMonsterSection _section;

    [Header("Variables")]
    protected bool _isActionLock;
    public bool _isAnimationLock;
    protected MonsterAbility _castingAbility;

    protected float _stayDistance = 0;
    protected float _awayDistance = 7;

    protected Coroutine _attackCoroutine;
    protected Coroutine _abilityCoroutine;
    protected Coroutine _stunCoroutine;


    #region Initialize

    protected virtual void OnDestroy()
     {
         if (EventManager.Instance == null)
             return;

         EventManager.Instance.Unsubscribe<MineSceneTorchUseEvent>(OnTorchUse);
     }

     protected virtual void Clear()
     {
         EventManager.Instance.Unsubscribe<MineSceneTorchUseEvent>(OnTorchUse);

         _buffFactory = null;
         _outline.enabled = false;

         IsAlive = true;
         IsAway = true;

         _staticMonster = null;
         _section = null;
         _hero = null;
         _onAttack = null;
     }

    public virtual void Initialize(Monster staticMonster, MineSceneHero hero, MineSceneMonsterSection section,
        Action<float> onAttack)
    {
        Clear();

        _staticMonster = staticMonster;
        _hero = hero;
        _section = section;
        _onAttack = onAttack;

        _buffFactory = new DefaultMonsterBuffFactory(_section);

        EventManager.Instance.Subscribe<MineSceneTorchUseEvent>(OnTorchUse);
    }

    public void InitializeTexture(Texture tex)
    {
        _material.mainTexture = tex;
    }

    #endregion

    #region CallBacks

    protected virtual void OnTorchUse(MineSceneTorchUseEvent eventData)
    {
        if (!IsAlive)
            return;

        if(eventData.IsStart)
        SetScared();
        else SetUnScared();
    }

    #endregion
    
    #region TimeFunctions

    #region Main States

    protected virtual IEnumerator IdleState(float delay)
    {
        while (true)
        {
            yield return null;
        }
    }

    protected virtual IEnumerator AttackState()
    {
        while (true)
        {
            yield return null;
        }
    }

    protected virtual IEnumerator AbilityState(bool immediately = false)
    {
        while (true)
        {
            yield return null;
        }
    }

    #endregion

    protected IEnumerator Stun(float time)
    {
        OnStun();
        _animation.speed = 0;
        IsStunned = true;
        yield return new WaitForSeconds(time);
        IsStunned = false;
        _animation.speed = 1;
        SetStunEnd();
    }

    protected IEnumerator AbilityCast(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);
        onEnd?.Invoke();
    }

    protected IEnumerator AttackCast(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);
        onEnd?.Invoke();
    }

    #endregion

    #region OutsideCalls

    public void Damage(float damage, AttackDamageType type, bool isMiss = false)
    {
        SetTakeDamage(damage, type, isMiss);
    }

    public void Heal(int value)
    {
        SetHealing(value);
    }

    public void Scared()
    {
        SetScared();
    }

    public void Death()
    {
        SetDeath();
    }

    public void Appear()
    {
        SetAppear();
    }

    public void RemoveBuff(AbilityType type)
    {
        SetRemoveBuff(type);
    }

    public void AddBuff(AbilityType type)
    {
        SetAddBuff(type);
    }

    #endregion Outside calls

    #region Etc


    public bool IsAnimationState(string state)
    {
        return _animation.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    #endregion

    #region Sets

    protected void SetLock()
    {
        _isActionLock = true;
    }

    protected void SetUnLock()
    {
        _isActionLock = false;
    }

    protected void SetAnimationLock()
    {
        _isAnimationLock = true;
    }

    protected void SetAnimationUnLock()
    {
        _isAnimationLock = false;
    }

    protected void SetInterruptAttack()
    {
        if (State != MonsterStateType.Attack)
            return;

        EventManager.Instance.Publish(new MineSceneSectionEndActionEvent(_section));
        StopCoroutine(_attackCoroutine);
        SetUnLock();
        SetState(MonsterStateType.None);

        OnInterruptAttack();
    }

    protected void SetInterruptAbility()
    {
        if (State != MonsterStateType.Ability)
            return;

        EventManager.Instance.Publish(new MineSceneSectionEndActionEvent(_section));
        StopCoroutine(_abilityCoroutine);
        SetUnLock();
        SetState(MonsterStateType.None);

        OnInterruptAbility();
    }

    protected void SetStun(bool state, float time)
    {
        if (_abilityCoroutine!=null) StopCoroutine(_abilityCoroutine);
        if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
        if (_stunCoroutine != null)  StopCoroutine(_stunCoroutine);

        EventManager.Instance.Publish(new MineSceneSectionEndActionEvent(_section));
        transform.DOKill();
        StartCoroutine(Stun(time));
    }

    protected void SetStunEnd()
    {
        OnStunEnd();   
    }

    //
    protected void SetState(MonsterStateType state)
    {
        State = state;
    }

    protected void SetStatus(MonsterStatusType status)
    {
        Status = status;
    }

    protected void SetRemoveBuff(AbilityType type)
    {
        if (!IsAlive)
            return;

        OnRemoveBuff(type);
    }

    protected void SetAddBuff(AbilityType type)
    {
        if (!IsAlive)
            return;

        OnAddBuff(type);
    }

    protected void SetScared()
    {
        if (!IsAlive)
            return;

        SetInterruptAttack();

        IsScared = true;
        OnScared();
    }

    protected void SetUnScared()
    {
        if (!IsAlive)
            return;

        IsScared = false;
        OnUnScared();
    }

    protected void SetAppear()
    {
        if (!IsAlive)
            return;

        OnAppear();
    }

    protected void SetHealing(int amount)
    {
        if (!IsAlive)
            return;

        OnHeal(amount);
    }

    protected void SetStartAttack()
    {
        if (!IsAlive)
            return;

        if (IsStunned)
            return;

        OnStartAttack();
    }

    protected void SetReadyAttack()
    {
        if (!IsAlive)
            return;

        if (IsStunned)
            return;

        SetLock();
        SetState(MonsterStateType.Attack);

        EventManager.Instance.Publish(new MineSceneSectionStartActionEvent(_section, MonsterActionType.Damage, _staticMonster.AttackTime));

        _attackCoroutine = StartCoroutine(AttackCast(_staticMonster.AttackTime, SetStartAttack));
        OnReadyAttack();
    }

    protected void SetEndAttack()
    {
        EventManager.Instance.Publish(new MineSceneSectionEndActionEvent(_section));

        if (!IsAlive)
            return;

        SetUnLock();
        SetState(MonsterStateType.None);

       // _onAttack?.Invoke(MineSceneDeveloperPanel.GetMonsterStats().MonsterDamage);
          _onAttack?.Invoke(_staticMonster.Damage);

        OnEndAttack();
    }

    protected void SetReadyAbility(MonsterAbility ability, bool immediately)
    {
        if (!IsAlive)
            return;

        if (IsStunned)
            return;

        SetLock();
        _castingAbility = ability;
        SetState(MonsterStateType.Ability);

        var time = immediately ? 0 : ability.UseTime;

        EventManager.Instance.Publish(new MineSceneSectionStartActionEvent(_section,
            ability.Type == MonsterAbilityType.SelfApply ?
                MonsterActionType.AbilityHeal : MonsterActionType.AbilityDamage, time));

        _abilityCoroutine = StartCoroutine(AbilityCast(time, SetStartUseAbility));
        OnReadyUseAbility();
    }

    protected void SetStartUseAbility()
    {
        if (!IsAlive)
            return;

        if (IsStunned)
            return;

        OnStartUseAbility();
    }

    protected void SetEndUseAbility()
    {
        EventManager.Instance.Publish(new MineSceneSectionEndActionEvent(_section));

        if (!IsAlive)
            return;

        SetUnLock();
        SetState(MonsterStateType.None);

        if (_castingAbility == null)
            return;

        if (IsStunned)
            return;

        if (_castingAbility.Type == MonsterAbilityType.OtherApply)
            SetUseOtherAbility(_castingAbility);
        else SetUseSelfAbility(_castingAbility);

       // _castingAbility = null;

        OnEndUseAbility();
    }

    protected void SetTakeDamage(float damage, AttackDamageType type, bool isMiss)
    {
        if (!IsAlive)
            return;

        OnTakeDamage(damage, type, isMiss);
    }


    protected void SetDeath()
    {
        if (!IsAlive)
            return;

        if (IsStunned)
        {
            if(_stunCoroutine!=null)
                StopCoroutine(_stunCoroutine);
        }

        EventManager.Instance.Publish(new MineSceneSectionEndActionEvent(_section));
        StopAllCoroutines();
        transform.DOKill();

        IsAlive = false;

        transform.DOLocalMoveY(-3, 1)
            .SetDelay(1);

        OnDeath();
    }

    protected void SetUseSelfAbility(MonsterAbility ability)
    {
        if (!IsAlive)
            return;

        var buff = _buffFactory.CreateSelfBuff(ability);
        OnUseSelfAbility(buff);
    }

    protected void SetUseOtherAbility(MonsterAbility ability)
    {
        if (!IsAlive)
            return;


        var buff = _buffFactory.CreateOtherBuff(ability, _section.Hero);
        OnUseOtherAbility(buff);
    }

    #endregion

    #region OnSet

    protected virtual void OnInterruptAttack()
    {

    }

    protected virtual void OnInterruptAbility()
    {

    }

    protected virtual void OnScared()
    {

    }

    protected virtual void OnUnScared()
    {

    }


    protected virtual void OnAddBuff(AbilityType type)
    {

    }

    protected virtual void OnRemoveBuff(AbilityType type)
    {

    }

    protected virtual void OnHeal(int amount)
    {

    }

    protected virtual void OnStunEnd()
    {

    }

    protected virtual void OnStun()
    {

    }

    protected virtual void OnDeath()
    {
        
    }

    protected virtual void OnEndUseAbility()
    {

    }

    protected virtual void OnStartUseAbility()
    {

    }

    protected virtual void OnReadyUseAbility()
    {

    }

    protected virtual void OnUseSelfAbility(MineSceneMonsterSelfBuff buff)
    {

    }

    protected virtual void OnUseOtherAbility(MineSceneMonsterOtherBuff buff)
    {

    }

    protected virtual void OnAppear()
    {

    }

    protected virtual void OnReadyAttack()
    {

    }

    protected virtual void OnStartAttack()
    {

    }

    protected virtual void OnEndAttack()
    {

    }

    protected virtual void OnTakeDamage(float damage, AttackDamageType type, bool isMiss)
    {

    }

    #endregion

    #region Finctionality

    protected virtual void ClearMaterial()
    {
        _material.SetColor("_Color", new Color(1, 1, 1, 1));
        _material.SetColor("_ColorHit", new Color(1, 1, 1, 0));
        _material.SetColor("_ColorFrozen", new Color(1, 1, 1, 0));
        _material.SetColor("_ColorAcid", new Color(1, 1, 1, 0));
    }

    #endregion
}
