using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Holds all general stats that bosses have as well as the functionality for
///     affecting those stats
/// Holds health and stagger
/// </summary>
public class BossStats : BossChildrenFunctionality
{
    public static BossStats Instance;
    
    private float _bossMaxHealth;
    private float _currentHealth;

    private float _bossDefaultStaggerMax;
    private float _currentStaggerCounter;

    private float _bossStaggerDuration;

    private float _bossDamageIncrementMultiplier;
    private float _bossDamageResistanceChangeOnStagger;

    private bool _bossStaggered = false;
    private bool _isBossEnraged = false;

    private float _baseBossDamageMultiplier = 1;
    private float _bossDamageResistanceMultiplier =1;

    private float _bossEnrageDamageMultiplier = 1;
    private float _storedEnrageMultiplier;

    private float _currentTimeUntilEnrage;
    private Coroutine _enrageCoroutine;

    #region Set Up
    /// <summary>
    /// Called when the Scriptable Object for the boss is assigned
    /// </summary>
    /// <param name="bossSO"> The scriptable object of the boss </param>
    private void BossSOAssigned(BossSO bossSO)
    {
        StatsSetUp(bossSO);
    }
    
    private void StatsSetUp(BossSO bossSO)
    {
        //Set the boss max health to be the max hp from the SO and multiplied by the difficulty modifier
        _bossMaxHealth = bossSO.GetMaxHP() * 
            SelectionManager.Instance.GetHealthMultiplierFromDifficulty();
        _bossDefaultStaggerMax = bossSO.GetBaseStaggerMax() *
            SelectionManager.Instance.GetStaggerMultiplierFromDifficulty();
        
        //Sets the starting health and stagger values
        _currentHealth = _bossMaxHealth;
        _currentStaggerCounter = 0;

        //Sets the stagger duration from the SO
        _bossStaggerDuration = bossSO.GetStaggerDuration();
        //Sets the damage scaling from the SO
        _bossDamageIncrementMultiplier = bossSO.GetBossDamageIncrementMultiplier();
        //Sets the damage resistance taken when staggered from the SO
        _bossDamageResistanceChangeOnStagger = bossSO.GetDamageResistanceChangeOnStagger();
        
        //Sets the damage dealt multiplier based on the difficulty
        _baseBossDamageMultiplier = SelectionManager.Instance.GetDamageMultiplierFromDifficulty();

        _currentTimeUntilEnrage = bossSO.GetEnrageTime();
        _storedEnrageMultiplier = bossSO.GetEnrageDamageMultiplier();
    }
#endregion

    #region Health
    private void CheckBossIsUnderHalf(float damage)
    {
        if(GetBossHealthPercentage() < .5f)
        {
            //Invokes any necessary functionality for reaching half health
            _myBossBase.InvokeBossHalfHealthEvent();
            IncreaseBossStatsAtHealthThreshholds();

            _myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderHalf);
            _myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderQuarter);

