using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Avalanche : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _avalanche;
    [SerializeField] private GameObject _targetZone;

    private GameObject _storedTargetZone;

    private GameObject _storedAvalanche;
    private Vector3 _edgeOfMap;


    #region Base Ability
    protected override void AbilityPrep()
    {
        //Determines the location of the edge of the map
        //Direction determined by the direction of the current target
        

        base.AbilityPrep();
    }

    protected override void StartShowTargetZone()
    {
        _storedTargetZone = Instantiate(_targetZone, transform.position, Quaternion.identity);
        _storedTargetZone.transform.position = new Vector3(_storedTargetZone.transform.position.x,
            _specificAreaTarget.y, _storedTargetZone.transform.position.z);

        //Adds the newly spawn target zone into the list of target zones currently active
        _currentTargetZones.Add(_storedTargetZone);

        StartCoroutine(UpdateTargetZone());

        base.StartShowTargetZone();
    }
    
    private IEnumerator UpdateTargetZone()
    {
        Vector3 lastCheckedDirection = Vector3.zero;
        while(_storedTargetZone != null)
        {
            Vector3 currentDirection = _storedTarget.transform.position - Vector3.zero;
            _edgeOfMap = GameplayManagers.Instance.GetEnvironmentManager().GetEdgeOfMapLoc(transform.position,
                (currentDirection).normalized);

            if (lastCheckedDirection == currentDirection)
            {
                yield return null;
                continue;
            }
            lastCheckedDirection = currentDirection;

            _storedTargetZone.transform.LookAt(_storedTarget.transform);
            _storedTargetZone.transform.eulerAngles = new Vector3(0, _storedTargetZone.transform.eulerAngles.y, 0);

            //Set the scale of the target zone to be the length of the distance from boss to edge of map
            _storedTargetZone.transform.localScale = new(_storedTargetZone.transform.localScale.x,
                _storedTargetZone.transform.localScale.y, Vector3.Distance(transform.position, _edgeOfMap) / 2);

            yield return null;
            
        }
    }

    protected override void AbilityStart()
    {
        _storedAvalanche = Instantiate(_avalanche, transform.position, Quaternion.identity);
        //Sets up the projectile
        SBP_Avalanche avalanche = _storedAvalanche.GetComponent<SBP_Avalanche>();
        avalanche.SetUpProjectile(_myBossBase);
        avalanche.AdditionalSetup(_storedTarget.transform.position);

        base.AbilityStart();
    }

    #endregion
}
