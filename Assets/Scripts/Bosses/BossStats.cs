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


    private void StatsSetup(BossSO bossSO)
    {
        _bossMaxHealth = bossSO.GetMaxHP();
        _bossDefaultStaggerMax = bossSO.GetBaseStaggerMax();

        _currentHealth = _bossMaxHealth;
        _currentStaggerCounter = 0;
    }

    private void CheckIfBossIsDead()
    {
        if(_currentHealth <= 0)
        {
            Debug.Log("BossDead");
        }
    }

    private void CheckIfBossIsStaggered()
    {
        if (_currentStaggerCounter >= _bossDefaultStaggerMax)
        {
            Debug.Log("Boss Staggered");
        }
    }
    
    #region Events
    public override void SubscribeToEvents()
    {
        myBossBase.GetSOSetEvent().AddListener(BossSOAssigned);
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
    #endregion

    #region Setters
    public void DealDamageToBoss(float damage)
    {
        _currentHealth -= damage;
        myBossBase.InvokeBossDamagedEvent(damage);
        CheckIfBossIsDead();
    }

    public void DealStaggerToBoss(float stagger)
    {
        _currentStaggerCounter += stagger;
        myBossBase.InvokeBossStaggerDealt(stagger);
        CheckIfBossIsStaggered();
    }
    #endregion
}
