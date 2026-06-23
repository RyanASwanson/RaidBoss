using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SB_VoidLord : SpecificBossFramework
{
    public static SB_VoidLord Instance;

    [Space] 
    [SerializeField] private float _passiveRayOfHopeSpawnRate;
    [SerializeField] private float _passiveRayOfHopeSpawnDistanceChecks;
    [SerializeField] private float _passiveRayOfHopeMinimumBossDistance;
    [SerializeField] private float _passiveRayOfHopeMapRadiusOffset;
    [SerializeField] private GameObject _rayOfHope;
    private Coroutine _rayOfHopePassiveSpawning;
    private float _rayOfHopePassiveSpawningTimer = 0;
    private float _rayOfHopeSpawnMapRadius;

    [Space]
    [SerializeField] private int _fadingHopeAttackFrequency;
    [SerializeField] private SBA_FadingHope _fadingHope;
    private int _fadingHopeCounter = 0;

    public void FadingHopeUsed()
    {
        _fadingHopeCounter = 0;
    }

    private void StartSpawningPassiveRaysOfHope()
    {
        StopSpawningPassiveRaysOfHope();
        
        _rayOfHopePassiveSpawning = StartCoroutine(SpawnPassiveRaysOfHope());
    }
    
    private void StopSpawningPassiveRaysOfHope()
    {
        if (!_rayOfHopePassiveSpawning.IsUnityNull())
        {
            StopCoroutine(_rayOfHopePassiveSpawning);
        }
    }

    private IEnumerator SpawnPassiveRaysOfHope()
    {
        while (true)
        {
            _rayOfHopePassiveSpawningTimer += Time.deltaTime;

            if (_rayOfHopePassiveSpawningTimer >= _passiveRayOfHopeSpawnRate)
            {
                _rayOfHopePassiveSpawningTimer = 0;
                SpawnRayOfHope(GetFurthestRandomRayOfHopeSpawnPoint());
            }
            
            yield return null;
        }
    }

    private void SpawnRayOfHope(Vector3 spawnPoint)
    {
        Instantiate(_rayOfHope, spawnPoint, Quaternion.identity);
    }

    private Vector3 GetFurthestRandomRayOfHopeSpawnPoint()
    {
        float currentDistance;
        float currentMinimumDistance;
        float furthestMinimumDistance = 0;
        
        Vector3 currentSpawnPoint;
        Vector3 currentMinimumSpawnPoint = Vector3.zero;
        Vector3 furthestMinimumSpawnPoint = Vector3.zero;

        List<HeroBase> livingHeroes = HeroesManager.Instance.GetCurrentLivingHeroes();
        
        /*
         * TODO add functionality to change the total amount of checks based on how many Heroes there are
         * TODO More heroes means fewer checks to ensure it isn't doing too many calculations
         */

        for(int i = 0; i < _passiveRayOfHopeSpawnDistanceChecks; i++)
        {
            currentSpawnPoint = GetRandomRayOfHopeSpawnPoint();
            currentMinimumDistance = int.MaxValue;
            
            // Debugs to show all spawn locations
            /*GameObject a = Instantiate(_tempObj, currentSpawnPoint, Quaternion.identity);
            a.transform.position += new Vector3(0, .01f, 0);
            Destroy(a, 3);*/
            
            foreach (HeroBase hero in livingHeroes)
            {
                currentDistance = Vector3.Distance(currentSpawnPoint, hero.transform.position);
                if (currentDistance < currentMinimumDistance)
                {
                    currentMinimumDistance = currentDistance;
                    currentMinimumSpawnPoint = currentSpawnPoint;
                }
            }
            
            if (currentMinimumDistance > furthestMinimumDistance)
            {
                furthestMinimumDistance = currentMinimumDistance;
                furthestMinimumSpawnPoint = currentMinimumSpawnPoint;
            }
        }

        return furthestMinimumSpawnPoint;
    }

    private Vector3 GetRandomRayOfHopeSpawnPoint()
    {
        Vector3 raySpawnPoint = new Vector3(Random.Range(-_rayOfHopeSpawnMapRadius, _rayOfHopeSpawnMapRadius),
            EnvironmentManager.Instance.GetDefaultGroundHeight(), Random.Range(-_rayOfHopeSpawnMapRadius, _rayOfHopeSpawnMapRadius));

        raySpawnPoint = Quaternion.Euler(0, -45, 0) * raySpawnPoint;
        
        // If we are too close to the boss
        if (Mathf.Abs(raySpawnPoint.x) + Mathf.Abs(raySpawnPoint.z) < _passiveRayOfHopeMinimumBossDistance)
        {
            // Try again by calling this function again
            return GetRandomRayOfHopeSpawnPoint();
        }
        
        return raySpawnPoint;
    }
    
    #region BaseBoss
    protected override void CreateSpecificBossInstance()
    {
        Instance = this;
    }

    protected override void StartFight()
    {
        base.StartFight();
        _rayOfHopeSpawnMapRadius = EnvironmentManager.Instance.GetMapRadius() + _passiveRayOfHopeMapRadiusOffset;
        StartSpawningPassiveRaysOfHope();
    }

    public override bool StartAbility(SpecificBossAbilityFramework bossAbility, bool isAbilityForceActivated)
    {
        if (bossAbility == _fadingHope)
        {
            FadingHopeUsed();
        }
        else
        {
            _fadingHopeCounter++;
        }
        
        return base.StartAbility(bossAbility, isAbilityForceActivated);
    }
    
    /// <summary>
    /// Randomly selected an ability from the list of readied abilities
    /// </summary>
    /// <returns> The next ability selected</returns>
    protected override SpecificBossAbilityFramework SelectNextAbility()
    {
        if (_fadingHopeCounter >= _fadingHopeAttackFrequency)
        {
            return _fadingHope;
        }

        int randomAbility = Random.Range(0, _readyBossAttacks.Count);

        return _readyBossAttacks[randomAbility];
    }
    
    protected override void BossDied()
    {
        base.BossDied();
    }

    protected override void CheckToUnlockSpecialistAchievement()
    {
        base.CheckToUnlockSpecialistAchievement();
        
        if (SelectionManager.Instance.GetSelectedDifficulty() < EGameDifficulty.Mythic)
        {
            return;
        }
        
        /*if ()
        {
            UnlockedSpecialistAchievement();
        }*/
    }
    
    #endregion
}
