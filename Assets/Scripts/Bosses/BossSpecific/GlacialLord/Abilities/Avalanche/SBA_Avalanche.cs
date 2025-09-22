using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_Avalanche : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private float _spawnDistance;
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _avalanche;

    private GameObject _storedTargetZone;

    private GameObject _storedAvalanche;
    private Vector3 _edgeOfMap;

    public const int AVALANCHE_END_AUDIO_ID = 0;
    
    
    private Vector3 GetProjectileSpawnLocation()
    {
        Vector3 spawnLocation = transform.position;

        Vector3 dir = _storedTarget.transform.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z).normalized;

        spawnLocation += dir * _spawnDistance;

        return spawnLocation;
    }

    public void ProjectileReachedEndOfPath()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[AVALANCHE_END_AUDIO_ID]);
    }
    
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
        _storedTargetZone.transform.position.Set(_storedTargetZone.transform.position.x,
            _specificAreaTarget.y, _storedTargetZone.transform.position.z);

        //Adds the newly spawn target zone into the list of target zones currently active
        _currentTargetZones.Add(_storedTargetZone);

        StartCoroutine(UpdateTargetZone());

        base.StartShowTargetZone();
    }
    
    private IEnumerator UpdateTargetZone()
    {
        Vector3 lastCheckedDirection = Vector3.zero;
        while(!_storedTargetZone.IsUnityNull())
        {
            Vector3 currentDirection = _storedTarget.transform.position - Vector3.zero;
            _edgeOfMap =  EnvironmentManager.Instance.GetEdgeOfMapLoc(transform.position,
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
        _storedAvalanche = Instantiate(_avalanche, GetProjectileSpawnLocation(), Quaternion.identity);
        //Sets up the projectile
        SBP_Avalanche avalanche = _storedAvalanche.GetComponent<SBP_Avalanche>();
        avalanche.SetUpProjectile(_myBossBase);
        avalanche.AdditionalSetUp(_storedTarget.transform.position, this);

        base.AbilityStart();
    }
    #endregion
}
