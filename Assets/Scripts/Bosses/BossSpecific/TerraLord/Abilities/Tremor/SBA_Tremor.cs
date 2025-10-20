using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Terra Lord's Tremor ability
/// </summary>
public class SBA_Tremor : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _tremor;
    
    public const int TREMOR_IMPACT_AUDIO_ID = 0;

    #region Base Ability
    /// <summary>
    /// Starts showing the target zone for the ability
    /// </summary>
    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Add(Instantiate(_targetZone, _specificAreaTarget, Quaternion.identity));
        base.StartShowTargetZone();
    }

    /// <summary>
    /// Starts the ability
    /// </summary>
    protected override void AbilityStart()
    {
        GameObject storedTremor = Instantiate(_tremor, _specificAreaTarget, Quaternion.identity);
        storedTremor.GetComponent<SBP_Tremor>().SetUpProjectile(_myBossBase, _abilityID);
        base.AbilityStart();
    }
    #endregion
}
