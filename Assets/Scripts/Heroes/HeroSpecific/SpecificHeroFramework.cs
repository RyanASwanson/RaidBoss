using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SpecificHeroFramework : MonoBehaviour
{
    internal float _basicAbilityCurrentCharge = 0;

    /*
    protected float _manualAbilityChargeTime;
    internal float _manualAbilityCurrentCharge = 0;*/

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
        while (_basicAbilityCurrentCharge < myHeroBase.GetHeroStats().GetDefaultBasicAbilityChargeTime())
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
    }
    public abstract bool ConditionsToActivateBasicAbilities();
    public virtual void ActivateBasicAbilities()
    {
        StartCooldownBasicAbility();
    }


    

    #endregion

    #region Manual Abilities
    public abstract void ActivateManualAbilities(Vector3 attackLocation);

    /*public virtual IEnumerator CooldownManualAbility()
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
    }*/
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

    protected virtual void DamageBoss(float damage)
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossStats().DealDamageToBoss(damage);
    }

    protected virtual void StaggerBoss(float stagger)
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossStats().DealStaggerToBoss(stagger);
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
    }
    public virtual void DeactivateHeroSpecificActivity()
    {
        StopCooldownBasicAbility();
    }

    public void SetDefaultValues(HeroSO heroSO)
    {

    }

    public virtual void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetGameStateManager().GetStartOfBattleEvent().AddListener(BattleStarted);
    }

}
