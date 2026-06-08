using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Provides the functionality for the Magma Lord's Volcano ability
/// </summary>
public class SBA_Volcano : SpecificBossAbilityFramework
{
    [Space] 
    [SerializeField] private float _sharedVolcanoTrackingEnrageMultiplier;
    [SerializeField] private float _sharedVolcanoTrackingEnrageScalingMultiplierIncreasePerMinute;

    private float _sharedVolcanoTrackingMultiplier = 1;

    [Space] 
    [SerializeField] private float _projectileDelay;

    private WaitForSeconds _projectileDelayWaitForSeconds;

    private const float _rotationAmount = 90;
    private const float _maxRotations = 3;

    [Space] 
    [SerializeField] private float _impactAudioPitchIncrease;
    
    private List<Vector3> _targetLocations = new List<Vector3>();
    private List<Vector3> _currentActiveTargetLocations = new List<Vector3>();
    
    private List<BossTargetZoneParent> _currentVolcanoFutureTargetZones = new();
    private List<BossTargetZoneParent> _currentActiveVolcanoFutureTargetZones = new();
    
    private List<GameObject> _storedDamageZones = new List<GameObject>();

    private Coroutine _targetZoneSpawningProcess;
    private Coroutine _damageZoneSpawningProcess;

    private const int VOLCANO_FUTURE_TARGET_ZONE_SPAWNED_AUDIO_ID = 0;
    private const int VOLCANO_IMPACT_AUDIO_ID = 1;

    [Space] 
    [SerializeField] private int _futureTargetZonesRequiredToActivate;
    [SerializeField] private int _futureTargetZonesCertainChanceToActivate;
    [SerializeField] private int _maxTargetZonesAllowed;

    [Space] 
    [SerializeField] private GameObject _volcanoHeroTrackingObject;
    private VolcanoHeroMovementTracking[] _volcanoHeroMovementTracking;

    [Space] 
    [SerializeField] private GameObject _volcanoFutureTargetZone;
    [SerializeField] private GameObject _futureTargetZoneVFX;
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _volcanoDamageZone;


    public void BattleStarted()
    {
        if (!_isAbilityEnabled)
        {
            return;
        }

        CreateVolcanoHeroTrackingOnHeroes();

        foreach (VolcanoHeroMovementTracking volcanoTracking in _volcanoHeroMovementTracking)
        {
            if (volcanoTracking.IsUnityNull())
            {
                continue;
            }

            volcanoTracking.StartTrackingHeroMovement();
        }
    }

    private void CalculateSharedVolcanoTrackingMultiplier()
    {
        _sharedVolcanoTrackingMultiplier = 1;
        if (BossStats.Instance.GetIsBossEnraged())
        {
            _sharedVolcanoTrackingMultiplier *= _sharedVolcanoTrackingEnrageMultiplier;
            
            _sharedVolcanoTrackingMultiplier *= 1 + (_sharedVolcanoTrackingEnrageScalingMultiplierIncreasePerMinute *
                                                 BossStats.Instance.GetMinutesSpentEnraged());
        }
    }

    private void CreateVolcanoHeroTrackingOnHeroes()
    {
        List<HeroBase> heroes = HeroesManager.Instance.GetCurrentHeroes();
        for (int i = 0; i < heroes.Count; i++)
        {
            VolcanoHeroMovementTracking volcanoHeroMovementTracking =
                Instantiate(_volcanoHeroTrackingObject, heroes[i].transform.position, Quaternion.identity)
                    .GetComponent<VolcanoHeroMovementTracking>();

            volcanoHeroMovementTracking.SetUpVolcanoTracking(this, heroes[i]);

            _volcanoHeroMovementTracking[i] = volcanoHeroMovementTracking;
        }
    }

    public void VolcanoTargetHitMax(VolcanoHeroMovementTracking volcanoHeroMovementTracking)
    {
        SpawnVolcanoFutureTargetZone(volcanoHeroMovementTracking.transform.position,true);
    }

