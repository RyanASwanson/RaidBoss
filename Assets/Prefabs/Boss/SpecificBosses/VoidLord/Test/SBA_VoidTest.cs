using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_VoidTest : SpecificBossAbilityFramework
{
    [Space] 
    [SerializeField] private float _spawnForwardOffset;
    [SerializeField] private Vector3[] _attackDirections;
    
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _voidMaw;
    
    private FollowObject _newestTargetZone;
    
    #region Base Ability

    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
    }

    protected override void StartShowTargetZone()
    {
        _newestTargetZone = Instantiate(_targetZone, _specificAreaTarget, Quaternion.identity)
            .GetComponent<FollowObject>();
        
        //_newestTargetZone.TargetZoneSetUp(_storedTarget);
        _newestTargetZone.StartFollowingObject(_storedTarget.gameObject);
        _currentTargetZones.Add(_newestTargetZone.GetComponent<BossTargetZoneParent>());
        
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        base.AbilityStart();
        
        Vector3 edgeOfMap =  EnvironmentManager.Instance.GetEdgeOfMapLoc(transform.position,
            (transform.forward).normalized);

        foreach (Vector3 direction in _attackDirections)
        {
            edgeOfMap =  EnvironmentManager.Instance.GetEdgeOfMapLoc(_storedTarget.transform.position,
                direction.normalized);

            edgeOfMap += (direction * _spawnForwardOffset);
            
            GeneralTranslate newestProjectile = Instantiate(_voidMaw,edgeOfMap + _specificAreaTarget,Quaternion.identity).GetComponent<GeneralTranslate>();
            
            //_newestTargetZone.transform.localEulerAngles = direction * -1;
            newestProjectile.transform.LookAt(_storedTarget.transform.position);
            newestProjectile.transform.localEulerAngles = new Vector3(0f, newestProjectile.transform.localEulerAngles.y, 0f);
            
            newestProjectile.StartMovingForwards();
        }
        
        /*SBP_VoidMaw voidMaw = Instantiate(_voidMaw, _specificLookTarget, Quaternion.identity).GetComponent<SBP_VoidMaw>();
        voidMaw.SetUpProjectile(_myBossBase,_abilityID);
        voidMaw.AdditionalSetUp(_storedTarget, _newestTargetZone.StopVoidMawTargetTracking());*/
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
