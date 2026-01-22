using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Overcharge : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _overChargeTargetZone;

    protected override void AbilityPrep()
    {
        base.AbilityPrep();
    }
    
    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();
        BossTargetZoneParent targetZone = Instantiate(_overChargeTargetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        _currentTargetZones.Add(targetZone);
        SB_ThunderLord.Instance.ChildGameObjectToImpendingStorm(targetZone.gameObject);
    }

    protected override void AbilityStart()
    {
        base.AbilityStart();
        SB_ThunderLord.Instance.GetImpendingStorm().ActivateOvercharge();
    }
}
