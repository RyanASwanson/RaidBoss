using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_VoidMaw : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private bool _doesSpawnRay;
    private SBP_RayOfHopeTargetZone _associatedRay;
    
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _voidMaw;
    
    private SBP_VoidMawTargetZone _newestTargetZone;
    
    private void SpawnRay()
    {
        _associatedRay = SB_VoidLord.Instance.SpawnRayOfHopeTargetZone(transform.position);
    }

    public void MawsReachedEnd()
    {
        if (!_associatedRay.IsUnityNull())
        {
            _associatedRay.SpawnRayOfHope();
        }
    }
    
    #region Base Ability

    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
    }

    protected override void StartShowTargetZone()
    {
        if (_doesSpawnRay)
        {
            SpawnRay();
        }
        
        _newestTargetZone = Instantiate(_targetZone, _specificAreaTarget, Quaternion.identity)
            .GetComponent<SBP_VoidMawTargetZone>();
        
        _newestTargetZone.TargetZoneSetUp(_storedTarget,_associatedRay);
        _currentTargetZones.Add(_newestTargetZone.GetComponent<BossTargetZoneParent>());
        
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        base.AbilityStart();
        
        _newestTargetZone.StopVoidMawTargetTracking();
        
        SBP_VoidMaw voidMaw = Instantiate(_voidMaw, _specificLookTarget, Quaternion.identity).GetComponent<SBP_VoidMaw>();
        voidMaw.SetUpProjectile(_myBossBase,_abilityID);
        voidMaw.AdditionalSetUp(this, _newestTargetZone.GetClosestCorner(), _newestTargetZone.GetHeroDistance());
    }
    
    protected override void AbilityDurationEnded()
    {
        base.AbilityDurationEnded();
    }

    public override void StopBossAbility()
    {
        base.StopBossAbility();
        _associatedRay.RemoveRay();
    }
    #endregion
}
