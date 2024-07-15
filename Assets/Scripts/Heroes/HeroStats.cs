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
    private float _previousHealthValue;

    private float _heroDefaultMovespeed;
    private float _currentMovespeed;

    private float _heroDefaultAggro;
    private float _currentAggro;

    private float _heroDefaultDamageResistance;
    private float _currentDamageResistance;

    private float _currentDamageMultiplier = 1;
    private float _currentStaggerMultiplier = 1;
    private float _currentHealingDealtMultiplier = 1;
    private float _currentHealingReceivedMultiplier = 1;

    private int _damageTakenOverridesCounter = 0;
    private int _healingTakenOverridesCounter = 0;
    private int _deathOverridesCounter = 0;

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

        SetPreviousHealthValue();
        
        _currentHealth -= damage / _currentDamageResistance;
        myHeroBase.InvokeHeroDamagedEvent(damage / _currentDamageResistance);
        CheckIfHeroIsDead();
    }

    public void HealHero(float healing)
    {
        //Checks for a healing override before healing
        if(ShouldOverrideHealing())
        {
            myHeroBase.InvokeHeroHealedOverrideEvent(healing);
            return;
        }

        if (healing == 0) return;

        SetPreviousHealthValue();

        healing *= _currentHealingReceivedMultiplier;

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
            if(ShouldOverrideDeath())
            {
                myHeroBase.InvokeHeroDeathOverrideEvent();
                return;
            }

            KillHero();
        }
    }

    /// <summary>
    /// Informs the hero manager of the heroes death
    /// </summary>
    public void KillHero()
    {
        AddDamageTakenOverrideCounter();

        myHeroBase.InvokeHeroDiedEvent();
        GameplayManagers.Instance.GetHeroesManager().HeroDied(myHeroBase);
    }

    private void SetPreviousHealthValue()
    {
        _previousHealthValue = _currentHealth;
    }

    #region Damage Override

    /// <summary>
    /// Determines if damaged should be overridden based on if they have damage overrides
    /// </summary>
    /// <returns></returns>
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
    #endregion

    #region Healing Override

    public bool ShouldOverrideHealing()
    {
        return _healingTakenOverridesCounter > 0;
    }

    public void AddHealingTakenOverrideCounter()
    {
        _healingTakenOverridesCounter++;
    }


    public void RemoveHealingTakenOverrideCounter()
    {
        _healingTakenOverridesCounter--;
    }

    #endregion

    #region Death Override
    private bool ShouldOverrideDeath()
    {
        return _deathOverridesCounter > 0;
    }

    public void AddDeathOverrideCounter()
    {
        _deathOverridesCounter++;
    }


    public void RemoveDeathOverrideCounter()
    {
        _deathOverridesCounter--;
    }
    #endregion

    #region Health Threshholds
    private void CheckHeroDamagedUnderHalf(float damage)
    {
        if (GetHeroHealthPercentage() < .5f)
        {
            myHeroBase.InvokeHeroDamagedUnderHalfEvent();

            myHeroBase.GetHeroDamagedEvent()?.RemoveListener(CheckHeroDamagedUnderHalf);
            myHeroBase.GetHeroDamagedEvent().AddListener(CheckHeroDamagedUnderQuarter);

            myHeroBase.GetHeroHealedEvent().AddListener(CheckHeroHealedAboveHalf);

            CheckHeroDamagedUnderQuarter(damage);
        }
    }

    private void CheckHeroDamagedUnderQuarter(float damage)
    {
        if (GetHeroHealthPercentage() < .25f)
        {
            myHeroBase.InvokeHeroDamagedUnderQuarterEvent();

            myHeroBase.GetHeroDamagedEvent()?.RemoveListener(CheckHeroDamagedUnderQuarter);

            myHeroBase.GetHeroHealedEvent().RemoveListener(CheckHeroHealedAboveHalf);
            myHeroBase.GetHeroHealedEvent().AddListener(CheckHeroHealedAboveQuarter);
        }
    }

    private void CheckHeroHealedAboveHalf(float healing)
    {
        if (GetHeroHealthPercentage() > .5f)
        {
            myHeroBase.InvokeHeroHealedAboveHalfEvent();

            myHeroBase.GetHeroHealedEvent()?.RemoveListener(CheckHeroHealedAboveHalf);

            myHeroBase.GetHeroDamagedEvent()?.RemoveListener(CheckHeroDamagedUnderQuarter);
            myHeroBase.GetHeroDamagedEvent().AddListener(CheckHeroDamagedUnderHalf);
        }
    }

    private void CheckHeroHealedAboveQuarter(float healing)
    {
        if (GetHeroHealthPercentage() > .25f)
        {
            myHeroBase.InvokeHeroHealedAboveQuarterEvent();

            myHeroBase.GetHeroHealedEvent()?.RemoveListener(CheckHeroHealedAboveQuarter);
            myHeroBase.GetHeroHealedEvent()?.AddListener(CheckHeroHealedAboveHalf);

            myHeroBase.GetHeroDamagedEvent().AddListener(CheckHeroDamagedUnderQuarter);

            CheckHeroHealedAboveHalf(healing);
        }
    }

    #endregion

    #region Stat Changes
    public void ChangeCurrentHeroDamageMultiplier(float changeValue)
    {
        _currentDamageMultiplier += changeValue;
    }

    public void ChangeCurrentHeroStaggerMultiplier(float changeValue)
    {
        _currentStaggerMultiplier += changeValue;
    }

    public void ChangeCurrentHeroHealingDealtMultiplier(float changeValue)
    {
        _currentHealingDealtMultiplier += changeValue;
    }

    public void ChangeCurrentHeroHealingReceivedMultiplier(float changeValue)
    {
        _currentHealingReceivedMultiplier += changeValue;
    }

    /// <summary>
    /// Increases or decreases the current speed value and updates how fast the nav mesh agent moves
    /// </summary>
    /// <param name="changeValue"></param>
    public void ChangeCurrentHeroSpeed(float changeValue)
    {
        _currentMovespeed += changeValue;

        myHeroBase.GetPathfinding().GetNavMeshAgent().speed = _currentMovespeed;
    }

    /// <summary>
    /// Increases or decreases the current aggro value
    /// </summary>
    /// <param name="changeValue"></param>
    public void ChangeCurrentHeroAggro(float changeValue)
    {
        _currentAggro += changeValue;
    }

    /// <summary>
    /// Increases or decreases the current aggro value
    /// </summary>
    /// <param name="changeValue"></param>
    public void ChangeCurrentHeroDamageResistance(float changeValue)
    {
        _currentDamageResistance += changeValue;
    }
    #endregion

    #region Events
    public override void SubscribeToEvents()
    {
        myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);

        myHeroBase.GetHeroDamagedEvent().AddListener(CheckHeroDamagedUnderHalf);
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
    public float GetDefaultDamageResistance() => _heroDefaultDamageResistance;


    public float GetCurrentHealth() => _currentHealth;
    public float GetPreviousHealth() => _previousHealthValue;
    public bool IsHeroMaxHealth() => _currentHealth >= _heroMaxHealth;
    public bool CanHeroBeHealed() => !IsHeroMaxHealth() && !ShouldOverrideHealing();
    public float GetHeroHealthPercentage() => _currentHealth / _heroMaxHealth;
    public float GetCurrentSpeed() => _currentMovespeed;
    public float GetCurrentAggro() => _currentAggro;
    public float GetCurrentDamageResistance() => _currentDamageResistance;

    public float GetCurrentDamageMultiplier() => _currentDamageMultiplier;
    public float GetCurrentStaggerMultiplier() => _currentStaggerMultiplier;
    public float GetCurrentHealingDealtMultiplier() => _currentHealingDealtMultiplier;
    public float GetCurrentHealingReceivedMultiplier() => _currentHealingReceivedMultiplier;
    #endregion

    #region Setters
    #endregion
}
