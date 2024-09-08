using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Magma Lord's Scorch ability
/// </summary>
public class SBA_Scorch : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _scorch;
    [SerializeField] private GameObject _targetZone;



    #region Base Ability

    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _specificAreaTarget, Quaternion.identity));
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        Instantiate(_scorch, _specificAreaTarget, Quaternion.identity);

        base.AbilityStart();
    }
    #endregion
}
