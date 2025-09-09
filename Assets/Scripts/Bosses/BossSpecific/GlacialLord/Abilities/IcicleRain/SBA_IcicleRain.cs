using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SBA_IcicleRain : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _icicleRain;
    [SerializeField] private GameObject _icicleRainUpwardsVisual;
    

    private GameObject _newestTargetZone;

    /// <summary>
    /// Makes the target zone and attack follow the hero it is targetting
    /// </summary>
    /// <param name="followingObject"></param>
    /// <returns></returns>
    protected IEnumerator FollowHeroTarget(GameObject followingObject)
    {
        while (!followingObject.IsUnityNull() && !_storedTarget.IsUnityNull())
        {
            //Set the position of the object to be at the location of the current target
            //The Y remains consistent
            followingObject.transform.position =
                new Vector3(_storedTarget.transform.position.x, _specificAreaTarget.y, _storedTarget.transform.position.z);

            yield return null;
        }
    }
    
    #region Base Ability
    protected override void AbilityPrep()
    {
        Instantiate(_icicleRainUpwardsVisual, transform.position, Quaternion.identity);
        base.AbilityPrep();
    }
    
    
    protected override void StartShowTargetZone()
    {
        //Spawns the target area
        _newestTargetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity);
        //Adds the target area to the list of target areas
        _currentTargetZones.Add(_newestTargetZone);

        //Makes the target area follow the hero that is being targetted
        StartCoroutine(FollowHeroTarget(_newestTargetZone));

        base.StartShowTargetZone();
    }

    protected override void AbilityStart()
    {
        //Spawns the damaging ability
        GameObject newestIcicleRain = Instantiate(_icicleRain, _newestTargetZone.transform.position, Quaternion.identity);

        SBP_IcicleRain icicleFunc = newestIcicleRain.GetComponent<SBP_IcicleRain>();
        icicleFunc.SetUpProjectile(_myBossBase);

        base.AbilityStart();
    }
    #endregion
}
