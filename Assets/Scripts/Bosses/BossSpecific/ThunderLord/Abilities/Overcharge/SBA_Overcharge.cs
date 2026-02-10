using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Overcharge : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _overchargeTargetZone;
    [SerializeField] private GameObject _overchargeTargetVFX;
    
    
    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();
        BossTargetZoneParent targetZone = Instantiate(_overchargeTargetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        _currentTargetZones.Add(targetZone);
        SB_ThunderLord.Instance.ChildGameObjectToImpendingStorm(targetZone.gameObject);
        
        GameObject overchargeTargetVFX = Instantiate(_overchargeTargetVFX, _storedTargetLocation,Quaternion.identity);
        SB_ThunderLord.Instance.ChildGameObjectToImpendingStorm(overchargeTargetVFX);
        
        BossVisuals.Instance.BossLookAt(targetZone.GetBossTargetZones()[0].GetAdditionalGameObjectReferences()[0], _abilityWindUpTime);
    }

    protected override void AbilityStart()
    {
        base.AbilityStart();
        SB_ThunderLord.Instance.GetImpendingStorm().ActivateOvercharge();
    }
}
