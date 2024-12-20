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


    

    private void DetermineAttackLocations()
    {
        _targetLocations = new List<Vector3>();

        for(int i = 0; i < _projectileCount; i++)
        {
            _targetLocations.Add(GenerateNewAttackLocation(0));
        }
    }

    private Vector3 GenerateNewAttackLocation(int attempt)
    {
        Vector3 currentTestLocation;
        float mapRadius = GameplayManagers.Instance.GetEnvironmentManager().GetMapRadius() - _mapRadiusOffset;

        
        currentTestLocation = new Vector3(Random.Range(-mapRadius, mapRadius), 
            transform.position.y, Random.Range(-mapRadius, mapRadius));

        currentTestLocation = GameplayManagers.Instance.GetEnvironmentManager().
                GetClosestPointToFloor(currentTestLocation);

        foreach (Vector3 target in _targetLocations)
        {
            if (Vector3.Distance(target, currentTestLocation) < _minimumProjectileDistance && attempt < 5)
                return GenerateNewAttackLocation(attempt++);
        }

        return currentTestLocation;
    }


    

    /*protected IEnumerator CreateVolcanoDamageZonesProcess()
    {

    }*/


    #region Base Ability
    protected override void StartShowTargetZone()
    {
        DetermineAttackLocations();

        foreach (Vector3 attackLoc in _targetLocations)
        {
            _currentTargetZones.Add(Instantiate(_targetZone, attackLoc, Quaternion.identity));
            //Instantiate(_targetZone, attackLoc, Quaternion.identity);
        }


        base.StartShowTargetZone();
    }

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
