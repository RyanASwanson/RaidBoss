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

    private float _heroDefaultAngularSpeed;
    private float _currentAngularSpeed;

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


    /// <summary>
    /// Assigns the values of the stats after the heroSO is assigned
    /// </summary>
    /// <param name="heroSO"></param>
    private void StatsSetup(HeroSO heroSO)
    {
        //Sets the starting default stat values
        _heroMaxHealth = heroSO.GetMaxHP();
        _heroDefaultMoveSpeed = heroSO.GetMoveSpeed();
        _heroDefaultAngularSpeed = heroSO.GetAngularSpeed();
        _heroDefaultAcceleration = heroSO.GetMoveAcceleration();
        _heroDefaultAggro = heroSO.GetAggro();
        _heroDefaultDamageResistance = heroSO.GetDamageResistance();

        //Sets the starting current stat values
        _currentHealth = _heroMaxHealth;
        _currentMoveSpeed = _heroDefaultMoveSpeed;
        _currentAngularSpeed = _heroDefaultAngularSpeed;
        _currentAcceleration = _heroDefaultAcceleration;
        _currentAggro = _heroDefaultAggro;
        _currentDamageResistance = _heroDefaultDamageResistance;

        //Sets up the movement speed
        _myHeroBase.GetPathfinding().GetNavMeshAgent().speed = _currentMoveSpeed ;
        _myHeroBase.GetPathfinding().GetNavMeshAgent().angularSpeed = _currentAngularSpeed ;
        _myHeroBase.GetPathfinding().GetNavMeshAgent().acceleration = _currentAcceleration;
    }

    public void DealDamageToHero(float damage)
    {
        //Checks for a damage override before dealing damage
        if (ShouldOverrideDamage())
        {
            _myHeroBase.InvokeHeroDamageOverrideEvent(damage);
            return;
        }

        SetPreviousHealthValue();
        
        _currentHealth -= damage / _currentDamageResistance;
        _myHeroBase.InvokeHeroDamagedEvent(damage / _currentDamageResistance);
        CheckIfHeroIsDead();
    }

    public void HealHero(float healing)
    {
        //Checks for a healing override before healing
        if(ShouldOverrideHealing())
        {
            _myHeroBase.InvokeHeroHealedOverrideEvent(healing);
            return;
        }

        if (healing == 0 || IsHeroMaxHealth()) return;

        //Set the their health was prior to being healed
        SetPreviousHealthValue();

        //Scale healing with healing receieved
        healing *= _currentHealingReceivedMultiplier;

        //Store the health prior to being healed
        float healthDifference = _currentHealth;

        //Increase the current health of the hero
        _currentHealth += healing;

        //Clamps the health within regular bounds
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _heroMaxHealth);

        //Find the difference between the starting and final health
        healthDifference = _currentHealth - healthDifference;
        _myHeroBase.InvokeHeroHealedEvent(healthDifference);
        
    }


    //Checks if the hero has died after taking damage
    private void CheckIfHeroIsDead()
    {
        if (_currentHealth <= 0)
        {
            if(ShouldOverrideDeath())
            {
                _myHeroBase.InvokeHeroDeathOverrideEvent();
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
        //Prevents hero from taking damage as they die
        AddDamageTakenOverrideCounter();

        //Tells the hero base to invoke the death event
        _myHeroBase.InvokeHeroDiedEvent();
        //Tells the heroes manager that this hero died
        GameplayManagers.Instance.GetHeroesManager().HeroDied(_myHeroBase);
    }

    public void ForceKillHero()
    {
        _myHeroBase.InvokeHeroDiedEvent();
    }

    private void SetPreviousHealthValue()
    {
        _previousHealthValue = _currentHealth;
    }

    #region Damage Override

    /// <summary>
    /// Determines if damaged should be overridden based on if they have damage overrides.
    /// </summary>
    /// <returns></returns>
    private bool ShouldOverrideDamage()
    {
        return _damageTakenOverridesCounter > 0;
    }

    /// <summary>
    /// Add 1 to the counter that tracks all active damage overrides.
    /// </summary>
    public void AddDamageTakenOverrideCounter()
    {
        _damageTakenOverridesCounter++;
    }
    /// <summary>
    /// Remove 1 from the counter that tracks all active damage overrides.
    /// </summary>
    public void RemoveDamageTakenOverrideCounter()
    {
        _damageTakenOverridesCounter--;
    }
    #endregion

    #region Healing Override

    /// <summary>
    /// Determines if healing should be overidden based on if they have healing overrides.
    /// </summary>
    /// <returns></returns>
    public bool ShouldOverrideHealing()
    {
        return _healingTakenOverridesCounter > 0;
    }
    /// <summary>
    /// Add 1 to the counter that tracks all active healing overrides.
    /// </summary>
    public void AddHealingTakenOverrideCounter()
    {
        _healingTakenOverridesCounter++;
    }
    /// <summary>
    /// Remove 1 from the counter that tracks all active healing overrides.
    /// </summary>
    public void RemoveHealingTakenOverrideCounter()
    {
        _healingTakenOverridesCounter--;
    }

    #endregion

    #region Death Override
    /// <summary>
    /// Determines if death should be overridden based on if they have any death overrides.
    /// Only taken into account when damage kills the hero, not if they are forcibly killed.
    ///     -EX: Terra Lord Passive forcibly kills
    /// </summary>
    /// <returns></returns>
    private bool ShouldOverrideDeath()
    {
        return _deathOverridesCounter > 0;
    }
    /// <summary>
    /// Add 1 to the counter that tracks all active death overrides.
    /// </summary>
    public void AddDeathOverrideCounter()
    {
        _deathOverridesCounter++;
    }

    /// <summary>
    /// Remove 1 from the counter that tracks all active death overrides.
    /// </summary>
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
            _myHeroBase.InvokeHeroDamagedUnderHalfEvent();

            _myHeroBase.GetHeroDamagedEvent()?.RemoveListener(CheckHeroDamagedUnderHalf);
            _myHeroBase.GetHeroDamagedEvent().AddListener(CheckHeroDamagedUnderQuarter);

            _myHeroBase.GetHeroHealedEvent().AddListener(CheckHeroHealedAboveHalf);

            CheckHeroDamagedUnderQuarter(damage);
        }
    }

    private void CheckHeroDamagedUnderQuarter(float damage)
    {
        if (GetHeroHealthPercentage() < .25f)
        {
            _myHeroBase.InvokeHeroDamagedUnderQuarterEvent();

            _myHeroBase.GetHeroDamagedEvent()?.RemoveListener(CheckHeroDamagedUnderQuarter);

            _myHeroBase.GetHeroHealedEvent().RemoveListener(CheckHeroHealedAboveHalf);
            _myHeroBase.GetHeroHealedEvent().AddListener(CheckHeroHealedAboveQuarter);
        }
    }

    private void CheckHeroHealedAboveHalf(float healing)
    {
        if (GetHeroHealthPercentage() > .5f)
        {
            _myHeroBase.InvokeHeroHealedAboveHalfEvent();

            _myHeroBase.GetHeroHealedEvent()?.RemoveListener(CheckHeroHealedAboveHalf);

            _myHeroBase.GetHeroDamagedEvent()?.RemoveListener(CheckHeroDamagedUnderQuarter);
            _myHeroBase.GetHeroDamagedEvent().AddListener(CheckHeroDamagedUnderHalf);
        }
    }

    private void CheckHeroHealedAboveQuarter(float healing)
    {
        if (GetHeroHealthPercentage() > .25f)
        {
            _myHeroBase.InvokeHeroHealedAboveQuarterEvent();

            _myHeroBase.GetHeroHealedEvent()?.RemoveListener(CheckHeroHealedAboveQuarter);
            _myHeroBase.GetHeroHealedEvent()?.AddListener(CheckHeroHealedAboveHalf);

            _myHeroBase.GetHeroDamagedEvent().AddListener(CheckHeroDamagedUnderQuarter);

            CheckHeroHealedAboveHalf(healing);
        }
    }

    #endregion

    #region Stat Changes

    /// <summary>
    /// Starts the process of applying a stat change to the hero
    /// </summary>
    /// <param name="stat"></param> Which stat is being adjusted
    /// <param name="changeValue"></param> How much the stat is being changed by
    /// <param name="secondaryValue"></param> How much a secondary stat (if it exists) is changed by
    /// <param name="duration"></param> How long the stat is changed for 
    public void ApplyStatChangeForDuration(HeroGeneralAdjustableStats stat, 
        float changeValue, float secondaryValue, float duration)
    {
        StartCoroutine(StatChangeDurationProcess(stat, changeValue, secondaryValue, duration));
    }

    /// <summary>
    /// Applies the stat change, then waits for the duration to reverse it
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="changeValue"></param>
    /// <param name="secondaryValue"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator StatChangeDurationProcess(HeroGeneralAdjustableStats stat,
        float changeValue, float secondaryValue, float duration)
    {
        ChangeSpecificStat(stat, changeValue,secondaryValue,true);
        yield return new WaitForSeconds(duration);
        ChangeSpecificStat(stat, -changeValue, -secondaryValue,true);
    }


    private void ChangeSpecificStat(HeroGeneralAdjustableStats stat, float changeValue, float secondaryValue, bool createBuffIcons)
    {
        Sprite buffDebuffIconSprite = null;

        switch(stat)
        {
            case (HeroGeneralAdjustableStats.DamageMultiplier):
                ChangeCurrentHeroDamageMultiplier(changeValue);

                buffDebuffIconSprite = _damageBuffIcon;
                break;
            case (HeroGeneralAdjustableStats.StaggerMultiplier):
                ChangeCurrentHeroStaggerMultiplier(changeValue);

                buffDebuffIconSprite = _staggerBuffIcon;
                break;
            case (HeroGeneralAdjustableStats.HealingDealtMultiplier):
                ChangeCurrentHeroHealingDealtMultiplier(changeValue);
                return;
            case (HeroGeneralAdjustableStats.HealingRecievedMultiplier):
                ChangeCurrentHeroHealingReceivedMultiplier(changeValue);

                buffDebuffIconSprite = _healingBuffIcon;
                break;
            case (HeroGeneralAdjustableStats.SpeedMultiplier):
                ChangeCurrentHeroSpeed(changeValue);
                ChangeCurrentHeroAcceleration(secondaryValue);

                buffDebuffIconSprite = _speedBuffIcon;
                break;
            case (HeroGeneralAdjustableStats.AggroMultiplier):
                ChangeCurrentHeroAggro(changeValue);
                break;
            case (HeroGeneralAdjustableStats.DamageResistanceMultiplier):
                ChangeCurrentHeroDamageResistance(changeValue);
                break;
        }

        // If the stat has an icon associated with it and creating icons is allowed
        // Tell the associated HeroUI manager to create a buff/debuff icon
        if (buffDebuffIconSprite != null && createBuffIcons)
        {
            _myHeroBase.GetHeroUIManager().CreateBuffDebuffIcon(buffDebuffIconSprite, changeValue > 0);
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

        _myHeroBase.GetPathfinding().GetNavMeshAgent().speed = _currentMoveSpeed;
    }

    public void ChangeCurrentHeroAngularSpeed(float changeValue)
    {
        _currentAngularSpeed += changeValue;

        _myHeroBase.GetPathfinding().GetNavMeshAgent().angularSpeed = _currentAngularSpeed;
    }

    public void ChangeCurrentHeroAcceleration(float changeValue)
    {
        _currentAcceleration += changeValue;

        _myHeroBase.GetPathfinding().GetNavMeshAgent().acceleration = _currentAcceleration;
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


    #region Base Hero
    public override void ChildFuncSetup(HeroBase heroBase)
    {
        base.ChildFuncSetup(heroBase);
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroDamagedEvent().AddListener(CheckHeroDamagedUnderHalf);
    }

    protected override void HeroSOAssigned(HeroSO heroSO)
    {
        StatsSetup(heroSO);
        base.HeroSOAssigned(heroSO);
    }

    #endregion

    #region Getters
    public float GetMaxHealth() => _heroMaxHealth;
    public float GetDefaultSpeed() => _heroMaxHealth;
    public float GetDefaultAngularSpeed() => _heroDefaultAngularSpeed;
    public float GetDefaultAggro() => _heroMaxHealth;
    public float GetDefaultDamageResistance() => _heroDefaultDamageResistance;


    public float GetCurrentHealth() => _currentHealth;
    public float GetPreviousHealth() => _previousHealthValue;
    public bool IsHeroMaxHealth() => _currentHealth >= _heroMaxHealth;
    public bool IsHeroDead() => _currentHealth <= 0;
    public bool CanHeroBeHealed() => !IsHeroMaxHealth() && !ShouldOverrideHealing();
    public float GetHeroHealthPercentage() => _currentHealth / _heroMaxHealth;
    public float GetCurrentSpeed() => _currentMoveSpeed;
    public float GetAngularSpeed() => _currentAngularSpeed;
    public float GetAcceleration() => _currentAcceleration;
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


/// <summary>
/// The stats which can be adjusted by buffs/debuffs
/// </summary>
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
