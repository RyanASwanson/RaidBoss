using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_StaticCharge : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _staticChargeObject;

    private BossTargetZoneParent _newestTargetZone;
    
    public const int STATIC_CHARGE_ATTACK_HIT_AUDIO_ID = 0;

    private void StartFollowingBossTarget(GameObject followGameObject)
    {
        if (followGameObject.TryGetComponent(out FollowObject followObject))
        {
            followObject.StartFollowingObject(_storedTarget.gameObject);
        }
    }
    
    #region Base Ability
    
    /// <summary>
    /// Starts displaying the target zone for the ability
    /// </summary>
    protected override void StartShowTargetZone()
    {
        //Spawns the target area
        _newestTargetZone = Instantiate(_targetZone, Vector3.zero, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        
        StartFollowingBossTarget(_newestTargetZone.gameObject);
        
        //Adds the target area to the list of target areas
        _currentTargetZones.Add(_newestTargetZone);

        base.StartShowTargetZone();
    }

    /// <summary>
    /// Starts the ability
    /// </summary>
    protected override void AbilityStart()
    {
        //Spawns the damaging ability
        GameObject newestStaticCharge = Instantiate(_staticChargeObject, Vector3.zero, Quaternion.identity);

        if (newestStaticCharge.TryGetComponent(out SBP_StaticCharge staticCharge))
        {
            staticCharge.SetUpProjectile(_myBossBase, _abilityID);
            staticCharge.AdditionalSetUp(_storedTarget);
        }
        
        StartFollowingBossTarget(newestStaticCharge);
        
        base.AbilityStart();
    }
    #endregion
}
