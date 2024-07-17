using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Magma lord attack
/// Create a damage target zone at the edge of the arena in the direction of a target
///     then has a projectile move from that point to the boss
/// </summary>
public class SBA_MagmaWave : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _magmaWave;
    [SerializeField] private GameObject _targetZone;

    private GameObject _storedMagmaWave;
    private Vector3 _edgeOfMap;

    protected override void AbilityPrep()
    {
        _edgeOfMap = GameplayManagers.Instance.GetEnvironmentManager().
            GetEdgeOfMapWithDirection(transform.position,
            (_storedTarget.transform.position - Vector3.zero).normalized);

        base.AbilityPrep();
    }

    protected override void StartShowTargetZone()
    {
        //Find the point in between the boss and edge of map
        Vector3 midpoint = (transform.position + _edgeOfMap) / 2;
        midpoint = GameplayManagers.Instance.GetEnvironmentManager().GetClosestPointToFloor(midpoint);

        GameObject newTargetZone = Instantiate(_targetZone, midpoint, Quaternion.identity);

        //Set the scale of the target zone to be the length of the distance from boss to edge of map
        newTargetZone.transform.localScale = new (newTargetZone.transform.localScale.x,
            newTargetZone.transform.localScale.y, Vector3.Distance(transform.position, _edgeOfMap)/2);

        //Make the target zone be pointed towards the boss
        newTargetZone.transform.LookAt(transform.position);
        newTargetZone.transform.eulerAngles = new Vector3(0, newTargetZone.transform.eulerAngles.y, 0);

        _currentTargetZones.Add(newTargetZone);

        base.StartShowTargetZone();
    }


    protected override void StartAbilityWindUp()
    {
        base.StartAbilityWindUp();
    }

    protected override void AbilityStart()
    {
        _storedMagmaWave = Instantiate(_magmaWave, _edgeOfMap, Quaternion.identity);
        SBP_MagmaWave mwScript = _storedMagmaWave.GetComponent<SBP_MagmaWave>();
        mwScript.SetUpProjectile(_ownerBossBase);
        mwScript.AdditionalSetup();
        base.AbilityStart();
    }
}
