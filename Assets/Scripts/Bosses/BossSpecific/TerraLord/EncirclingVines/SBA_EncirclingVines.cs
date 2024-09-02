using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_EncirclingVines : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private float _yLoc;

    [Space]
    [SerializeField] private GameObject _encirclingVines;
    [SerializeField] private GameObject _targetZone;

    private GameObject _newestTargetZone;

    protected override void StartShowTargetZone()
    {
        _newestTargetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity);
        _currentTargetZones.Add(_newestTargetZone);

        StartCoroutine(FollowHeroTarget(_newestTargetZone));

        base.StartShowTargetZone();
    }

    protected IEnumerator FollowHeroTarget(GameObject followingObject)
    {
        while(followingObject != null && _storedTarget != null)
        {
            followingObject.transform.position =  
                new Vector3(_storedTarget.transform.position.x, _yLoc, _storedTarget.transform.position.z);

            yield return null;
        }
    }

    protected override void AbilityStart()
    {
        GameObject newestVines =Instantiate(_encirclingVines, _newestTargetZone.transform.position, Quaternion.identity);
        StartCoroutine(FollowHeroTarget(newestVines));
        base.AbilityStart();
    }

    #region Base Ability

    #endregion
}
