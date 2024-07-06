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

    private bool _bossStaggered = false;

    private float _bossDamageMultiplier = 1;

   

    private void StatsSetup(BossSO bossSO)
    {
        //Set the boss max health to be the max hp from the SO and multiplied by the difficulty modifier
        _bossMaxHealth = bossSO.GetMaxHP() * 
            UniversalManagers.Instance.GetSelectionManager().GetHealthMultiplierFromDifficulty();
        _bossDefaultStaggerMax = bossSO.GetBaseStaggerMax() *
            UniversalManagers.Instance.GetSelectionManager().GetStaggerMultiplierFromDifficulty();

        _currentHealth = _bossMaxHealth;
        _currentStaggerCounter = 0;

        _bossStaggerDuration = bossSO.GetStaggerDuration();

        _bossDamageIncrementMultiplier = bossSO.GetBossDamageIncrementMultiplier();

        _bossDamageMultiplier = UniversalManagers.Instance.
                GetSelectionManager().GetDamageMultiplierFromDifficulty();
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
        myBossBase.InvokeBossStaggeredEvent();
    }

    /// <summary>
    /// Activated by event
    /// Occurs when the boss stagger has run out
    /// </summary>
    private void BossNoLongerStaggered()
    {
        _bossStaggered = false;
        _currentStaggerCounter = 0;
    }

    private void CheckBossIsUnderHalf(float damage)
    {
        if(GetBossHealthPercentage() < .5f)
        {
            myBossBase.InvokeBossHalfHealthEvent();
            IncreaseBossStatsAtHealthThreshholds();

            myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderHalf);
            myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderQuarter);

            CheckBossIsUnderQuarter(damage);
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

            CheckBossIsUnderTenth(damage);
        }
    }

    private void CheckBossIsUnderTenth(float damage)
    {
        if (GetBossHealthPercentage() < .1f)
        {
            myBossBase.InvokeBossTenthHealthEvent();
            IncreaseBossStatsAtHealthThreshholds();

            myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderTenth);
            myBossBase.GetBossDamagedEvent().AddListener(CheckIfBossIsDead);

            CheckIfBossIsDead(damage);
        }
    }

    private void CheckIfBossIsDead(float damage)
    {
        if (_currentHealth <= 0)
        {
            GameplayManagers.Instance.GetGameStateManager().SetGameplayState(GameplayStates.PostBattleWon);
        }
    }

    protected virtual void IncreaseBossStatsAtHealthThreshholds()
    {
        myBossBase.GetBossStats().MultiplyBossDamageMultiplier(_bossDamageIncrementMultiplier);
    }

    #region Events
    public override void SubscribeToEvents()
    {
        myBossBase.GetSOSetEvent().AddListener(BossSOAssigned);

        myBossBase.GetBossNoLongerStaggeredEvent().AddListener(BossNoLongerStaggered);

        myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderHalf);
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

    public float GetBossDamageMultiplier() => _bossDamageMultiplier;
    #endregion

    #region Setters
    public void DealDamageToBoss(float damage)
    {
        _currentHealth -= damage;
        myBossBase.InvokeBossDamagedEvent(damage);
    }

    public void DealStaggerToBoss(float stagger)
    {
        if (_bossStaggered) return;

        _currentStaggerCounter += stagger;
        myBossBase.InvokeBossStaggerDealt(stagger);
        CheckIfBossIsStaggered();
    }

    public void MultiplyBossDamageMultiplier(float amount)
    {
        _bossDamageMultiplier *= amount;
    }
    #endregion
}
