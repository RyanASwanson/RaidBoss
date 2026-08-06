using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_FadingHope : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _fadingHope;
    
    public const int RAY_OF_HOPE_SPAWN_AUDIO_ID = 0;
    
    #region Base Ability

    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _specificAreaTarget, Quaternion.identity).GetComponent<BossTargetZoneParent>());
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        Instantiate(_fadingHope, _specificAreaTarget, Quaternion.identity);

        base.AbilityStart();
    }
    #endregion
}
