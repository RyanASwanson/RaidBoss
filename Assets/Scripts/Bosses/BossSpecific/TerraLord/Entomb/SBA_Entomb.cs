using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Entomb : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _entomb;
    [SerializeField] private GameObject _targetZone;

    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity));
        base.StartShowTargetZone();
    }

    protected override void AbilityStart()
    {
        GameObject storedEntomb = Instantiate(_entomb, _storedTargetLocation, Quaternion.identity);
        storedEntomb.GetComponent<SBP_Entomb>().SetUpProjectile(_myBossBase);
        base.AbilityStart();
    }
}