    public void SpawnVolcanoFutureTargetZone(Vector3 targetLocation, bool doesPlayAudio)
    {
        if (_currentVolcanoFutureTargetZones.Count >= _maxTargetZonesAllowed)
        {
            return;
        }

        BossTargetZoneParent targetZoneParent = SpawnVolcanoFutureTargetZone(targetLocation);
        SpawnVolcanoFutureTargetZoneVFX(targetZoneParent.transform.position);

        _currentVolcanoFutureTargetZones.Add(targetZoneParent);
        _targetLocations.Add(targetZoneParent.transform.position);

        if (doesPlayAudio)
        {
            PlayVolcanoFutureTargetZoneSpawnedAudio();
        }
    }

    private BossTargetZoneParent SpawnVolcanoFutureTargetZone(Vector3 targetLocation)
    {
        BossTargetZoneParent targetZoneParent =
            Instantiate(_volcanoFutureTargetZone, targetLocation, Quaternion.identity)
                .GetComponent<BossTargetZoneParent>();

        targetZoneParent.transform.position =
            EnvironmentManager.Instance.GetClosestPointToFloor(targetZoneParent.transform.position);
        
        return targetZoneParent;
    }

    private void SpawnVolcanoFutureTargetZoneVFX(Vector3 targetLocation)
    {
        Instantiate(_futureTargetZoneVFX,targetLocation, Quaternion.identity);
    }

    private IEnumerator VolcanoTargetZoneCreationProcess()
    {
        InformHeroTrackingOfAbilityActivation();

        _currentActiveTargetLocations.Clear();
        _currentActiveTargetLocations.InsertRange(0, _targetLocations);
        
        _currentActiveVolcanoFutureTargetZones.Clear();
        _currentActiveVolcanoFutureTargetZones.InsertRange(0, _currentVolcanoFutureTargetZones);

        _targetLocations.Clear();
        _currentVolcanoFutureTargetZones.Clear();

        for (int i = 0; i < _currentActiveTargetLocations.Count; i++)
        {
            _currentActiveVolcanoFutureTargetZones[i].RemoveBossTargetZones();

            _currentTargetZones.Add(Instantiate(_targetZone, _currentActiveTargetLocations[i], Quaternion.identity)
                .GetComponent<BossTargetZoneParent>());

            if (i != 0)
            {
                PlayTargetZoneSpawnedAudio();
            }

            yield return _projectileDelayWaitForSeconds;
        }
    }

    private void BossStaggeredDuringVolcanoTargetZoneCreationProcess()
    {
        /*
         * Since the Magma Lord was staggered in the middle of removing the future zones, and spawning the target
         * zones, and the _targetLocations and _currentVolcanoFutureTargetZones were cleared it could lead to issues.
         * The future zones that were already destroyed, or actively being destroyed will not be kept around,
         * and the other future target zones are not included in the lists and are never removed.
         *
         * So we have to respawn the destroyed future target zones, and add the other zones back into the lists
         */
        
        // Iterate through all currently active target locations
        for (int i = 0; i < _currentActiveTargetLocations.Count; i++)
        {
            // If a future target zone is destroyed or is being destroyed
            if (_currentActiveVolcanoFutureTargetZones[i].IsUnityNull() || _currentActiveVolcanoFutureTargetZones[i].GetIsDestroyingSelf())
            {
                // Spawn a new replacement future target zone
                BossTargetZoneParent targetZoneParent = SpawnVolcanoFutureTargetZone(_currentActiveTargetLocations[i]);
                
                // Add it back to the lists
                _targetLocations.Insert(i,targetZoneParent.transform.position);
                _currentVolcanoFutureTargetZones.Insert(i,targetZoneParent);
                
                continue;
            }
            
            // Add the future target zones that weren't removed back into the lists
            _targetLocations.Insert(i,_currentActiveTargetLocations[i]);
            _currentVolcanoFutureTargetZones.Insert(i,_currentActiveVolcanoFutureTargetZones[i]);
        }
    }

