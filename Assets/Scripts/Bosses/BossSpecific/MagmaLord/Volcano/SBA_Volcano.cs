using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Volcano : SpecificBossAbilityFramework
{
    [SerializeField] private int _projectileCount;

    [SerializeField] private GameObject _volcanoDamageZone;
    [SerializeField] private GameObject _targetZone;

    private List<GameObject> _storedDamageZones = new List<GameObject>();
    private List<Vector3> _targetLocations;

    protected override void AbilityPrep()
    {
        base.AbilityPrep();
    }

    protected override void StartShowTargetZone()
    {
        DetermineAttackLocations();

        foreach(Vector3 attackLoc in _targetLocations)
            _currentTargetZones.Add(Instantiate(_targetZone, attackLoc, Quaternion.identity));

        base.StartShowTargetZone();
    }

    private void DetermineAttackLocations()
    {
        _targetLocations = new List<Vector3>();

        for(int i = 0; i < _projectileCount; i++)
        {
            float mapRadius = GameplayManagers.Instance.GetEnvironmentManager().GetMapRadius();
            float tempX = Random.Range(-mapRadius, mapRadius);
            float tempZ = Random.Range(-mapRadius, mapRadius);

            _targetLocations.Add(GameplayManagers.Instance.GetEnvironmentManager().
                GetClosestPointToFloor(new Vector3(tempX, transform.position.y, tempZ)));
        }
    }

    protected override void StartAbilityWindUp()
    {
        //_storedFallingMeteor = Instantiate(_volcanoDamageZone, _storedTargetLocation, _volcanoDamageZone.transform.rotation);
        base.StartAbilityWindUp();
    }

    protected override void AbilityStart()
    {
        foreach (Vector3 attackLoc in _targetLocations)
        {
            _storedDamageZones.Add(Instantiate(_volcanoDamageZone, attackLoc, Quaternion.identity));
        }


            
        //_storedMovingMeteor.GetComponent<SBP_FollowingMeteor>().AdditionalSetup(_storedTarget);
        base.AbilityStart();
    }
}
