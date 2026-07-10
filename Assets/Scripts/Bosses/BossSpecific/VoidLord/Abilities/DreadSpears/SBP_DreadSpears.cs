using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_DreadSpears : BossProjectileFramework
{
    [SerializeField] private float _projectileInterval;

    [Space]
    [SerializeField] private float _initialProjectileDistance;
    [SerializeField] private float _distancePerProjectile;
    [SerializeField] private float _edgeOfMapOffset;
    
    private WaitForSeconds _projectileWait;
    private Vector3 _targetSpawnLocation;
    private float _targetSpawnDistance;
    private float _edgeOfMapDistance;
    private int _projectileCounter = 0;
    
    [Space]
    [SerializeField] private GameObject _dreadSpearsHolder;
    
    [Space]
    [SerializeField] private GameObject _dreadSpear;
    
    private void StartSpearSpawningProcess()
    {
        StartCoroutine(SpikeSpawningProcess());
    }

    // <summary>
    /// The process by which the individual spikes appear from the ground
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpikeSpawningProcess()
    {
        while(_targetSpawnDistance < _edgeOfMapDistance)
        {
            SpawnProjectile();
            yield return _projectileWait;
        }
    }

    
    private void SpawnProjectile()
    {
        Instantiate(_dreadSpear, _targetSpawnLocation, Quaternion.identity);
        
        _projectileCounter++;
        CalculateNextTargetSpawnLocation();
    }

    private void CalculateNextTargetSpawnLocation()
    {
        _targetSpawnLocation = transform.forward * ((_distancePerProjectile * _projectileCounter) + _initialProjectileDistance);
        _targetSpawnLocation.Set(_targetSpawnLocation.x, transform.position.y, _targetSpawnLocation.z);
        _targetSpawnDistance = Vector3.Distance(Vector3.zero, _targetSpawnLocation);
    }
    
    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        
        _projectileWait = new WaitForSeconds(_projectileInterval);
        
        CalculateNextTargetSpawnLocation();
        
        _edgeOfMapDistance =  Vector3.Distance(Vector3.zero,
            EnvironmentManager.Instance.GetEdgeOfMapLoc(transform.position, transform.forward));
        _edgeOfMapDistance += _edgeOfMapOffset;
        
        StartSpearSpawningProcess();
    }
    #endregion
}
