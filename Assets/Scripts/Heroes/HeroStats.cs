using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStats : HeroChildrenFunctionality
{
    private float _heroMaxHealth;
    private float _currentHealth;

    private float _heroDefaultMovespeed;
    private float _currentMovespeed;

    private float _heroDefaultAggro;
    private float _currentAggro;

    private const float _heroDefaultAttackSpeedMultiplier = 1;
    private float _currentAttackSpeedMultiplier;

    private float _heroDefaultDamageResistance;
    private float _currentDamageResistance;

    private float _heroDefaultBasicAbilityChargeTime;
    private float _heroDefaultBasicAbilityRange;
    private float _heroDefaultBasicAbilityStrength;

    public override void ChildFuncSetup(HeroBase heroBase)
    {
        base.ChildFuncSetup(heroBase);
    }

    private void StatsSetup(HeroSO heroSO)
    {
        _heroMaxHealth = heroSO.GetMaxHP();
        _heroDefaultMovespeed = heroSO.GetMoveSpeed();
        _heroDefaultAggro = heroSO.GetAggro();
        _heroDefaultDamageResistance = heroSO.GetDamageResistance();

        _heroDefaultBasicAbilityChargeTime = heroSO.GetBasicAbilityChargeTime();
        _heroDefaultBasicAbilityRange = heroSO.GetBasicAbilityRange();
        _heroDefaultBasicAbilityStrength = heroSO.GetBasicAbilityStrength();

        _currentHealth = _heroMaxHealth;
        _currentMovespeed = _heroDefaultMovespeed;
        _currentAggro = _heroDefaultAggro;
        _currentAttackSpeedMultiplier = _heroDefaultAttackSpeedMultiplier;
        _currentDamageResistance = _heroDefaultDamageResistance;

        myHeroBase.GetPathfinding().GetNavMeshAgent().speed = _heroDefaultMovespeed;
    }

    private void CheckIfHeroIsDead()
    {
        if (_currentHealth <= 0)
        {
            GameplayManagers.Instance.GetHeroesManager().HeroDied(myHeroBase);
        }
    }

    #region Events
    public override void SubscribeToEvents()
    {
        myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);
    }

    private void HeroSOAssigned(HeroSO heroSO)
    {
        StatsSetup(heroSO);
    }

    #endregion

    #region Getters
    public float GetMaxHealth() => _heroMaxHealth;
    public float GetDefaultSpeed() => _heroMaxHealth;
    public float GetDefaultAggro() => _heroMaxHealth;
    public float GetDefaultAttackSpeedMultiplier() => _heroDefaultAttackSpeedMultiplier;
    public float GetDefaultDamageResistance() => _heroDefaultDamageResistance;

    public float GetDefaultBasicAbilityChargeTime() => _heroDefaultBasicAbilityChargeTime;
    public float GetDefaultBasicAbilityRange() => _heroDefaultBasicAbilityRange;
    public float GetDefaultBasicAbilityStrength() => _heroDefaultBasicAbilityStrength;

    public float GetCurrentHealth() => _currentHealth;
    public float GetCurrentSpeed() => _currentMovespeed;
    public float GetCurrentAggro() => _currentAggro;
    public float GetCurrentAttackSpeedMultiplier() => _currentAttackSpeedMultiplier;
    public float GetCurrentDamageResistance() => _currentDamageResistance;
    #endregion

    #region Setters
    public void DealDamageToHero(float damage)
    {
        _currentHealth -= damage;
        CheckIfBossIsDead();
    }
    #endregion
}
