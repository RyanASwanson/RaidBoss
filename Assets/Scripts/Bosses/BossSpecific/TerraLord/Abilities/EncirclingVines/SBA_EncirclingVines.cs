using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Terra Lord's Encircling Vines ability
/// </summary>
public class SBA_EncirclingVines : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _encirclingVines;

    private BossTargetZoneParent _newestTargetZone;
    
    public const int ENCIRCLING_VINES_END_AUDIO_ID = 0;

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
        SBP_EncirclingVines newestVines = 
            Instantiate(_encirclingVines, _newestTargetZone.transform.position, Quaternion.identity).GetComponent<SBP_EncirclingVines>();
        
        newestVines.SetUpProjectile(_myBossBase,_abilityID,_wasBossEnragedOnAbilityActivation);
        
        if (!_storedTarget.IsUnityNull())
        {
            newestVines.AdditionalSetUp(_storedTarget.gameObject);
        }
        else
        {
            newestVines.AdditionalSetUp();
        }
        
        base.AbilityStart();
    }
    #endregion
}
