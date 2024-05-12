using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Magma lord attack
/// Create a damage target zone then a meteor falls on that location
///     does initial damage and creates a damaging zone
/// </summary>
public class SBA_Meteor : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _fallingMeteor;
    [SerializeField] private GameObject _targetZone;

    public override void AbilityPrep(Vector3 targetLocation)
    {
        Debug.Log("SpawnMeteor");
        Instantiate(_fallingMeteor, targetLocation, Quaternion.identity);
        base.AbilityPrep(targetLocation);
    }

    public override void StartShowTargetZone(Vector3 targetLocation)
    {
        _currentTargetZones.Add(Instantiate(_targetZone, targetLocation, Quaternion.identity));
        base.StartShowTargetZone(targetLocation);
    }

    public override void StartAbilityWindUp(Vector3 targetLocation)
    {
        Instantiate(_fallingMeteor, targetLocation, Quaternion.identity);
        base.StartAbilityWindUp(targetLocation);
    }
}
