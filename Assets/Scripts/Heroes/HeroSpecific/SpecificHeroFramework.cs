using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SpecificHeroFramework : MonoBehaviour
{
    protected float _basicAbilityChargeTime;
    internal float _basicAbilityCurrentCharge = 0;

    protected float _manualAbilityChargeTime;
    internal float _manualAbilityCurrentCharge = 0;

    internal float _basicAbilityRange;

    internal HeroBase myHeroBase;

    protected Coroutine _attemptingBasicAbilitiesCoroutine;
    internal Coroutine _basicAbilityCooldownCoroutine;
    internal Coroutine _manualAbilityCooldownCoroutine;

    #region Basic Abilities
    public virtual void StartCheckingToAttemptBasicAbilities()
    {
        _attemptingBasicAbilitiesCoroutine = StartCoroutine(CheckingToAttemptBasicAbilities());
    }
    public abstract IEnumerator CheckingToAttemptBasicAbilities();
    public abstract bool AttemptBasicAbilities();
    public abstract void ActivateBasicAbilities();

    protected virtual void StartCooldownBasicAbility()
    {
        _basicAbilityCooldownCoroutine = StartCoroutine(CooldownBasicAbility());
    }

    protected virtual void StopCooldownBasicAbility()
    {
        StopCoroutine(_basicAbilityCooldownCoroutine);
    }

    public virtual IEnumerator CooldownBasicAbility()
    {
        _basicAbilityCurrentCharge = 0;
        while (_basicAbilityCurrentCharge < _basicAbilityChargeTime)
        {
            AddToBasicAbilityChargeTime(Time.deltaTime * myHeroBase.GetHeroStats().GetCurrentAttackSpeedMultiplier());
            yield return null;
        }
        StartCheckingToAttemptBasicAbilities();
    }

    public virtual void AddToBasicAbilityChargeTime(float addedAmount)
    {
        _basicAbilityCurrentCharge += addedAmount;
    }

    #endregion

    #region Manual Abilities
    public abstract void ActivateManualAbilities(Vector3 attackLocation);

    public virtual IEnumerator CooldownManualAbility()
    {
        while (_manualAbilityCurrentCharge < _manualAbilityChargeTime)
        {
            AddToBasicAbilityChargeTime(Time.deltaTime);
            yield return null;
        }
        _manualAbilityChargeTime = 0;
    }

    public virtual void AddToManualAbilityChargeTime(float addedAmount)
    {
        _manualAbilityCurrentCharge += addedAmount;
    }
    #endregion

    #region Passive Abilities
    public abstract void ActivatePassiveAbilities();

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
    #endregion

    public virtual void SetupSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        myHeroBase = heroBase;
        SetDefaultValues(heroSO);
        SubscribeToEvents();
    }

    public virtual void BattleStarted()
    {
        SubscribeToHeroSpecificEvents();
    }

    public virtual void SubscribeToHeroSpecificEvents()
    {

    }
    public virtual void UnsubscribeToHeroSpecificEvents()
    {

    }

    public void SetDefaultValues(HeroSO heroSO)
    {
        _basicAbilityChargeTime = heroSO.GetBasicAbilityChargeTime();
    }
    public virtual void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetGameStateManager().GetStartOfBattleEvent().AddListener(BattleStarted);

        SubscribeToHeroSpecificEvents();
    }

}
