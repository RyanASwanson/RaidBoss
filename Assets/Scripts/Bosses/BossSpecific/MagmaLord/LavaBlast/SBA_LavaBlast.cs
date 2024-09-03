using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Magma Lord's Lava Blast ability
/// Spawns a safe zone in the center, if no hero is in it when the ability starts
///     deals damage on the outer rim of the arena
/// </summary>
public class SBA_LavaBlast : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private Vector3 _targetLocation;

    [SerializeField] private GameObject _lavaBlast;
    [SerializeField] private GameObject _targetZone;

    [SerializeField] private GameObject _failedVFX;

    private Queue<GameObject> _storedSafeZones = new Queue<GameObject>();

    
    /// <summary>
    /// Checks if at least 1 hero is in the safe zone area
    /// </summary>
    /// <returns></returns>
    private bool IsHeroInSafeZone()
    {
        return _storedSafeZones.Dequeue().GetComponent<BossAbilitySafeZone>().DoesSafeZoneContainHero();
    }

    /// <summary>
    /// If there is a hero in the safe zone the ability fails and doesn't deal damage
    /// </summary>
    private void AbilityFailed()
    {
        Instantiate(_failedVFX, _targetLocation, Quaternion.identity);
    }

    #region Base Ability

    /// <summary>
    /// Spawns in the safe zone 
    /// </summary>
    protected override void StartShowTargetZone()
    {
        GameObject newTargetZone = Instantiate(_targetZone, _targetLocation, Quaternion.identity);

        _storedSafeZones.Enqueue(newTargetZone);

        _currentTargetZones.Add(newTargetZone);

        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        //Checks if at least 1 hero is in the safe zone
        if (IsHeroInSafeZone())
        {
            //If there is, cancel the ability
            AbilityFailed();
            return;
        }

        //Spawn the damage zone
        Instantiate(_lavaBlast, _targetLocation, Quaternion.identity);

        base.AbilityStart();
    }
    #endregion
}
