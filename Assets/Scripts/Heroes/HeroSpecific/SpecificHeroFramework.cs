using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
    
    [SerializeField] private float _basicAbilityAnimationBufferBeforeDisable = 0.25f;
    [SerializeField] private float _manualAbilityAnimationBufferBeforeDisable = 0.25f;
    [SerializeField] private float _passiveAbilityAnimationBufferBeforeDisable = 0.25f;
    private WaitForSeconds _basicAbilityAnimationDisableWait;
    private WaitForSeconds _manualAbilityAnimationDisableWait;
    private WaitForSeconds _passiveAbilityAnimationDisableWait;

    [Space] 
    [Header("UI")] 
    [SerializeField] protected GameObject _heroSpecificUI;

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
        if (!_basicAbilityCooldownCoroutine.IsUnityNull())
        {
            StopCoroutine(_basicAbilityCooldownCoroutine);
            _basicAbilityCooldownCoroutine = null;
        }
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

    protected virtual void StopCheckingToAttemptBasicAbilities()
    {
        if (!_attemptingBasicAbilitiesCoroutine.IsUnityNull())
        {
            StopCoroutine(_attemptingBasicAbilitiesCoroutine);
            _attemptingBasicAbilitiesCoroutine = null;
        }
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
        if (!_hasBasicAbilityAnimation)
        {
            return;
        }

        _myHeroBase.GetHeroVisuals().TriggerBasicAbilityAnimation();
        _myHeroBase.GetHeroVisuals().ResetBasicAbilityAnimation(_basicAbilityAnimationDisableWait);
    }

    protected virtual void PlayBasicAbilityAudio()
    {
        if (AudioManager.Instance.PlaySpecificAudio(
                AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()].BasicAbilityUsed,
                out EventInstance eventInstance))
        {
            BasicAbilityAudioPlayed(eventInstance);
        }
    }

    protected virtual void BasicAbilityAudioPlayed(EventInstance eventInstance)
    {
        
    }

    public virtual void ActivateBasicAbilities()
    {
        TriggerBasicAbilityAnimation();
        
        PlayBasicAbilityAudio();

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
        if (!_manualAbilityCooldownCoroutine.IsUnityNull())
        {
            StopCoroutine(_manualAbilityCooldownCoroutine);
            _manualAbilityCooldownCoroutine = null;
        }
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

    public virtual void AttemptActivationOfManualAbility()
    {
        if(_manualAbilityCurrentCharge >= _manualAbilityChargeTime)
        {
            ActivateManualAbilities();
        }
    }

    protected virtual void TriggerManualAbilityAnimation()
    {
        if (!_hasManualAbilityAnimation) return;

        _myHeroBase.GetHeroVisuals().TriggerManualAbilityAnimation();
        _myHeroBase.GetHeroVisuals().ResetManualAbilityAnimation(_manualAbilityAnimationDisableWait);
    }
    
    protected virtual void PlayManualAbilityAudio()
    {
        if (AudioManager.Instance.PlaySpecificAudio(
                AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()].ManualAbilityUsed,
                out EventInstance eventInstance))
        {
            ManualAbilityAudioPlayed(eventInstance);
        }
    }

    protected virtual void ManualAbilityAudioPlayed(EventInstance eventInstance)
    {
        
    }

    public virtual void ActivateManualAbilities()
    {
        _manualAbilityCurrentCharge = 0;

        _manualAbilityCooldownCoroutine = null;

        TriggerManualAbilityAnimation();
        
        PlayManualAbilityAudio();

        StartCooldownManualAbility();

        _myHeroBase.InvokeHeroManualAbilityUsedEvent();
    }
    #endregion

    #region Passive Abilities

    protected virtual void TriggerPassiveAbilityAnimation()
    {
        if (!_hasPassiveAbilityAnimation) return;

        _myHeroBase.GetHeroVisuals().TriggerPassiveAbilityAnimation();
        _myHeroBase.GetHeroVisuals().ResetPassiveAbilityAnimation(_passiveAbilityAnimationDisableWait);
    }
    
    protected virtual void PlayPassiveAbilityAudio()
    {
        if (AudioManager.Instance.PlaySpecificAudio(
                AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()].PassiveAbilityUsed,
                out EventInstance eventInstance))
        {
            PassiveAbilityAudioPlayed(eventInstance);
        }
    }

    protected virtual void PassiveAbilityAudioPlayed(EventInstance eventInstance)
    {
        
    }

    public virtual void ActivatePassiveAbilities()
    {
        TriggerPassiveAbilityAnimation();
        
        PlayPassiveAbilityAudio();
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
        SetInitialValues();
        SubscribeToEvents();
    }

    private void SetInitialValues()
    {
        _basicAbilityAnimationDisableWait = new WaitForSeconds(_basicAbilityAnimationBufferBeforeDisable);
        _manualAbilityAnimationDisableWait = new WaitForSeconds(_manualAbilityAnimationBufferBeforeDisable);
        _passiveAbilityAnimationDisableWait = new WaitForSeconds(_passiveAbilityAnimationBufferBeforeDisable);
    }

    /// <summary>
    /// Causes the hero to start using their abilities and provides
    /// an overridable battle started functionality specific to the hero
    /// </summary>
    protected virtual void BattleStarted()
    {
        ActivateHeroSpecificActivity();
    }

    protected virtual void BattleWon()
    {
        DeactivateHeroSpecificActivity();
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
        StopCheckingToAttemptBasicAbilities();
        StopCooldownManualAbility();
    }

    public virtual void HeroSpecificUICreated(GameObject heroSpecificUI)
    {
        
    }

    /// <summary>
    /// Provides an overridable event subscription function
    /// </summary>
    protected virtual void SubscribeToEvents()
    {
        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(BattleStarted);
        GameStateManager.Instance.GetBattleWonEvent().AddListener(BattleWon);
        _myHeroBase.GetHeroDiedEvent().AddListener(HeroDied);
    }

    #region Getters
    public float GetBasicAbilityChargeTime() => _basicAbilityChargeTime;

    public float GetManualAbilityChargePercent() => _manualAbilityCurrentCharge / _manualAbilityChargeTime;

    public Animator GetSpecificHeroAnimator() => _heroSpecificAnimator;
    
    public GameObject GetSpecificHeroUI() => _heroSpecificUI;
    #endregion
}
