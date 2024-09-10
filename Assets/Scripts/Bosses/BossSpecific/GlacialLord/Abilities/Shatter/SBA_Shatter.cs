using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Shatter : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _shatterProjectile;
    private GameObject _currentFollowingTargetZone;
    private GameObject _currentShatterProjectile;

    [Space]
    [SerializeField] private GameObject _iceShard;
    [SerializeField] private List<Vector3> _iceCrystalLocations;
    private GameObject _currentCrystalShard;
    private int _iceCrystalLocationCounter;

    [SerializeField] private SB_GlacialLord _glacialLord;

    protected override void AbilityPrep()
    {
        base.AbilityPrep();
        _currentCrystalShard = Instantiate(_iceShard, ChooseCrystalLocation(), Quaternion.identity);
    }

    private Vector3 ChooseCrystalLocation()
    {
        Vector3 storedLocation =_iceCrystalLocations[Random.Range(0, _iceCrystalLocations.Count-1)];
        _iceCrystalLocationCounter++;
        if (_iceCrystalLocationCounter >= _iceCrystalLocations.Count)
            _iceCrystalLocationCounter = 0;

        return storedLocation;
    }

    protected override void StartShowTargetZone()
    {
        _currentFollowingTargetZone = Instantiate(_targetZone, transform.position, Quaternion.identity);
        _currentTargetZones.Add(_currentFollowingTargetZone);
        StartCoroutine(TargetZoneFollow());
    }

    private IEnumerator TargetZoneFollow()
    {
        while(true)
        {
            _currentFollowingTargetZone.transform.LookAt(_glacialLord.CrownLocationClosestToFloor());
            _currentFollowingTargetZone.transform.eulerAngles = 
                new Vector3(0, _currentFollowingTargetZone.transform.eulerAngles.y, 0);
            yield return null;
        }
    }

    protected override void AbilityStart()
    {
        base.AbilityStart();
        _currentShatterProjectile =Instantiate(_shatterProjectile, transform.position, Quaternion.identity);

        _currentShatterProjectile.transform.LookAt(_glacialLord.CrownLocationClosestToFloor());
        _currentShatterProjectile.transform.eulerAngles =
            new Vector3(0, -_currentShatterProjectile.transform.eulerAngles.y, 0);

        _currentShatterProjectile.GetComponent<SBP_Shatter>().SetUpProjectile(_myBossBase);
    }

    
}
