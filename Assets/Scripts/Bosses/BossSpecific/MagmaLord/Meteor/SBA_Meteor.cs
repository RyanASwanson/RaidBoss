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
    private GameObject _storedMovingMeteor;

    protected override void AbilityPrep()
    {
        base.AbilityPrep();
    }

    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity));
        base.StartShowTargetZone();
    }

    protected override void StartAbilityWindUp()
    {
        _storedFallingMeteor = Instantiate(_fallingMeteor, _storedTargetLocation, _fallingMeteor.transform.rotation);
        StartCoroutine(LookAtTarget());
        base.StartAbilityWindUp();
    }

    private IEnumerator LookAtTarget()
    {
        while (_storedTarget != null && _storedFallingMeteor != null)
        {
            _storedFallingMeteor.transform.LookAt(_storedTarget.transform.position);
            _storedFallingMeteor.transform.eulerAngles = new Vector3(0, _storedFallingMeteor.transform.eulerAngles.y, 0);
            yield return null;
        }
    }

    protected override void AbilityStart()
    {
        Destroy(_storedFallingMeteor);

        _storedTargetLocation = new Vector3(_storedTargetLocation.x, -.3f, _storedTargetLocation.z);
        _storedMovingMeteor = Instantiate(_movingMeteor, _storedTargetLocation, Quaternion.identity);
        _storedMovingMeteor.GetComponent<SBP_FollowingMeteor>().AdditionalSetup(_storedTarget);
        base.AbilityStart();
    }
}
