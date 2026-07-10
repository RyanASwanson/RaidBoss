using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Isolation : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _isolation;
    
    #region Base Ability

    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
    }

    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        base.AbilityStart();
    }
    
    protected override void AbilityDurationEnded()
    {
        base.AbilityDurationEnded();
    }

    public override void StopBossAbility()
    {
        base.StopBossAbility();
    }
    #endregion
}
