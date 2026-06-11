using System;
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

    [SerializeField] protected bool _doesManualAbilityHaveDuration;
    [SerializeField] protected bool _doesManualAbilityChargeWhileActive;

    [Space] 
    [SerializeField] protected bool _isManualAbilityDurationFixed;
    [SerializeField] protected float _manualAbilityFixedDuration;
    protected WaitForSeconds _manualAbilityFixedWait;
    
    protected float _manualAbilityCurrentCharge = 0;
    
    protected bool _isManualAbilityActive = false;

    protected bool _canHeroChargeAbilities = true;
    protected bool _canHeroUseAbilities = true;

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

    protected bool _isSubscribedToEvents = false;

    #region Basic Abilities

    protected virtual void SetUpBasicAbility()
    {
        
    }
    
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
        AddToBasicAbilityChargeTime(addedAmount*_myHeroBase.GetHeroStats().GetBasicAbilityCooldownRateMultiplier());
    }
    
    public virtual void AddToBasicAbilityChargeTime(float addedAmount)
    {
        if (!_canHeroChargeAbilities)
        {
            return;
        }
        
        _basicAbilityCurrentCharge += addedAmount;
    }

    protected virtual void BasicAbilityCooldownReady()
    {
        _basicAbilityCooldownCoroutine = null;
        StartCheckingToAttemptBasicAbilities();
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
        while (!DoesMeetConditionsToActivateBasicAbilities())
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
    public virtual bool DoesMeetConditionsToActivateBasicAbilities()
    {
        return !_myHeroBase.GetPathfinding().IsHeroMovingWithPathfinding() && _canHeroUseAbilities;
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

    protected virtual void SetUpManualAbility()
    {
        if (SelectionManager.Instance.GetSelectedMissionStatModifiersOut(out MissionStatModifiers missionStatModifiers))
        {
            _manualAbilityChargeTime *= missionStatModifiers.GetHeroManualCooldownTimeMultiplier();
        }

        if (_isManualAbilityDurationFixed)
        {
            _manualAbilityFixedWait = new WaitForSeconds(_manualAbilityFixedDuration);
        }
    }
    
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
            CooldownAddToManualAbilityCharge(Time.deltaTime);
            yield return null;
        }

        if (!_isManualAbilityActive)
        {
            ManualAbilityFullyCharged();
        }
    }
    
    protected virtual void CooldownAddToManualAbilityCharge(float addedAmount)
    {
        AddToManualAbilityChargeTime(addedAmount*_myHeroBase.GetHeroStats().GetManualAbilityCooldownRateMultiplier());
    }
    
    public virtual void AddToManualAbilityChargeTime(float addedAmount)
    {
        if (!_canHeroChargeAbilities)
        {
            return;
        }
        
        _manualAbilityCurrentCharge += addedAmount;
        _myHeroBase.InvokeHeroManualAbilityChargingEvent();
    }

    public virtual void ManualAbilityFullyCharged()
    {
        StopCooldownManualAbility();
        _manualAbilityCurrentCharge = _manualAbilityChargeTime;
        
        _myHeroBase.InvokeHeroManualAbilityFullyChargedEvent();
    }

    public virtual void AttemptActivationOfManualAbility()
    {
        if(_manualAbilityCurrentCharge >= _manualAbilityChargeTime && !_isManualAbilityActive && _canHeroUseAbilities)
        {
            ActivateManualAbilities();
        }
    }

    protected virtual void TriggerManualAbilityAnimation()
    {
        if (!_hasManualAbilityAnimation)
        {
            return;
        }

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
        _isManualAbilityActive = true;
        
        _manualAbilityCurrentCharge = 0;

        _manualAbilityCooldownCoroutine = null;

        TriggerManualAbilityAnimation();
        
        PlayManualAbilityAudio();

        _myHeroBase.InvokeHeroManualAbilityUsedEvent();

        if (_doesManualAbilityHaveDuration)
        {
            if (_isManualAbilityDurationFixed)
            {
                StartCoroutine(FixedManualAbilityDuration());
            }
            
            if (_doesManualAbilityChargeWhileActive)
            {
                StartCooldownManualAbility();
            }
        }
        else
        {
            EndManualAbility();
        }
    }

    public virtual IEnumerator FixedManualAbilityDuration()
    {
        yield return _manualAbilityFixedWait;
        
        EndManualAbility();
    }

    public virtual void EndManualAbility()
    {
        _isManualAbilityActive = false;

        if (_manualAbilityCurrentCharge >= _manualAbilityChargeTime)
        {
            ManualAbilityFullyCharged();
        }
        else if (!_doesManualAbilityChargeWhileActive)
        {
            StartCooldownManualAbility();
        }
    }
    #endregion

    #region Passive Abilities

    protected virtual void SetUpPassiveAbility()
    {
        
    }

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
        
        // Returns damage adjusted for boss damage resistance
        damage = BossStats.Instance.DealDamageToBoss(damage);
        
        _myHeroBase.InvokeHeroDealtDamageEvent(damage);
        
        _myHeroBase.GetHeroStats().AddToTotalHeroDamageDealt(damage);
    }

    /// <summary>
    /// Provides the functionality for a hero to deal stagger to the boss
    /// </summary>
    /// <param name="stagger"></param>
    public virtual void StaggerBoss(float stagger)
    {
        stagger *= _myHeroBase.GetHeroStats().GetCurrentStaggerMultiplier();

        // Returns stagger adjusted for boss stagger resistance if implemented
        stagger = BossStats.Instance.DealStaggerToBoss(stagger);

        _myHeroBase.InvokeHeroDealtStaggerEvent(stagger);
        
        _myHeroBase.GetHeroStats().AddToTotalHeroStaggerDealt(stagger);
    }

    /// <summary>
    /// Provides the functionality to heal a specific hero
    /// </summary>
    /// <param name="healing"></param>
    /// <param name="target"></param>
    public virtual void HealTargetHero(float healing, HeroBase target)
    {
        healing *= _myHeroBase.GetHeroStats().GetCurrentHealingDealtMultiplier();
        
        // Returns healing adjusted for healing resistance
        healing = target.GetHeroStats().HealHero(healing);
        
        _myHeroBase.GetHeroStats().AddToTotalHeroHealingDealt(healing);
    }

    #endregion

    public virtual void SetUpSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        _myHeroBase = heroBase;
        SetInitialValues();
        SetUpAbilities();
        SubscribeToEvents();
    }

    protected void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SetInitialValues()
    {
        _basicAbilityAnimationDisableWait = new WaitForSeconds(_basicAbilityAnimationBufferBeforeDisable);
        _manualAbilityAnimationDisableWait = new WaitForSeconds(_manualAbilityAnimationBufferBeforeDisable);
        _passiveAbilityAnimationDisableWait = new WaitForSeconds(_passiveAbilityAnimationBufferBeforeDisable);
    }

    protected virtual void SetUpAbilities()
    {
        SetUpBasicAbility();
        SetUpManualAbility();
        SetUpPassiveAbility();
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
        DeactivateHeroSpecificActivity();
        _canHeroUseAbilities = false;
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
        if (_isSubscribedToEvents)
        {
            return;
        }
        
        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(BattleStarted);
        GameStateManager.Instance.GetBattleWonEvent().AddListener(BattleWon);
        _myHeroBase.GetHeroDiedEvent().AddListener(HeroDied);

        _isSubscribedToEvents = true;
    }

    protected virtual void UnsubscribeFromEvents()
    {
        if (!_isSubscribedToEvents)
        {
            return;
        }
        
        GameStateManager.Instance.GetStartOfBattleEvent().RemoveListener(BattleStarted);
        GameStateManager.Instance.GetBattleWonEvent().RemoveListener(BattleWon);
        _myHeroBase.GetHeroDiedEvent().RemoveListener(HeroDied);

        _isSubscribedToEvents = false;
    }

    #region Getters
    public float GetBasicAbilityChargeTime() => _basicAbilityChargeTime;

    public float GetManualAbilityChargePercent() => _manualAbilityCurrentCharge / _manualAbilityChargeTime;

    public Animator GetSpecificHeroAnimator() => _heroSpecificAnimator;
    
    public GameObject GetSpecificHeroUI() => _heroSpecificUI;
    #endregion

    #region MyRegion

    public void SetCanHeroChargeAbilities(bool canHeroChargeAbilities) => _canHeroChargeAbilities = canHeroChargeAbilities;
    public void SetCanHeroUseAbilities(bool canUseAbilities) => _canHeroUseAbilities = canUseAbilities;

    #endregion
}
