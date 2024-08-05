using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Entomb : SpecificBossAbilityFramework
{
    [SerializeField] private float _rotationAmount;

    private Quaternion _storedTargetRotation;

    [SerializeField] private GameObject _entomb;
    [SerializeField] private GameObject _entombWall;

    [SerializeField] private GameObject _targetZone;

    private GameObject _currentSafeZone;

    protected override void StartShowTargetZone()
    {
        CalculateAttackRotation();

        _currentTargetZones.Add(Instantiate(_targetZone, _storedTargetLocation, _storedTargetRotation));

        //_currentSafeZone = Instantiate(_safeZone, _storedTargetLocation, _storedTargetRotation);
        //_currentTargetZones.Add(_currentSafeZone);

        base.StartShowTargetZone();
    }

    protected override void AbilityStart()
    {
        GameObject storedEntomb = Instantiate(_entomb, _storedTargetLocation, _storedTargetRotation);
        storedEntomb.GetComponent<SBP_Entomb>().SetUpProjectile(_myBossBase);

        /*if (_currentSafeZone.GetComponent<BossAbilitySafeZone>().DoesSafeZoneContainHero())
        {
            return;
        }*/

        //GameObject storedWalls = Instantiate(_entombWall, _storedTargetLocation, _storedTargetRotation);

        
        //storedWalls.GetComponent<SBP_EntombWalls>().SetUpProjectile(_myBossBase);

        base.AbilityStart();


    }

    protected void CalculateAttackRotation()
    {
        float rotationMultiplier = 1;
        switch (Random.Range(0, 2))
        {
            case (0):
                rotationMultiplier *= -1;
                break;
            case (1):
                break;
        }


        float randomYRotation = rotationMultiplier * _rotationAmount;

        _storedTargetRotation = Quaternion.Euler(new Vector3(0, randomYRotation, 0));

        
    }

}
