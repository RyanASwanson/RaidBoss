using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_DespairHex : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _despairHex;
    
    private List<HeroBase> _hexedHeroes = new List<HeroBase>();
    
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
