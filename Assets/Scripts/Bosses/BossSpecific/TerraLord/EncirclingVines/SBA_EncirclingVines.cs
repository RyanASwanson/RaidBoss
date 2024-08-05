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

        StartCoroutine(TargetZoneFollow());

        base.StartShowTargetZone();
    }

    protected IEnumerator TargetZoneFollow()
    {
        while(_newestTargetZone != null && _storedTarget != null)
        {
            _newestTargetZone.transform.position =  
                new Vector3(_storedTarget.transform.position.x, _yLoc, _storedTarget.transform.position.z);

            yield return null;
        }
    }

    protected override void AbilityStart()
    {
        Instantiate(_encirclingVines, _storedTarget.transform.position, Quaternion.identity);
        base.AbilityStart();
    }
}
