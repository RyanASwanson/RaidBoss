using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains the basic functionality that all specific heroes will inherit from
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

    #region Basic Abilities
    /// <summary>
    /// Starts the cooldown of the heroes basic ability and stores the coroutine
    /// </summary>
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
        {
            yield return new WaitForFixedUpdate();
        }

        ActivateBasicAbilities();
        _attemptingBasicAbilitiesCoroutine = null;
    }

    /// <summary>
    /// Provides the rules for what conditions have to be true in order
    ///     for the basic ability to be used
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Starts the cooldown of the heroes manual ability and stores the coroutine
    /// </summary>
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

        _myHeroBase.InvokeHeroManualAbilityUsedEvent(attackLocation);
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

    /// <summary>
    /// Provides the functionality for the hero to deal damage to the boss.
    /// All damage dealt by a hero goes through here
    /// </summary>
    /// <param name="damage"></param>
    public virtual void DamageBoss(float damage)
    {
        damage *= _myHeroBase.GetHeroStats().GetCurrentDamageMultiplier();

        BossStats.Instance.DealDamageToBoss(damage);

        _myHeroBase.InvokeHeroDealtDamageEvent(damage);
    }

    /// <summary>
    /// Provides the functionality for a hero to deal stagger to the boss
    /// </summary>
    /// <param name="stagger"></param>
    public virtual void StaggerBoss(float stagger)
    {
        stagger *= _myHeroBase.GetHeroStats().GetCurrentStaggerMultiplier();

        BossStats.Instance.DealStaggerToBoss(stagger);

        _myHeroBase.InvokeHeroDealtStaggerEvent(stagger);
    }

    /// <summary>
    /// Provides the functionality to heal a specific hero
    /// </summary>
    /// <param name="healing"></param>
    /// <param name="target"></param>
    public virtual void HealTargetHero(float healing, HeroBase target)
    {
        healing *= _myHeroBase.GetHeroStats().GetCurrentHealingDealtMultiplier();
        target.GetHeroStats().HealHero(healing);
    }

    #endregion

    public virtual void SetUpSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        _myHeroBase = heroBase;
        SubscribeToEvents();
    }

    /// <summary>
    /// Causes the hero to start using their abilities and provides
    /// an overridable battle started functionality specific to the hero
    /// </summary>
    protected virtual void BattleStarted()
    {
        ActivateHeroSpecificActivity();
    }

    /// <summary>
    /// Provides an overridable death functionality specific to the hero
    /// </summary>
    protected virtual void HeroDied()
    {

    }
    
    /// <summary>
    /// Starts the cooldowns for abilities
    /// Called at the start of the battle
    /// </summary>
    public virtual void ActivateHeroSpecificActivity()
    {
        StartCooldownBasicAbility();
        StartCooldownManualAbility();
    }

    /// <summary>
    /// Stops all cooldowns from being used
    /// </summary>
    public virtual void DeactivateHeroSpecificActivity()
    {
        StopCooldownBasicAbility();
        StopCooldownManualAbility();
    }

    /// <summary>
    /// Provides an overridable event subscription function
    /// </summary>
    protected virtual void SubscribeToEvents()
    {
        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(BattleStarted);

        _myHeroBase.GetHeroDiedEvent().AddListener(HeroDied);
    }

    #region Getters
    public float GetBasicAbilityChargeTime() => _basicAbilityChargeTime;

    public float GetManualAbilityChargePercent() => _manualAbilityCurrentCharge / _manualAbilityChargeTime;

    public Animator GetSpecificHeroAnimator() => _heroSpecificAnimator;
    #endregion
}
