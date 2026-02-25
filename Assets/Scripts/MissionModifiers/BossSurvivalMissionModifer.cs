using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSurvivalMissionModifer : MissionModifierBase
{
    [SerializeField] private float _bossHealthMultiplier = 1;
    [SerializeField] private float _bossStaggerMultiplier = 1;
    [SerializeField] private float _bossDamageResistanceChangeOnStaggerMultiplier = 1;
    
    #region BaseMissionModifier
    public override void SetUpMissionModifier()
    {
        
    }
    
    public override void AdjustBossStatsModifier(BossStats bossStats)
    {
        base.AdjustBossStatsModifier(bossStats);

        bossStats.SetBossMaxHealth(bossStats.GetBossMaxHealth() * _bossHealthMultiplier);
        bossStats.SetBossMaxStagger(bossStats.GetBossMaxStagger() * _bossStaggerMultiplier);
        bossStats.SetBossDamageResistanceChangeOnStagger
            (bossStats.GetBossDamageResistanceChangeOnStagger() * _bossDamageResistanceChangeOnStaggerMultiplier);
    }
    #endregion
}