            CheckBossIsUnderQuarter(damage);
        }
    }

    private void CheckBossIsUnderQuarter(float damage)
    {
        if (GetBossHealthPercentage() < .25f)
        {
            //Invokes any necessary functionality for reaching quarter health
            _myBossBase.InvokeBossQuarterHealthEvent();
            //Increases boss stats
            IncreaseBossStatsAtHealthThreshholds();

            //No longer listens for damage event
            _myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderQuarter);
            //Causes the 10 percent health check to listen for the damage event
            _myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderTenth);

            CheckBossIsUnderTenth(damage);
        }
    }

    private void CheckBossIsUnderTenth(float damage)
    {
        if (GetBossHealthPercentage() < .1f)
        {
            //Invokes any necessary functionality for reaching tenth health
            _myBossBase.InvokeBossTenthHealthEvent();
            //Increases boss stats
            IncreaseBossStatsAtHealthThreshholds();

            //No longer listens for damage event
            _myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderTenth);
            //Causes the death check to listen for the damage event
            _myBossBase.GetBossDamagedEvent().AddListener(CheckIfBossIsDead);

            CheckIfBossIsDead(damage);
        }
    }

    private void CheckIfBossIsDead(float damage)
    {
        if (_currentHealth <= 0)
        {
            GameStateManager.Instance.SetGameplayState(EGameplayStates.PostBattleWon);
        }
    }

    protected virtual void IncreaseBossStatsAtHealthThreshholds()
    {
        MultiplyBossDamageMultiplier(_bossDamageIncrementMultiplier);
    }
    #endregion

    #region Stagger

    /// <summary>
    /// Checks if the boss is above their stagger cap
    /// If they are the boss becomes staggered
    /// </summary>
    private void CheckIfBossIsStaggered()
    {
        if (_currentStaggerCounter >= _bossDefaultStaggerMax)
        {
            BossStaggered();
        }
    }

    /// <summary>
    /// Staggers the boss and shoots out the event
    /// </summary>
    private void BossStaggered()
    {
        _bossStaggered = true;
        StopEnrageTimer();
        _myBossBase.InvokeBossStaggeredEvent();

        DecreaseBossDamageResistanceOnStagger();
    }

    /// <summary>
    /// Activated by event
    /// Occurs when the boss stagger has run out
    /// </summary>
    private void BossNoLongerStaggered()
    {
        _bossStaggered = false;
        StartEnrageTimer();
        _currentStaggerCounter = 0;

        IncreaseBossDamageResistanceAfterStagger();
    }

    #endregion

    #region Enrage
    /// <summary>
    /// Starts the enrage timer for the boss
    /// </summary>
    private void StartEnrageTimer()
    {
        // Checks if the boss is already enraged
        if (_isBossEnraged)
        {
            // Stop as we don't need to start the enrage timer if the boss is already enraged
            return;
        }
        
        // Checks if the enrage timer is already active
        if (_enrageCoroutine.IsUnityNull())
        {
            // Start the timer as the timer isn't active yet
            _enrageCoroutine = StartCoroutine(BossEnrageCounter());
        }
        else
        {
            Debug.LogWarning("Cannot start Enrage timer while it is already active");
        }
    }

    /// <summary>
    /// Stops the enrage timer
    /// </summary>
    private void StopEnrageTimer()
    {
        // Check if the enrage process is active
        if (!_enrageCoroutine.IsUnityNull())
        {
            // Stop the enrage timer
            StopCoroutine(_enrageCoroutine);
        }
    }

    /// <summary>
    /// The process of counting down until the boss enrages
    /// </summary>
    /// <returns></returns>
    private IEnumerator BossEnrageCounter()
    {
        while(_currentTimeUntilEnrage > 0)
        {
            _currentTimeUntilEnrage -= Time.deltaTime;
            yield return null;
        }

        EnrageMax();
    }

    /// <summary>
    /// Called when the boss reaches max enrage
    /// </summary>
    private void EnrageMax()
    {
        _bossEnrageDamageMultiplier = _storedEnrageMultiplier;
        _myBossBase.InvokeBossEnragedEvent();
    }
    #endregion

    #region Stat Changes

    /// <summary>
    /// Increases or decreases the current aggro value
    /// </summary>
    /// <param name="changeValue"></param>
    public void ChangeBossDamageResistance(float changeValue)
    {
        _bossDamageResistanceMultiplier+= changeValue;
    }

    private void DecreaseBossDamageResistanceOnStagger()
    {
        ChangeBossDamageResistance(-_bossDamageResistanceChangeOnStagger);
    }

    private void IncreaseBossDamageResistanceAfterStagger()
    {
        ChangeBossDamageResistance(_bossDamageResistanceChangeOnStagger);
    }
    #endregion

    #region Base Children Functionality
    /// <summary>
    /// Establishes the instance for the Boss Stats
    /// </summary>
    protected override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    public override void SubscribeToEvents()
    {
        _myBossBase.GetSOSetEvent().AddListener(BossSOAssigned);

        _myBossBase.GetBossNoLongerStaggeredEvent().AddListener(BossNoLongerStaggered);

        _myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderHalf);

        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(StartEnrageTimer);
    }
    #endregion

    #region Getters
    public float GetBossMaxHealth() => _bossMaxHealth;
    public float GetBossCurrentHealth() => _currentHealth;
    public float GetBossHealthPercentage() => _currentHealth / _bossMaxHealth;

    public float GetBossMaxStagger() => _bossDefaultStaggerMax;
    public float GetBossCurrentStaggerAmount() => _currentStaggerCounter;
    public float GetBossStaggerPercentage() => _currentStaggerCounter / _bossDefaultStaggerMax;

    public float GetStaggerDuration() => _bossStaggerDuration;

    public float GetBaseBossDamageMultiplier() => _baseBossDamageMultiplier;

    public float GetBossEnrageDamageMultiplier() => _bossEnrageDamageMultiplier;

    public float GetCombinedBossDamageMultiplier() => _baseBossDamageMultiplier * _bossEnrageDamageMultiplier;
    #endregion

    #region Setters
    public void DealDamageToBoss(float damage)
    {
        damage /= _bossDamageResistanceMultiplier;
        _currentHealth -= damage;
        _myBossBase.InvokeBossDamagedEvent(damage);
    }

    public void DealStaggerToBoss(float stagger)
    {
        // Stop if the boss is already staggered
        if (_bossStaggered)
        {
            return;
        }

        _currentStaggerCounter += stagger;
        _myBossBase.InvokeBossStaggerDealt(stagger);
        CheckIfBossIsStaggered();
    }

    public void MultiplyBossDamageMultiplier(float amount)
    {
        _baseBossDamageMultiplier *= amount;
    }
    #endregion
}
