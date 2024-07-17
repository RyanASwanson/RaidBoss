using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Tremor : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private Vector3 _targetLocation;

    [SerializeField] private GameObject _tremor;
    [SerializeField] private GameObject _targetZone;

    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _targetLocation, Quaternion.identity));
        base.StartShowTargetZone();
    }

    protected override void AbilityStart()
    {
        Instantiate(_tremor, _targetLocation, Quaternion.identity);
        base.AbilityStart();
    }
}
