using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_CrystalBarrage : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private float _targetDistance;
    private Vector3 _currentTargetLocation;

    private Vector3[] _targetDirections = { Vector3.forward, Vector3.left, Vector3.back,Vector3.right};
    private const float _targetHeight = -.75f;

    [SerializeField] private GameObject _crystalBarrage;
    [SerializeField] private GameObject _targetZone;

    public override void AbilitySetup(BossBase bossBase)
    {
        base.AbilitySetup(bossBase);
        CreateTargetLocations();
    }

    private void CreateTargetLocations()
    {
        for (int i = 0; i < _targetDirections.Length; i++)
        {
            _targetDirections[i] *= _targetDistance;
            _targetDirections[i] = new Vector3(_targetDirections[i].x, _targetHeight, _targetDirections[i].z);
        }
            

    }

    protected override void StartShowTargetZone()
    {
        DetermineTargetLocation();

        _currentTargetZones.Add(Instantiate(_targetZone, _currentTargetLocation, Quaternion.identity));

        base.StartShowTargetZone();
    }

    private void DetermineTargetLocation()
    {
        float currentFurthestDistance = 100;

        foreach (Vector3 targetDir in _targetDirections)
        {
            float currentDist = Vector3.Distance(_storedTargetLocation, targetDir);
            if (currentDist < currentFurthestDistance)
            {
                _currentTargetLocation = targetDir;
                currentFurthestDistance = currentDist;
            }
                
        }
        
    }

    protected override void AbilityStart()
    {
        Instantiate(_crystalBarrage, _currentTargetLocation, Quaternion.identity);

        base.AbilityStart();
    }

}
