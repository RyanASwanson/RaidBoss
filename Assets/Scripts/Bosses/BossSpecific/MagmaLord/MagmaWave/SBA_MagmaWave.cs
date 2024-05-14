using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Magma lord attack
/// Create a damage target zone at the edge of the arena in the direction of a target
///     then has a projectile move from that point to the boss
/// </summary>
public class SBA_MagmaWave : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _magmaWave;
    [SerializeField] private GameObject _targetZone;

    private GameObject _storedMagmaWave;

    public override void AbilityPrep()
    {
        base.AbilityPrep();
    }

    public override void StartShowTargetZone()
    {
        GameObject newTargetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity);
        newTargetZone.transform.LookAt(_ownerBossBase.transform);
        _currentTargetZones.Add(newTargetZone);
        base.StartShowTargetZone();
    }

    public override void StartAbilityWindUp()
    {
        base.StartAbilityWindUp();
    }

    public override void AbilityStart()
    {
        //_storedMovingMeteor = Instantiate(_movingMeteor, _storedTargetLocation, Quaternion.identity);
        //_storedMovingMeteor.GetComponent<SBP_FollowingMeteor>().AdditionalSetup(_storedTarget);
        base.AbilityStart();
    }
}
