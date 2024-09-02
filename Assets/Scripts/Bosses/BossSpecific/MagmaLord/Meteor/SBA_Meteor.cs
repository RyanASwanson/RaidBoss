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
    [Space]
    [SerializeField] private GameObject _fallingMeteor;
    [SerializeField] private GameObject _movingMeteor;
    [SerializeField] private GameObject _targetZone;

    [SerializeField] private GameObject _directionalTargetZone;

    //[SerializeField] private Vector3 _fallingMeteorAngleVariance;

    private GameObject _storedFallingMeteor;
    private GameObject _storedMovingMeteor;

    protected override void AbilityPrep()
    {
        base.AbilityPrep();
    }

    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity));

        GameObject newDirectionalTargetZone = Instantiate(_directionalTargetZone, 
            _storedTargetLocation, Quaternion.identity);

        _currentTargetZones.Add(newDirectionalTargetZone);

        StartCoroutine(FollowingDirectionalTargetZone(newDirectionalTargetZone));

        base.StartShowTargetZone();
    }

    protected override void StartAbilityWindUp()
    {
        _storedFallingMeteor = Instantiate(_fallingMeteor, _storedTargetLocation, _fallingMeteor.transform.rotation);
        _storedFallingMeteor.GetComponent<SBP_FallingMeteor>().AdditionalSetup(_storedTarget.gameObject);

        base.StartAbilityWindUp();
    }

    protected IEnumerator FollowingDirectionalTargetZone(GameObject targetZone)
    {
        StartCoroutine(HideTargetZoneUntilNonZero(targetZone));

        while(targetZone != null && _storedTarget != null)
        {
            targetZone.transform.LookAt(_storedTarget.transform.position);
            targetZone.transform.eulerAngles = new Vector3(0, targetZone.transform.eulerAngles.y, 0);
            yield return null;
        }
    }

    protected IEnumerator HideTargetZoneUntilNonZero(GameObject targetZone)
    {
        targetZone.SetActive(false);

        //If that direction is close to zero choose a random direction instead
        while (targetZone != null &&
            (Mathf.Abs(targetZone.transform.position.x - _storedTarget.transform.position.x) < .1f &&
            Mathf.Abs(targetZone.transform.position.z - _storedTarget.transform.position.z) < .1f))
            yield return null;

        if (targetZone != null)
            targetZone.SetActive(true);
    }

    protected void FallingMeteorContact()
    {
        _storedFallingMeteor.GetComponent<SBP_FallingMeteor>().FloorContact();
        Destroy(_storedFallingMeteor);
    }


    protected override void AbilityStart()
    {
        FallingMeteorContact();

        _storedTargetLocation = new Vector3(_storedTargetLocation.x, -.3f, _storedTargetLocation.z);
        _storedMovingMeteor = Instantiate(_movingMeteor, _storedTargetLocation, Quaternion.identity);
        _storedMovingMeteor.GetComponent<SBP_FollowingMeteor>().AdditionalSetup(_storedTarget);
        base.AbilityStart();
    }

    #region Base Ability

    #endregion
}
