using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all general stats that bosses have as well as the functionality for
///     affecting those stats
/// Holds health and stagger
/// </summary>
public class BossStats : BossChildrenFunctionality
{
    private float _bossMaxHealth;
    private float _currentHealth;

    private float _bossDefaultStaggerMax;
    private float _currentStaggerCounter;

    private float _bossStaggerDuration;

    private float _bossDamageIncrementMultiplier;
    private float _bossDamageResistanceChangeOnStagger;

    private bool _bossStaggered = false;

    private float _baseBossDamageMultiplier = 1;
    private float _bossDamageResistanceMultiplier =1;

    private float _bossEnrageDamageMultiplier = 1;
    private float _storedEnrageMultiplier;

    private float _currentTimeUntilEnrage;
    private Coroutine _enrageCoroutine;

    private void StatsSetup(BossSO bossSO)
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

        DecreaseBossDamageResOnStagger();
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

        IncreaseBossDamageResAfterStagger();
    }

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
        _myBossBase.GetBossStats().MultiplyBossDamageMultiplier(_bossDamageIncrementMultiplier);
    }

    #region Enrage
    private void StartEnrageTimer()
    {
        if (_enrageCoroutine == null)
        {
            _enrageCoroutine = StartCoroutine(BossEnrageCounter());
        }
    }

    private void StopEnrageTimer()
    {
        if (_enrageCoroutine != null)
        {
            StopCoroutine(_enrageCoroutine);
        }
    }

    private IEnumerator BossEnrageCounter()
    {
        while(_currentTimeUntilEnrage > 0)
        {
            _currentTimeUntilEnrage -= Time.deltaTime;
            yield return null;
        }

        EnrageMax();
    }

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

    private void DecreaseBossDamageResOnStagger()
    {
        ChangeBossDamageResistance(-_bossDamageResistanceChangeOnStagger);
    }

    private void IncreaseBossDamageResAfterStagger()
    {
        ChangeBossDamageResistance(_bossDamageResistanceChangeOnStagger);
    }
    #endregion

    #region Events
    public override void SubscribeToEvents()
    {
        _myBossBase.GetSOSetEvent().AddListener(BossSOAssigned);

        _myBossBase.GetBossNoLongerStaggeredEvent().AddListener(BossNoLongerStaggered);

        _myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderHalf);

        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(StartEnrageTimer);
    }

    private void BossSOAssigned(BossSO bossSO)
    {
        StatsSetup(bossSO);
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
        if (_bossStaggered) return;

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
