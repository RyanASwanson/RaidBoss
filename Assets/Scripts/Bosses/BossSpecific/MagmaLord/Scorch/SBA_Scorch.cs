using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Magma lord attack
/// Deals damage in an area around the boss
/// </summary>
public class SBA_Scorch : SpecificBossAbilityFramework
{
    [SerializeField] private Vector3 _targetLocation;

    [SerializeField] private GameObject _scorch;
    [SerializeField] private GameObject _targetZone;

    protected override void AbilityPrep()
    {
        base.AbilityPrep();
    }

    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _targetLocation, Quaternion.identity));
        base.StartShowTargetZone();
    }

    protected override void StartAbilityWindUp()
    {
        base.StartAbilityWindUp();
    }

    protected override void AbilityStart()
    {
        Instantiate(_scorch, _targetLocation, Quaternion.identity);

        base.AbilityStart();
    }
}
