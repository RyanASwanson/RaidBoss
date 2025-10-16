using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Magma Lord's Magma Wave ability
/// </summary>
public class SBA_MagmaWave : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _magmaWave;

    private GameObject _storedMagmaWave;
    private Vector3 _edgeOfMap;


    #region Base Ability
    protected override void AbilityPrep()
    {
        //Determines the location of the edge of the map
        //Direction determined by the direction of the current target
        _edgeOfMap = EnvironmentManager.Instance.
            GetEdgeOfMapLoc(transform.position,
            (_storedTarget.transform.position - Vector3.zero).normalized);

        base.AbilityPrep();
    }

    /// <summary>
    /// Shows the target zone of the attack and determines the location and angle
    /// </summary>
    protected override void StartShowTargetZone()
    {
        //Find the point in between the boss and edge of map
        Vector3 midpoint = (transform.position + _edgeOfMap) / 2;
        midpoint = EnvironmentManager.Instance.GetClosestPointToFloor(midpoint);

        GameObject newTargetZone = Instantiate(_targetZone, midpoint, Quaternion.identity);

        //Set the scale of the target zone to be the length of the distance from boss to edge of map
        newTargetZone.transform.localScale = new(newTargetZone.transform.localScale.x,
            newTargetZone.transform.localScale.y, Vector3.Distance(transform.position, _edgeOfMap) / 2);

        //Make the target zone be pointed towards the boss
        newTargetZone.transform.LookAt(transform.position);
        newTargetZone.transform.eulerAngles = new Vector3(0, newTargetZone.transform.eulerAngles.y, 0);

        //Adds the newly spawn target zone into the list of target zones currently active
        _currentTargetZones.Add(newTargetZone);

        base.StartShowTargetZone();
    }


    protected override void StartAbilityWindUp()
    {
        base.StartAbilityWindUp();
    }

    /// <summary>
    /// Spawns and sets up the projectile
    /// </summary>
    protected override void AbilityStart()
    {
        //Spawns the magma wave damage zone
        _storedMagmaWave = Instantiate(_magmaWave, _edgeOfMap, Quaternion.identity);
        //Sets up the projectile
        _storedMagmaWave.GetComponent<SBP_MagmaWave>().SetUpProjectile(_myBossBase, _abilityID);
        base.AbilityStart();
    }
    #endregion
}
