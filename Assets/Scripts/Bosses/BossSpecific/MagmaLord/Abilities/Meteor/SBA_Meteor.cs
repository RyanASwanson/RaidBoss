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

    private Vector3 _storedHeroLocation;

    private GameObject _storedFallingMeteor;
    private SBP_FallingMeteor _storedFallingMeteorFunc;
    private GameObject _storedMovingMeteor;

    
    /// <summary>
    /// Waits until the hero that is being targetted is not directly on top of the target zone
    /// </summary>
    /// <param name="targetZone"></param>
    /// <returns></returns>
    protected IEnumerator HideTargetZoneUntilNonZero(GameObject targetZone)
    {
        Vector3 startingLookLocation = _myBossBase.gameObject.transform.position;
        
        targetZone.transform.LookAt(startingLookLocation);
        targetZone.transform.eulerAngles = new Vector3(0, targetZone.transform.eulerAngles.y, 0);

        //If that direction is close to zero choose a random direction instead
        while (!targetZone.IsUnityNull() && !_storedTarget.IsUnityNull() &&
               (Mathf.Abs(targetZone.transform.position.x - _storedTarget.transform.position.x) < .1f &&
                Mathf.Abs(targetZone.transform.position.z - _storedTarget.transform.position.z) < .1f))
        {
            yield return null;
        }

        if (!targetZone.IsUnityNull())
        {
            StartCoroutine(FollowingDirectionalTargetZone(targetZone));
        }
    }
    
    protected IEnumerator FollowingDirectionalTargetZone(GameObject targetZone)
    {
        while(!targetZone.IsUnityNull())
        {
            if (!_storedTarget.IsUnityNull())
            {
                _storedHeroLocation = _storedTarget.transform.position;
            }
            targetZone.transform.LookAt(_storedHeroLocation);
            targetZone.transform.eulerAngles = new Vector3(0, targetZone.transform.eulerAngles.y, 0);
            yield return null;
        }
    }

    /// <summary>
    /// Occurs the moment the meteor makes contact with the ground
    /// </summary>
    protected void FallingMeteorContact()
    {
        if (!_storedFallingMeteor.IsUnityNull())
        {
            _storedFallingMeteorFunc.FloorContact();
            Destroy(_storedFallingMeteor);
        }
    }

    #region Base Ability
    /// <summary>
    /// Displays the target zone of the ability
    /// </summary>
    protected override void StartShowTargetZone()
    {
        BossTargetZoneParent targetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        _currentTargetZones.Add(targetZone);
        
        _storedHeroLocation = _myBossBase.gameObject.transform.position;
        StartCoroutine(HideTargetZoneUntilNonZero(targetZone.GetBossTargetZones()[0].GetAdditionalGameObjectReferences()[0]));

        base.StartShowTargetZone();
    }

    /// <summary>
    /// Starts the wind up of the ability
    /// </summary>
    protected override void StartAbilityWindUp()
    {
        _storedFallingMeteor = Instantiate(_fallingMeteor, _storedTargetLocation, _fallingMeteor.transform.rotation);
        _storedFallingMeteorFunc = _storedFallingMeteor.GetComponent<SBP_FallingMeteor>();
        _storedFallingMeteorFunc.SetUpProjectile(_myBossBase,_abilityID);
        _storedFallingMeteorFunc.AdditionalSetUp(_storedTarget.gameObject);

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
            _storedMovingMeteor.GetComponent<SBP_FollowingMeteor>().AdditionalSetUp(_storedHeroLocation);
        }
        
        base.AbilityStart();
    }

    public override void StopBossAbility()
    {
        base.StopBossAbility();
        if (!_storedFallingMeteor.IsUnityNull())
        {
            _storedFallingMeteorFunc.StopFallingMeteor();
        }
    }
    #endregion
}
