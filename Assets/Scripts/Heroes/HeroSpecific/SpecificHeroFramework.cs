using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains the basic functionality that all heroes will use
/// </summary>
public abstract class SpecificHeroFramework : MonoBehaviour
{
    [SerializeField] protected float _basicAbilityChargeTime;

    protected float _basicAbilityCurrentCharge = 0;

    [Space]

    [SerializeField] protected float _manualAbilityChargeTime;
    protected float _manualAbilityCurrentCharge = 0;

    [Space]
    [Header("Animations")]
    [SerializeField] protected Animator _heroSpecificAnimator;

    [SerializeField] protected bool _hasBasicAbilityAnimation;
    [SerializeField] protected bool _hasManualAbilityAnimation;
    [SerializeField] protected bool _hasPassiveAbilityAnimation;

    internal HeroBase _myHeroBase;

    protected Coroutine _attemptingBasicAbilitiesCoroutine;
    protected Coroutine _basicAbilityCooldownCoroutine;
    protected Coroutine _manualAbilityCooldownCoroutine;

    protected UnityEvent<float> _heroDealtDamageEvent = new UnityEvent<float>();
    protected UnityEvent<float> _heroDealtStaggerEvent = new UnityEvent<float>();

    #region Basic Abilities
    protected virtual void StartCooldownBasicAbility()
    {
        _basicAbilityCooldownCoroutine = StartCoroutine(CooldownBasicAbility());
    }

    protected virtual void StopCooldownBasicAbility()
    {
        StopCoroutine(_basicAbilityCooldownCoroutine);
        _basicAbilityCooldownCoroutine = null;
    }

    protected virtual IEnumerator CooldownBasicAbility()
    {
        _basicAbilityCurrentCharge = 0;
        while (_basicAbilityCurrentCharge < _basicAbilityChargeTime)
        {
            //AddToBasicAbilityChargeTime(Time.deltaTime);
            CooldownAddToBasicAbilityCharge(Time.deltaTime);
            yield return null;
        }

        BasicAbilityCooldownReady();
    }

    protected virtual void CooldownAddToBasicAbilityCharge(float addedAmount)
    {
        AddToBasicAbilityChargeTime(addedAmount);
    }

    protected virtual void BasicAbilityCooldownReady()
    {
        _basicAbilityCooldownCoroutine = null;
        StartCheckingToAttemptBasicAbilities();
    }

    public virtual void AddToBasicAbilityChargeTime(float addedAmount)
    {
        _basicAbilityCurrentCharge += addedAmount;
    }

    public virtual void StartCheckingToAttemptBasicAbilities()
    {
        _attemptingBasicAbilitiesCoroutine = StartCoroutine(CheckingToAttemptBasicAbilities());
    }
    public virtual IEnumerator CheckingToAttemptBasicAbilities()
    {
        while (!ConditionsToActivateBasicAbilities())
            yield return new WaitForFixedUpdate();
        ActivateBasicAbilities();
    }

    public virtual bool ConditionsToActivateBasicAbilities()
    {
        return !_myHeroBase.GetPathfinding().IsHeroMoving();
    }

    protected virtual void TriggerBasicAbilityAnimation()
    {
        if (!_hasBasicAbilityAnimation) return;

        _myHeroBase.GetHeroVisuals().TriggerBasicAbilityAnimation();
    }

    public virtual void ActivateBasicAbilities()
    {
        TriggerBasicAbilityAnimation();

        StartCooldownBasicAbility();
    }

    #endregion

    #region Manual Abilities
    

    protected virtual void StartCooldownManualAbility()
    {
        _manualAbilityCooldownCoroutine = StartCoroutine(CooldownManualAbility());
    }

    protected virtual void StopCooldownManualAbility()
    {
        StopCoroutine(_manualAbilityCooldownCoroutine);
        _manualAbilityCooldownCoroutine = null;
    }

    public virtual IEnumerator CooldownManualAbility()
    {
        while (_manualAbilityCurrentCharge < _manualAbilityChargeTime)
        {
            AddToManualAbilityChargeTime(Time.deltaTime);
            yield return null;
        }

        _myHeroBase.InvokeHeroManualAbilityFullyChargedEvent();
    }

