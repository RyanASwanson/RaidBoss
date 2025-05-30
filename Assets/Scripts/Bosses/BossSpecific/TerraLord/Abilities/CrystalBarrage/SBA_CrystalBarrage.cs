using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Terra Lord's Crystal Barrage ability
/// </summary>
public class SBA_CrystalBarrage : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private int _projectileCount;
    [SerializeField] private float _timeBeforeProjectiles;
    [SerializeField] private float _timeBetweenProjectiles;

    [Space]
    [SerializeField] private Vector3 _upwardsProjectileSpawnVariance;

    [Space]
    [SerializeField] private float _targetWidth;
    [SerializeField] private float _targetDistance;
    private Vector3 _currentTargetLocation;

    private Vector3[] _targetDirections = { Vector3.forward, Vector3.left, Vector3.back,Vector3.right};

    [Space]
    [SerializeField] private float _spawnYEulerVariance;

    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _crystalBarrage;
    [SerializeField] private GameObject _barrageUpwardsVisual;

    [Space]
    [SerializeField] private GameObject _upwardsCrystalSource;

    #region Target Location

    /// <summary>
    /// Calculates the locations at which the ability can be used
    /// </summary>
    private void CalculateTargetLocations()
    {
        //Iterate through each direction
        for (int i = 0; i < _targetDirections.Length; i++)
        {
            //Multiply the direction by the distance
            _targetDirections[i] *= _targetDistance;
            //Keep the y value consistent
            _targetDirections[i] = new Vector3(_targetDirections[i].x, _specificAreaTarget.y, _targetDirections[i].z);
        }
            

    }

    /// <summary>
    /// Determines where the attack is going to occur
    /// </summary>
    private void DetermineTargetLocation()
    {
        //The attack uses 4 seperate base target locations
        //In order to determine which of them is closest to the attack target
        //  it performs distance checks with each of the base locations
        //Start the distance as float max so that it is more than all base location distances
        float currentFurthestDistance = float.MaxValue;

        //Iterate through each of the base target locations
        foreach (Vector3 targetDir in _targetDirections)
        {
            //Find the distance between the area we are attacking and the current base target location
            float currentDist = Vector3.Distance(_storedTargetLocation, targetDir);

            //Check if the current distance is less than the furthest distance we have so far
            if (currentDist < currentFurthestDistance)
            {
                //Set the attack location as the current base target location
                _currentTargetLocation = targetDir;
                //Set the current furthest distance as the current distance
                currentFurthestDistance = currentDist;
            }
                
        }
        
    }

    #endregion


    #region Upwards Projectile
    protected void StartUpwardsProjectileProcess()
    {
        StartCoroutine(UpwardsProjectileProcess());
    }

    protected IEnumerator UpwardsProjectileProcess()
    {
        yield return new WaitForSeconds(_timeBeforeProjectiles);

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
        Vector3 spawnEulerAngles = new Vector3(0, Random.Range(-_spawnYEulerVariance,_spawnYEulerVariance), 0);

        Instantiate(_crystalBarrage, spawnLocation, Quaternion.Euler(spawnEulerAngles));
    }
    #endregion

    #region Base Ability
    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
        CalculateTargetLocations();
    }

    /// <summary>
    /// Determines the area to use the ability, spawns in the target area for the attack,
    /// and starts the initial animation of the projectiles
    /// </summary>
    protected override void StartShowTargetZone()
    {
        DetermineTargetLocation();

        _currentTargetZones.Add(Instantiate(_targetZone, _currentTargetLocation, Quaternion.identity));

        StartUpwardsProjectileProcess();

        base.StartShowTargetZone();
    }

    protected override void AbilityStart()
    {
        StartDamageProjectileProcess();

        base.AbilityStart();
    }

    
    #endregion

}
