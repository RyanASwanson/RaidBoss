using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Magma lord attack
/// Create a target zone at the center of the arena if no hero is inside it deal 
///     damage at the edges of the arena
/// </summary>
public class SBA_MagmaBlast : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private Vector3 _targetLocation;

    [SerializeField] private GameObject _magmaBlast;
    [SerializeField] private GameObject _targetZone;

    [SerializeField] private GameObject _failedVFX;

    private Queue<GameObject> _storedSafeZones = new Queue<GameObject>();

    protected override void AbilityPrep()
    {

        base.AbilityPrep();
    }

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


    protected override void StartAbilityWindUp()
    {
        base.StartAbilityWindUp();
    }


    protected override void AbilityStart()
    {
        if (_storedSafeZones.Dequeue().GetComponent<BossAbilitySafeZone>().DoesSafeZoneContainHero())
        {
            AbilityFailed();
            return;
        }
            

        Instantiate(_magmaBlast, _targetLocation, Quaternion.identity);

        base.AbilityStart();
    }

    private void AbilityFailed()
    {
        Instantiate(_failedVFX, _targetLocation, Quaternion.identity);
    }
}
