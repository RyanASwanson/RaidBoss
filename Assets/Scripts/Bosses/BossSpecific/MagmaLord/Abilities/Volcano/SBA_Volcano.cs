using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Magma Lord's Volcano ability
/// </summary>
public class SBA_Volcano : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private int _projectileCount;
    [SerializeField] private float _minimumProjectileDistance;
    [SerializeField] private float _mapRadiusOffset;

    private const float _rotationAmount = 90;
    private const float _maxRotations = 3;

    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _volcanoDamageZone;

    private List<GameObject> _storedDamageZones = new List<GameObject>();
    private List<Vector3> _targetLocations;

    /// <summary>
    /// Determines where the attacks should occur
    /// </summary>
    private void DetermineAttackLocations()
    {
        _targetLocations = new List<Vector3>();

        //Iterate for the amount of projectiles this attack spawns
        for(int i = 0; i < _projectileCount; i++)
        {
            //Determines where the attack should take place
            _targetLocations.Add(GenerateNewAttackLocation(0));
        }
    }

    /// <summary>
    /// Creates a new location for the attack to spawn
    /// </summary>
    /// <param name="attempt"> The amount of attempts it has spent trying to spawn it away from other projectiles </param>
    /// <returns></returns>
    private Vector3 GenerateNewAttackLocation(int attempt)
    {
        Vector3 currentTestLocation;
        float mapRadius = EnvironmentManager.Instance.GetMapRadius() - _mapRadiusOffset;
        
        currentTestLocation = new Vector3(Random.Range(-mapRadius, mapRadius), 
            transform.position.y, Random.Range(-mapRadius, mapRadius));

        currentTestLocation = EnvironmentManager.Instance.GetClosestPointToFloor(currentTestLocation);

        foreach (Vector3 target in _targetLocations)
        {
            //Checks if the projectile is too close to the current other projectiles
            //Only checks if the amount of attempts is less that a designated number to prevent going forever
            if (Vector3.Distance(target, currentTestLocation) < _minimumProjectileDistance && attempt < 5)
            {
                return GenerateNewAttackLocation(attempt+1);
            }
        }

        return currentTestLocation;
    }
    
    //TODO Delay the activations to be one after another
    /*protected IEnumerator CreateVolcanoDamageZonesProcess()
    {

    }*/
    
    #region Base Ability
    /// <summary>
    /// Spawns in all the target zones
    /// </summary>
    protected override void StartShowTargetZone()
    {
        DetermineAttackLocations();

        foreach (Vector3 attackLoc in _targetLocations)
        {
            _currentTargetZones.Add(Instantiate(_targetZone, attackLoc, Quaternion.identity));
        }
        
        base.StartShowTargetZone();
    }

    /// <summary>
    /// Starts the ability and spawns in the damaging volcano zones
    /// </summary>
    protected override void AbilityStart()
    {
        foreach (Vector3 attackLoc in _targetLocations)
        {
            GameObject newestDamageZone = Instantiate(_volcanoDamageZone, attackLoc, Quaternion.identity);

            newestDamageZone.transform.eulerAngles += new Vector3(0, _rotationAmount *
                Mathf.RoundToInt(Random.Range(0, _maxRotations)), 0);
            _storedDamageZones.Add(newestDamageZone);
        }

        base.AbilityStart();
    }
    #endregion
}