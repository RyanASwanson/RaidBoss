using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_StaticCharge : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _staticChargeObject;

    private BossTargetZoneParent _newestTargetZone;
    
    public const int STATIC_CHARGE_ATTACK_HIT_AUDIO_ID = 0;
    
    
    #region Base Ability
    
    /// <summary>
    /// Starts displaying the target zone for the ability
    /// </summary>
    protected override void StartShowTargetZone()
    {
        bool hasHeroTarget = _mySpecificBoss.GetBossAttackTargets().Count > 1;
        
        if (!hasHeroTarget)
        {
            _storedTargetLocation = _myBossBase.gameObject.transform.position;

            _storedTarget = null;
        }
        
        //Spawns the target area
        _newestTargetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        
        //Adds the target area to the list of target areas
        _currentTargetZones.Add(_newestTargetZone);
        
        FollowObject followTargetZone = _newestTargetZone.GetComponent<FollowObject>();
        if (hasHeroTarget)
        {
            //Makes the target area follow the hero that is being targeted
            followTargetZone.StartFollowingObject(_storedTarget.gameObject);
        }
        else
        {
            followTargetZone.SetFollowLocationOffset(_specificAreaTarget);
            followTargetZone.StartFollowingObject(_myBossBase.gameObject);
        }

        base.StartShowTargetZone();
    }

    /// <summary>
    /// Starts the ability
    /// </summary>
    protected override void AbilityStart()
    {
        //Spawns the damaging ability
        GameObject newestStaticCharge = Instantiate(_staticChargeObject, _newestTargetZone.transform.position, Quaternion.identity);

        if (newestStaticCharge.TryGetComponent(out SBP_StaticCharge staticCharge))
        {
            staticCharge.SetUpProjectile(_myBossBase, _abilityID);
            staticCharge.AdditionalSetUp(_storedTarget);
        }
        
        FollowObject followStaticCharge = newestStaticCharge.GetComponent<FollowObject>();
        if (!_storedTarget.IsUnityNull())
        {
            followStaticCharge.StartFollowingObject(_storedTarget.gameObject);
        }
        else
        {
            followStaticCharge.transform.position -= _specificAreaTarget;
        }
        
        base.AbilityStart();
    }
    #endregion
}
