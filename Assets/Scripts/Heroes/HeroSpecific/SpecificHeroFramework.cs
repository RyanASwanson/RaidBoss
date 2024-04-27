using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SpecificHeroFramework : MonoBehaviour
{
    internal float _basicAbilityChargeTime;
    internal float _manualAbilityChargeTime;

    internal float _basicAbilityCurrentCharge = 0;
    internal float _manualAbilityCurrentCharge = 0;

    internal HeroBase myHeroBase;

    internal Coroutine basicAbilityCooldownCoroutine;
    internal Coroutine manualAbilityCooldownCoroutine;

    #region Basic Abilities
    public abstract void ActivateBasicAbilities();

    public virtual IEnumerator CooldownBasicAbility()
    {
        while(_basicAbilityChargeTime < _basicAbilityCurrentCharge)
        {
            AddToBasicAbilityChargeTime(Time.deltaTime * myHeroBase.GetHeroStats().GetCurrentAttackSpeedMultiplier());
            yield return null;
        }
        _basicAbilityChargeTime = 0;
    }

    public virtual void AddToBasicAbilityChargeTime(float addedAmount)
    {
        _basicAbilityCurrentCharge += addedAmount;
    }

    #endregion

    #region Manual Abilities
    public abstract void ActivateManualAttack(Vector3 attackLocation);

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
    public virtual void SetupSpecificHero(HeroBase heroBase)
    {
        myHeroBase = heroBase;
        SubscribeToEvents();
    }

    public virtual void SubscribeToEvents()
    {
        
    }
}
