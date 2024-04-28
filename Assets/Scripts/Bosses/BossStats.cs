using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : BossChildrenFunctionality
{
    private float _bossMaxHealth;
    private float _currentHealth;

    private float _bossDefaultStaggerCounter;
    private float _currentStaggerCounter;


    private void StatsSetup(BossSO bossSO)
    {
        _bossMaxHealth = bossSO.GetMaxHP();

        _currentHealth = _bossMaxHealth;
    }

    private void CheckIfBossIsDead()
    {
        if(_currentHealth <= 0)
        {
            Debug.Log("BossDead");
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

    #endregion

    #region Setters
    public void DealDamageToBoss(float damage)
    {
        _currentHealth -= damage;
        CheckIfBossIsDead();
    }
    #endregion
}
