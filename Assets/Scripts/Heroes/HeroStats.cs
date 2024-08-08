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

    private float _heroDefaultMoveSpeed;
    private float _currentMoveSpeed;

    private float _heroDefaultAcceleration;
    private float _currentAcceleration;

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

    [SerializeField] private Sprite _damageBuffIcon;
    [SerializeField] private Sprite _staggerBuffIcon;
    [SerializeField] private Sprite _speedBuffIcon;
    [SerializeField] private Sprite _healingBuffIcon;

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
        _heroDefaultMoveSpeed = heroSO.GetMoveSpeed();
        _heroDefaultAcceleration = heroSO.GetMoveAcceleration();
        _heroDefaultAggro = heroSO.GetAggro();
        _heroDefaultDamageResistance = heroSO.GetDamageResistance();


        _currentHealth = _heroMaxHealth;
        _currentMoveSpeed = _heroDefaultMoveSpeed;
        _currentAcceleration = _heroDefaultAcceleration;
        _currentAggro = _heroDefaultAggro;
        _currentDamageResistance = _heroDefaultDamageResistance;

        //Sets up the movement speed
        myHeroBase.GetPathfinding().GetNavMeshAgent().speed = _heroDefaultMoveSpeed;
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

        if (healing == 0 || IsHeroMaxHealth()) return;

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
    public void ApplyStatChangeForDuration(HeroGeneralAdjustableStats stat, 
        float changeValue, float secondaryValue, float duration)
    {
        StartCoroutine(StatChangeDurationProcess(stat, changeValue, secondaryValue, duration));
    }

    private IEnumerator StatChangeDurationProcess(HeroGeneralAdjustableStats stat,
        float changeValue, float secondaryValue, float duration)
    {
        ChangeSpecificStat(stat, changeValue,secondaryValue);
        yield return new WaitForSeconds(duration);
        ChangeSpecificStat(stat, -changeValue, -secondaryValue);
    }

    private void ChangeSpecificStat(HeroGeneralAdjustableStats stat, float changeValue, float secondaryValue)
    {
        switch(stat)
        {
            case (HeroGeneralAdjustableStats.DamageMultiplier):
                ChangeCurrentHeroDamageMultiplier(changeValue);
                myHeroBase.GetHeroUIManager().CreateBuffDebuffIcon(_damageBuffIcon, changeValue > 0);
                return;
            case (HeroGeneralAdjustableStats.StaggerMultiplier):
                ChangeCurrentHeroStaggerMultiplier(changeValue);
                myHeroBase.GetHeroUIManager().CreateBuffDebuffIcon(_staggerBuffIcon, changeValue > 0);
                return;
            case (HeroGeneralAdjustableStats.HealingDealtMultiplier):
                ChangeCurrentHeroHealingDealtMultiplier(changeValue);
                return;
            case (HeroGeneralAdjustableStats.HealingRecievedMultiplier):
                ChangeCurrentHeroHealingReceivedMultiplier(changeValue);
                myHeroBase.GetHeroUIManager().CreateBuffDebuffIcon(_healingBuffIcon, changeValue > 0);
                return;
            case (HeroGeneralAdjustableStats.SpeedMultiplier):
                ChangeCurrentHeroSpeed(changeValue);
                ChangeCurrentHeroAcceleration(secondaryValue);
                myHeroBase.GetHeroUIManager().CreateBuffDebuffIcon(_speedBuffIcon, changeValue > 0);
                return;
            case (HeroGeneralAdjustableStats.AggroMultiplier):
                ChangeCurrentHeroAggro(changeValue);
                return;
            case (HeroGeneralAdjustableStats.DamageResistanceMultiplier):
                ChangeCurrentHeroDamageResistance(changeValue);
                return;
        }
    }

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
        _currentMoveSpeed += changeValue;

        myHeroBase.GetPathfinding().GetNavMeshAgent().speed = _currentMoveSpeed;
    }

    public void ChangeCurrentHeroAcceleration(float changeValue)
    {
        _currentAcceleration += changeValue;

        myHeroBase.GetPathfinding().GetNavMeshAgent().acceleration = _currentAcceleration;
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
    public float GetCurrentSpeed() => _currentMoveSpeed;
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

public enum HeroGeneralAdjustableStats
{
    DamageMultiplier,
    StaggerMultiplier,
    HealingDealtMultiplier,
    HealingRecievedMultiplier,
    SpeedMultiplier,
    AccelerationMultiplier,
    AggroMultiplier,
    DamageResistanceMultiplier
};
