using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the general stats for the hero
/// </summary>
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

    private int _damageTakenOverridesCounter = 0;

    public override void ChildFuncSetup(HeroBase heroBase)
    {
        base.ChildFuncSetup(heroBase);
    }

    /// <summary>
    /// Assigns the values of the stats after the heroSO is assigned
    /// </summary>
    /// <param name="heroSO"></param>
    private void StatsSetup(HeroSO heroSO)
    {
        _heroMaxHealth = heroSO.GetMaxHP();
        _heroDefaultMovespeed = heroSO.GetMoveSpeed();
        _heroDefaultAggro = heroSO.GetAggro();
        _heroDefaultDamageResistance = heroSO.GetDamageResistance();


        _currentHealth = _heroMaxHealth;
        _currentMovespeed = _heroDefaultMovespeed;
        _currentAggro = _heroDefaultAggro;
        _currentAttackSpeedMultiplier = _heroDefaultAttackSpeedMultiplier;
        _currentDamageResistance = _heroDefaultDamageResistance;

        //Sets up the movement speed
        myHeroBase.GetPathfinding().GetNavMeshAgent().speed = _heroDefaultMovespeed;
    }

    public void DealDamageToHero(float damage)
    {
        //Checks for a damage override before dealing damage
        if (ShouldOverrideDamage())
        {
            myHeroBase.InvokeHeroDamageOverrideEvent(damage);
            return;
        }
        
        _currentHealth -= damage;
        myHeroBase.InvokeHeroDamagedEvent(damage);
        CheckIfHeroIsDead();
    }

    public void HealHero(float healing)
    {
        float healthDifference = _currentHealth;

        _currentHealth += healing;

        _currentHealth = Mathf.Clamp(_currentHealth, 0, _heroMaxHealth);

        healthDifference = _currentHealth - healthDifference;
        myHeroBase.InvokeHeroHealedEvent(healthDifference);
        
    }

    //Checks if the hero has died after taking damage
    private void CheckIfHeroIsDead()
    {
        if (_currentHealth <= 0)
        {
            GameplayManagers.Instance.GetHeroesManager().HeroDied(myHeroBase);
        }
    }

    private bool ShouldOverrideDamage()
    {
        return _damageTakenOverridesCounter > 0;
    }

    public void AddDamageTakenOverrideCounter()
    {
        _damageTakenOverridesCounter++;
    }
    

    public void RemoveDamageTakenOverrideCounter()
    {
        _damageTakenOverridesCounter--;
    }

    private void CheckBossIsUnderHalf(float damage)
    {
        if (GetHeroHealthPercentage() < .5f)
        {
            myBossBase.InvokeBossHalfHealthEvent();
            IncreaseBossStatsAtHealthThreshholds();

            myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderHalf);
            myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderQuarter);
        }
    }

    private void CheckBossIsUnderQuarter(float damage)
    {
        if (GetBossHealthPercentage() < .25f)
        {
            myBossBase.InvokeBossQuarterHealthEvent();
            IncreaseBossStatsAtHealthThreshholds();

            myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderQuarter);
            myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderTenth);
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


    public float GetCurrentHealth() => _currentHealth;
    public bool IsHeroMaxHealth() => _currentHealth >= _heroMaxHealth;
    public float GetHeroHealthPercentage() => _currentHealth / _heroMaxHealth;
    public float GetCurrentSpeed() => _currentMovespeed;
    public float GetCurrentAggro() => _currentAggro;
    public float GetCurrentAttackSpeedMultiplier() => _currentAttackSpeedMultiplier;
    public float GetCurrentDamageResistance() => _currentDamageResistance;
    #endregion

    #region Setters
    
    #endregion
}
