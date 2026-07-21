using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_VoidMaw : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _voidMaw;
    
    private SBP_VoidMawTargetZone _newestTargetZone;
    
    #region Base Ability

    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
    }

    protected override void StartShowTargetZone()
    {
        _newestTargetZone = Instantiate(_targetZone, _specificAreaTarget, Quaternion.identity)
            .GetComponent<SBP_VoidMawTargetZone>();
        
        _newestTargetZone.TargetZoneSetUp(_storedTarget);
        _currentTargetZones.Add(_newestTargetZone.GetComponent<BossTargetZoneParent>());
        
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        base.AbilityStart();
        SBP_VoidMaw voidMaw = Instantiate(_voidMaw, _specificLookTarget, Quaternion.identity).GetComponent<SBP_VoidMaw>();
        voidMaw.SetUpProjectile(_myBossBase,_abilityID);
        voidMaw.AdditionalSetUp(_storedTarget, _newestTargetZone.StopVoidMawTargetTracking());
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
