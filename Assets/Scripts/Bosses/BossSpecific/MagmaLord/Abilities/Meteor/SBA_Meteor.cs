using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Magma Lord's Meteor ability
/// </summary>
public class SBA_Meteor : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _fallingMeteor;
    [SerializeField] private GameObject _movingMeteor;

    [SerializeField] private GameObject _directionalTargetZone;

    //[SerializeField] private Vector3 _fallingMeteorAngleVariance;

    private GameObject _storedFallingMeteor;
    private GameObject _storedMovingMeteor;

    
    protected IEnumerator FollowingDirectionalTargetZone(GameObject targetZone)
    {
        StartCoroutine(HideTargetZoneUntilNonZero(targetZone));

        while(!targetZone.IsUnityNull() && !_storedTarget.IsUnityNull())
        {
            targetZone.transform.LookAt(_storedTarget.transform.position);
            targetZone.transform.eulerAngles = new Vector3(0, targetZone.transform.eulerAngles.y, 0);
            yield return null;
        }
    }

    /// <summary>
    /// Waits until the hero that is being targetted is not directly on top of the target zone
    /// </summary>
    /// <param name="targetZone"></param>
    /// <returns></returns>
    protected IEnumerator HideTargetZoneUntilNonZero(GameObject targetZone)
    {
        targetZone.SetActive(false);

        //If that direction is close to zero choose a random direction instead
        while (targetZone.IsUnityNull() &&
               (Mathf.Abs(targetZone.transform.position.x - _storedTarget.transform.position.x) < .1f &&
                Mathf.Abs(targetZone.transform.position.z - _storedTarget.transform.position.z) < .1f))
        {
            yield return null;
        }

        if (!targetZone.IsUnityNull())
        {
            targetZone.SetActive(true);
        }
    }

    /// <summary>
    /// Occurs the moment the meteor makes contact with the ground
    /// </summary>
    protected void FallingMeteorContact()
    {
        _storedFallingMeteor.GetComponent<SBP_FallingMeteor>().FloorContact();
        Destroy(_storedFallingMeteor);
    }

    #region Base Ability
    /// <summary>
    /// Displays the target zone of the ability
    /// </summary>
    protected override void StartShowTargetZone()
    {
        GameObject targetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity);
        _currentTargetZones.Add(targetZone);

        StartCoroutine(FollowingDirectionalTargetZone(targetZone.GetComponentInChildren<BossTargetZone>().GetAdditionalGameObjectReferences()[0]));

        base.StartShowTargetZone();
    }

    /// <summary>
    /// Starts the wind up of the ability
    /// </summary>
    protected override void StartAbilityWindUp()
    {
        _storedFallingMeteor = Instantiate(_fallingMeteor, _storedTargetLocation, _fallingMeteor.transform.rotation);
        _storedFallingMeteor.GetComponent<SBP_FallingMeteor>().AdditionalSetUp(_storedTarget.gameObject);

        base.StartAbilityWindUp();
    }

    /// <summary>
    /// Starts teh ability
    /// </summary>
    protected override void AbilityStart()
    {
        if (!BossStats.Instance.GetIsBossStaggered())
        {
            FallingMeteorContact();

            _storedTargetLocation = new Vector3(_storedTargetLocation.x, -.3f, _storedTargetLocation.z);
            _storedMovingMeteor = Instantiate(_movingMeteor, _storedTargetLocation, Quaternion.identity);
            _storedMovingMeteor.GetComponent<SBP_FollowingMeteor>().AdditionalSetUp(_storedTarget);
        }
        
        base.AbilityStart();
    }
    #endregion
}
