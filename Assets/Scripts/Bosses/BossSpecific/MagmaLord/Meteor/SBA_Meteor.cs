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
    [SerializeField] private GameObject _movingMeteor;
    [SerializeField] private GameObject _targetZone;

    [SerializeField] private Vector3 _fallingMeteorAngleVariance;

    private GameObject _storedFallingMeteor;

    public override void AbilityPrep()
    {
        base.AbilityPrep();
    }

    public override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity));
        base.StartShowTargetZone();
    }

    public override void StartAbilityWindUp()
    {
        _storedFallingMeteor = Instantiate(_fallingMeteor, _storedTargetLocation, Quaternion.identity);
        base.StartAbilityWindUp();
    }

    public override void AbilityStart()
    {
        Destroy(_storedFallingMeteor);

        Instantiate(_movingMeteor, _storedTargetLocation, Quaternion.identity);
        base.AbilityStart();
    }
}
