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

    internal HeroBase myHeroBase;

    protected Coroutine _attemptingBasicAbilitiesCoroutine;
    internal Coroutine _basicAbilityCooldownCoroutine;
    internal Coroutine _manualAbilityCooldownCoroutine;

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

    public virtual IEnumerator CooldownBasicAbility()
    {
        _basicAbilityCurrentCharge = 0;
        while (_basicAbilityCurrentCharge < _basicAbilityChargeTime)
        {
            AddToBasicAbilityChargeTime(Time.deltaTime * myHeroBase.GetHeroStats().GetCurrentAttackSpeedMultiplier());
            yield return null;
        }

        BasicAbilityCooldownReady();
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
        return false;
    }

    public virtual void ActivateBasicAbilities()
    {
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
            myHeroBase.InvokeHeroManualAbilityChargingEvent();
            yield return null;
        }

        myHeroBase.InvokeHeroManualAbilityFullyChargedEvent();
    }

    public virtual void AddToManualAbilityChargeTime(float addedAmount)
    {
        _manualAbilityCurrentCharge += addedAmount;
    }

    public virtual void AttemptActivationOfManualAbility(Vector3 activateLocation)
    {
        if(_manualAbilityCurrentCharge >= _manualAbilityChargeTime)
        {
            ActivateManualAbilities(activateLocation);
        }
    }

    public virtual void ActivateManualAbilities(Vector3 attackLocation)
    {
        _manualAbilityCurrentCharge = 0;

        _manualAbilityCooldownCoroutine = null;
        StartCooldownManualAbility();
    }
    #endregion

    #region Passive Abilities
    public virtual void ActivatePassiveAbilities()
    {

    }

    #endregion

    #region GeneralAbilityFunctionality
    protected virtual bool InAttackRangeOfBoss(float attackRange)
    {
        Vector2 heroPos = new Vector2(myHeroBase.gameObject.transform.position.x, 
            myHeroBase.gameObject.transform.position.z);
        Vector2 bossPos = new Vector2(GameplayManagers.Instance.GetBossManager().GetBossBase().transform.position.x,
            GameplayManagers.Instance.GetBossManager().GetBossBase().transform.position.z);

        return Vector2.Distance(heroPos, bossPos) < attackRange;
    }

    public virtual void DamageBoss(float damage)
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossStats().DealDamageToBoss(damage);
    }

    public virtual void StaggerBoss(float stagger)
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossStats().DealStaggerToBoss(stagger);
    }

    public virtual void HeroTakeDamageOverride(float damage)
    {

    }
    #endregion

    public virtual void SetupSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        myHeroBase = heroBase;
        SetDefaultValues(heroSO);
        SubscribeToEvents();
    }

    public virtual void BattleStarted()
    {
        ActivateHeroSpecificActivity();
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

    public virtual void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetGameStateManager().GetStartOfBattleEvent().AddListener(BattleStarted);

        myHeroBase.GetHeroManualAbilityAttemptEvent().AddListener(AttemptActivationOfManualAbility);
    }

    #region Getters
    public float GetBasicAbilityChargeTime() => _basicAbilityChargeTime;

    public float GetManualAbilityChargePercent() => _manualAbilityCurrentCharge / _manualAbilityChargeTime;
    #endregion

}
