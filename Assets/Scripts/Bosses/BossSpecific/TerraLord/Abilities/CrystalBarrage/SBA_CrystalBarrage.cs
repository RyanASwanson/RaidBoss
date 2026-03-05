using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private WaitForSeconds _delayBetweenProjectiles;

    [Space]
    [SerializeField] private Vector3 _upwardsProjectileSpawnVariance;

    [Space]
    [SerializeField] private float _targetWidth;
    [SerializeField] private float _targetDistance;
    private Vector3 _currentTargetLocation;

    private Vector3[] _targetDirections = { Vector3.forward, Vector3.left, Vector3.back,Vector3.right};
    private Vector3[] _targetLocations;

    [Space]
    [SerializeField] private float _spawnYEulerVariance;
    [SerializeField] private float _fallingTime;
    private WaitForSeconds _fallingWait;

    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _individualTargetZone;
    [SerializeField] private GameObject _crystalBarrage;
    [SerializeField] private GameObject _barrageUpwardsVisual;
    
    private Coroutine _individualTargetZoneSpawnCoroutine;
    private Coroutine _upwardsProjectilesSpawnCoroutine;
    private Coroutine _damageProjectileSpawnCoroutine;

    [Space]
    [SerializeField] private GameObject _upwardsCrystalSource;
    
    public const int CRYSTAL_BARRAGE_UPWARDS_PROJECTILE_SPAWNED_AUDIO_ID = 0;
    public const int CRYSTAL_BARRAGE_IMPACT_AUDIO_ID = 1;

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

        Vector3 randomSpawnVariance;
        for (int i = 0; i < _targetLocations.Length; i++)
        {
            randomSpawnVariance = new Vector3(Random.Range(-_targetWidth, _targetWidth),
                0, Random.Range(-_targetWidth, _targetWidth));

            randomSpawnVariance = Quaternion.Euler(0, -45, 0) * randomSpawnVariance;
            _targetLocations[i] = _currentTargetLocation + randomSpawnVariance;
        }
        
    }

    private void StartCreateIndividualTargetZoneProcess()
    {
        _individualTargetZoneSpawnCoroutine = StartCoroutine(CreateIndividualTargetZonesProcess());
    }

    private void StopCreateIndividualTargetZoneProcess()
    {
        if (!_individualTargetZoneSpawnCoroutine.IsUnityNull())
        {
            StopCoroutine(_individualTargetZoneSpawnCoroutine);
        }
    }
    
    private IEnumerator CreateIndividualTargetZonesProcess()
    {
        for (int i = 0; i < _projectileCount; i++)
        {
            _currentTargetZones.Add((Instantiate(_individualTargetZone, _targetLocations[i], Quaternion.identity).GetComponent<BossTargetZoneParent>()));
            if (i != 0)
            {
                PlayTargetZoneSpawnedAudio();
            }
            yield return _delayBetweenProjectiles;
        }
    }

    #endregion

    #region Upwards Projectile
    protected void StartUpwardsProjectileProcess()
    {
        _upwardsProjectilesSpawnCoroutine = StartCoroutine(UpwardsProjectileProcess());
    }

    protected void StopUpwardsProjectileProcess()
    {
        if (!_upwardsProjectilesSpawnCoroutine.IsUnityNull())
        {
            StopCoroutine(_upwardsProjectilesSpawnCoroutine);
        }
    }

    protected IEnumerator UpwardsProjectileProcess()
    {
        yield return new WaitForSeconds(_timeBeforeProjectiles);

        for(int i = 0; i < _projectileCount; i++)
        {
            CreateUpwardsProjectile();
            yield return _delayBetweenProjectiles;
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

        PlayUpwardsProjectileSpawnedSFX();
    }
    
    private void PlayUpwardsProjectileSpawnedSFX()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[CRYSTAL_BARRAGE_UPWARDS_PROJECTILE_SPAWNED_AUDIO_ID]);
    }

    #endregion

    #region DamageProjectile
    private void StartDamageProjectileProcess()
    {
        _damageProjectileSpawnCoroutine = StartCoroutine(DamageProjectileProcess());
    }

    private void StopDamageProjectileProcess()
    {
        if (!_damageProjectileSpawnCoroutine.IsUnityNull())
        {
            StopCoroutine(_damageProjectileSpawnCoroutine);
        }
    }

    private IEnumerator DamageProjectileProcess()
    {
        for (int i = 0; i < _projectileCount; i++)
        {
            CreateDamageProjectile(i);
            StartCoroutine(PlayProjectileImpactDelay());
            yield return _delayBetweenProjectiles;
        }
    }

    private void CreateDamageProjectile(int projectileID)
    {
        Vector3 spawnEulerAngles = new Vector3(0, Random.Range(-_spawnYEulerVariance,_spawnYEulerVariance), 0);

        Instantiate(_crystalBarrage, _targetLocations[projectileID], Quaternion.Euler(spawnEulerAngles));
    }

    private IEnumerator PlayProjectileImpactDelay()
    {
        yield return _fallingWait;
        PlayProjectileImpactSFX();
    }
    
    private void PlayProjectileImpactSFX()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[CRYSTAL_BARRAGE_IMPACT_AUDIO_ID]);
    }
    
    #endregion

    #region Base Ability
    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
        
        _delayBetweenProjectiles = new WaitForSeconds(_timeBetweenProjectiles);
        _fallingWait = new WaitForSeconds(_fallingTime);
        
        _targetLocations = new Vector3[_projectileCount];
        
        CalculateTargetLocations();
    }

    /// <summary>
    /// Determines the area to use the ability, spawns in the target area for the attack,
    /// and starts the initial animation of the projectiles
    /// </summary>
    protected override void StartShowTargetZone()
    {
        DetermineTargetLocation();

        //_currentTargetZones.Add(Instantiate(_targetZone, _currentTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>());
        StartCreateIndividualTargetZoneProcess();

        StartUpwardsProjectileProcess();

        base.StartShowTargetZone();
    }
    
    protected override IEnumerator TargetZonesProcess()
    {
        yield return _targetZoneWait;
        StartRemoveIndividualTargetZonesDelayedProcess();
    }

    protected override void AbilityStart()
    {
        StartDamageProjectileProcess();

        base.AbilityStart();
    }
    
    public override void StopBossAbility()
    {
        StopCreateIndividualTargetZoneProcess();

        StopUpwardsProjectileProcess();

        StopDamageProjectileProcess();
        
        base.StopBossAbility();
    }
    #endregion
}
