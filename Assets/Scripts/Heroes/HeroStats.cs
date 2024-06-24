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
        
        _currentHealth -= damage;
        myHeroBase.InvokeHeroDamagedEvent(damage);
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

        SetPreviousHealthValue();

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

    public void KillHero()
    {
        GameplayManagers.Instance.GetHeroesManager().HeroDied(myHeroBase);
    }

    private void SetPreviousHealthValue()
    {
        _previousHealthValue = _currentHealth;
    }

    #region Damage Override
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

    private bool ShouldOverrideHealing()
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
    #endregion

    #region Setters
    
    #endregion
}
