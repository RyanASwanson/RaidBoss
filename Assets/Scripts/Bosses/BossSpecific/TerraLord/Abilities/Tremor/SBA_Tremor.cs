using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Terra Lord's Tremor ability
/// </summary>
public class SBA_Tremor : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _tremor;
    [SerializeField] private GameObject _targetZone;

    #region Base Ability
    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _specificAreaTarget, Quaternion.identity));
        base.StartShowTargetZone();
    }

    protected override void AbilityStart()
    {
        GameObject storedTremor = Instantiate(_tremor, _specificAreaTarget, Quaternion.identity);
        storedTremor.GetComponent<SBP_Tremor>().SetUpProjectile(_myBossBase);
        base.AbilityStart();
    }
    #endregion
}
