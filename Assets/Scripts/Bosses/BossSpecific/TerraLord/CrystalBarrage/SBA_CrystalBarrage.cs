using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_CrystalBarrage : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private int _projectileCount;
    [SerializeField] private float _timeBetweenProjectiles;

    [Space]
    [SerializeField] private Vector3 _upwardsProjectileSpawnVariance;

    [Space]
    [SerializeField] private float _targetWidth;
    [SerializeField] private float _targetDistance;
    private Vector3 _currentTargetLocation;

    private Vector3[] _targetDirections = { Vector3.forward, Vector3.left, Vector3.back,Vector3.right};
    private const float _targetHeight = -.75f;

    [SerializeField] private GameObject _crystalBarrage;
    [SerializeField] private GameObject _barrageUpwardsVisual;
    [SerializeField] private GameObject _targetZone;

    [Space]
    [SerializeField] private GameObject _upwardsCrystalSource;

    public override void AbilitySetup(BossBase bossBase)
    {
        base.AbilitySetup(bossBase);
        CalculateTargetLocations();
    }

    private void CalculateTargetLocations()
    {
        for (int i = 0; i < _targetDirections.Length; i++)
        {
            _targetDirections[i] *= _targetDistance;
            _targetDirections[i] = new Vector3(_targetDirections[i].x, _targetHeight, _targetDirections[i].z);
        }
            

    }

    protected override void StartShowTargetZone()
    {
        DetermineTargetLocation();

        _currentTargetZones.Add(Instantiate(_targetZone, _currentTargetLocation, Quaternion.identity));

        StartUpwardsProjectileProcess();

        base.StartShowTargetZone();
    }

    private void DetermineTargetLocation()
    {
        float currentFurthestDistance = float.MaxValue;

        foreach (Vector3 targetDir in _targetDirections)
        {
            float currentDist = Vector3.Distance(_storedTargetLocation, targetDir);
            if (currentDist < currentFurthestDistance)
            {
                _currentTargetLocation = targetDir;
                currentFurthestDistance = currentDist;
            }
                
        }
        
    }

    protected override void AbilityStart()
    {
        StartDamageProjectileProcess();

        base.AbilityStart();
    }

    #region Upwards Projectile
    protected void StartUpwardsProjectileProcess()
    {
        StartCoroutine(UpwardsProjectileProcess());
    }

    protected IEnumerator UpwardsProjectileProcess()
    {
        for(int i = 0; i < _projectileCount; i++)
        {
            CreateUpwardsProjectile();
            yield return new WaitForSeconds(_timeBetweenProjectiles);
        }
        
    }

    protected void CreateUpwardsProjectile()
    {
        Vector3 randomSpawnVariance = new Vector3(
            Random.Range(-_upwardsProjectileSpawnVariance.x, _upwardsProjectileSpawnVariance.x),
            Random.Range(-_upwardsProjectileSpawnVariance.y, _upwardsProjectileSpawnVariance.y),
            Random.Range(-_upwardsProjectileSpawnVariance.z, _upwardsProjectileSpawnVariance.z));

        Vector3 spawnLocation = _upwardsCrystalSource.transform.position + randomSpawnVariance;

        Instantiate(_barrageUpwardsVisual, spawnLocation, Quaternion.identity);
    }

    #endregion

    #region DamageProjectile

    protected void StartDamageProjectileProcess()
    {
        StartCoroutine(DamageProjectileProcess());
    }

    protected IEnumerator DamageProjectileProcess()
    {
        for (int i = 0; i < _projectileCount; i++)
        {
            CreateDamageProjectile();
            yield return new WaitForSeconds(_timeBetweenProjectiles);
        }
            
    }

    protected void CreateDamageProjectile()
    {
        Vector3 randomSpawnVariance = new Vector3(Random.Range(-_targetWidth, _targetWidth),
            0, Random.Range(-_targetWidth, _targetWidth));

        randomSpawnVariance = Quaternion.Euler(0, -45, 0) * randomSpawnVariance;

        Vector3 spawnLocation = _currentTargetLocation + randomSpawnVariance;

        Instantiate(_crystalBarrage, spawnLocation, Quaternion.identity) ;
    }
    #endregion

}
