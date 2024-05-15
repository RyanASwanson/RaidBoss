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

    private GameObject _storedScorch;

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
        _storedScorch = Instantiate(_scorch, _targetLocation, Quaternion.identity);
        //_storedMovingMeteor.GetComponent<SBP_FollowingMeteor>().AdditionalSetup(_storedTarget);

        Destroy(_storedScorch, 2);

        base.AbilityStart();
    }
}
