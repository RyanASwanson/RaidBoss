using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Entomb : SpecificBossAbilityFramework
{
    [SerializeField] private float _rotationAmount;

    private Quaternion _storedTargetRotation;

    [SerializeField] private GameObject _entomb;
    [SerializeField] private GameObject _targetZone;

    

    protected override void StartShowTargetZone()
    {
        CalculateAttackRotation();
        _currentTargetZones.Add(Instantiate(_targetZone, _storedTargetLocation, _storedTargetRotation));
        base.StartShowTargetZone();
    }

    protected override void AbilityStart()
    {
        GameObject storedEntomb = Instantiate(_entomb, _storedTargetLocation, _storedTargetRotation);
        storedEntomb.GetComponent<SBP_Entomb>().SetUpProjectile(_myBossBase);
        base.AbilityStart();
    }

    protected void CalculateAttackRotation()
    {
        float randomYRotation = Random.Range(-1, 1) * _rotationAmount;

        _storedTargetRotation = Quaternion.Euler(new Vector3(0, randomYRotation, 0));
        
    }
}