    private IEnumerator VolcanoDamageCreationProcess()
    {
        int volcanoSpawned = 0;
        foreach (Vector3 attackLoc in _currentActiveTargetLocations)
        {
            GameObject newestDamageZone = Instantiate(_volcanoDamageZone, attackLoc, Quaternion.identity);

            newestDamageZone.transform.eulerAngles +=
                new Vector3(0, _rotationAmount * Mathf.RoundToInt(Random.Range(0, _maxRotations)), 0);

            _storedDamageZones.Add(newestDamageZone);

            PlayVolcanoAbilityAudio(volcanoSpawned);

            yield return _projectileDelayWaitForSeconds;
            volcanoSpawned++;
        }
    }

    private void InformHeroTrackingOfAbilityActivation()
    {
        foreach (VolcanoHeroMovementTracking volcanoHeroTracking in _volcanoHeroMovementTracking)
        {
            if (volcanoHeroTracking.IsUnityNull())
            {
                continue;
            }

            volcanoHeroTracking.VolcanoAbilityWasUsed();
        }
    }

    private void PlayVolcanoFutureTargetZoneSpawnedAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].BossAbilityAudio[_abilityID]
                .GeneralAbilityAudio[VOLCANO_FUTURE_TARGET_ZONE_SPAWNED_AUDIO_ID]);
    }

    private void PlayVolcanoAbilityAudio(int volcanoSpawned)
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].BossAbilityAudio[_abilityID]
                .GeneralAbilityAudio[VOLCANO_IMPACT_AUDIO_ID], out EventInstance eventInstance);

        eventInstance.getPitch(out float pitch);
        eventInstance.setPitch(pitch + (volcanoSpawned * _impactAudioPitchIncrease));
    }


    #region Base Ability

    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
        _projectileDelayWaitForSeconds = new WaitForSeconds(_projectileDelay);

        _volcanoHeroMovementTracking =
            new VolcanoHeroMovementTracking[SelectionManager.Instance.GetHeroLimitFromDifficulty()];
    }

    /// <summary>
    /// Spawns in all the target zones
    /// </summary>
    protected override void StartShowTargetZone()
    {
        _targetZoneSpawningProcess = StartCoroutine(VolcanoTargetZoneCreationProcess());

        base.StartShowTargetZone();
    }

    /// <summary>
    /// Starts the ability and spawns in the damaging volcano zones
    /// </summary>
    protected override void AbilityStart()
    {
        SB_MagmaLord.Instance.SetHasVolcanoBeenUsed(true);

        _damageZoneSpawningProcess = StartCoroutine(VolcanoDamageCreationProcess());

        base.AbilityStart();
    }

    public override void StopBossAbility()
    {
        base.StopBossAbility();
        if (!_targetZoneSpawningProcess.IsUnityNull())
        {
            StopCoroutine(_targetZoneSpawningProcess);
            BossStaggeredDuringVolcanoTargetZoneCreationProcess();
        }

        if (!_damageZoneSpawningProcess.IsUnityNull())
        {
            StopCoroutine(_damageZoneSpawningProcess);
        }
    }

    public override bool GetCanAbilityBeUsed()
    {
        if (_currentVolcanoFutureTargetZones.Count < _futureTargetZonesRequiredToActivate)
        {
            return false;
        }

        float requiredMinChance = (float)_currentVolcanoFutureTargetZones.Count /
                                  (float)_futureTargetZonesCertainChanceToActivate;
        
        float randomChance = Random.Range(0, 1f);

        return randomChance <= requiredMinChance;
    }


    public override void SubscribeToEvents()
    {
        if (_isSubscribedToEvents)
        {
            return;
        }
        
        base.SubscribeToEvents();
        _myBossBase.GetSecondPassedEnrageEvent().AddListener(CalculateSharedVolcanoTrackingMultiplier);
    }

    public override void UnsubscribeFromEvents()
    {
        if (!_isSubscribedToEvents)
        {
            return;
        }
        
        base.UnsubscribeFromEvents();
        _myBossBase.GetSecondPassedEnrageEvent().AddListener(CalculateSharedVolcanoTrackingMultiplier);
    }

    #endregion

    #region Getters

    public float GetSharedVolcanoTrackingMultiplier() => _sharedVolcanoTrackingMultiplier;

    #endregion
}