    public virtual void AddToManualAbilityChargeTime(float addedAmount)
    {
        _manualAbilityCurrentCharge += addedAmount;
        _myHeroBase.InvokeHeroManualAbilityChargingEvent();
    }

    public virtual void AttemptActivationOfManualAbility(Vector3 activateLocation)
    {
        if(_manualAbilityCurrentCharge >= _manualAbilityChargeTime)
        {
            ActivateManualAbilities(activateLocation);
        }
    }

    protected virtual void TriggerManualAbilityAnimation()
    {
        if (!_hasManualAbilityAnimation) return;

        _myHeroBase.GetHeroVisuals().TriggerManualAbilityAnimation();
    }

    public virtual void ActivateManualAbilities(Vector3 attackLocation)
    {
        _manualAbilityCurrentCharge = 0;

        _manualAbilityCooldownCoroutine = null;

        TriggerManualAbilityAnimation();

        StartCooldownManualAbility();
    }
    #endregion

    #region Passive Abilities

    protected virtual void TriggerPassiveAbilityAnimation()
    {
        if (!_hasPassiveAbilityAnimation) return;

        _myHeroBase.GetHeroVisuals().TriggerPassiveAbilityAnimation();
    }

    public virtual void ActivatePassiveAbilities()
    {
        TriggerPassiveAbilityAnimation();
    }

    #endregion

    #region GeneralAbilityFunctionality
    protected virtual bool InAttackRangeOfBoss(float attackRange)
    {
        Vector2 heroPos = new Vector2(_myHeroBase.gameObject.transform.position.x, 
            _myHeroBase.gameObject.transform.position.z);
        Vector2 bossPos = new Vector2(GameplayManagers.Instance.GetBossManager().GetBossBase().transform.position.x,
            GameplayManagers.Instance.GetBossManager().GetBossBase().transform.position.z);

        return Vector2.Distance(heroPos, bossPos) < attackRange;
    }

    public virtual void DamageBoss(float damage)
    {
        damage *= _myHeroBase.GetHeroStats().GetCurrentDamageMultiplier();

        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossStats().DealDamageToBoss(damage);

        InvokeHeroDealtDamageEvent(damage);
    }

    public virtual void StaggerBoss(float stagger)
    {
        stagger *= _myHeroBase.GetHeroStats().GetCurrentStaggerMultiplier();

        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossStats().DealStaggerToBoss(stagger);

        InvokeHeroDealtStaggerEvent(stagger);
    }

    public virtual void HeroTakeDamageOverride(float damage)
    {

    }


    #endregion

    public virtual void SetupSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        _myHeroBase = heroBase;
        SetDefaultValues(heroSO);
        SubscribeToEvents();
    }

    protected virtual void BattleStarted()
    {
        ActivateHeroSpecificActivity();
    }

    protected virtual void HeroDied()
    {

    }

    public virtual void ActivateHeroSpecificActivity()
    {
        StartCooldownBasicAbility();
        StartCooldownManualAbility();
    }
    public virtual void DeactivateHeroSpecificActivity()
    {
        StopCooldownBasicAbility();
        StopCooldownManualAbility();
    }

    public void SetDefaultValues(HeroSO heroSO)
    {

    }

    protected virtual void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetGameStateManager().GetStartOfBattleEvent().AddListener(BattleStarted);

        _myHeroBase.GetHeroManualAbilityAttemptEvent().AddListener(AttemptActivationOfManualAbility);

        _myHeroBase.GetHeroDiedEvent().AddListener(HeroDied);
    }

    #region Events
    protected void InvokeHeroDealtDamageEvent(float damage)
    {
        _heroDealtDamageEvent?.Invoke(damage);
    }
    protected void InvokeHeroDealtStaggerEvent(float stagger)
    {
        _heroDealtStaggerEvent?.Invoke(stagger);
    }
    #endregion

    #region Getters
    public float GetBasicAbilityChargeTime() => _basicAbilityChargeTime;

    public float GetManualAbilityChargePercent() => _manualAbilityCurrentCharge / _manualAbilityChargeTime;

    public Animator GetSpecificHeroAnimator() => _heroSpecificAnimator;

    public UnityEvent<float> GetHeroDealtDamageEvent() => _heroDealtDamageEvent;
    public UnityEvent<float> GetHeroDealtStaggerEvent() => _heroDealtStaggerEvent;
    #endregion

    

}
