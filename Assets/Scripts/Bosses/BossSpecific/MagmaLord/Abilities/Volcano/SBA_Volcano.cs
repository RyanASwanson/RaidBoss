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
    [SerializeField] private float _projectileDelay;

    private WaitForSeconds _projectileDelayWaitForSeconds;

    private const float _rotationAmount = 90;
    private const float _maxRotations = 3;
    
    [Space] 
    [SerializeField] private float _impactAudioPitchIncrease;

    private List<GameObject> _storedDamageZones = new List<GameObject>();
    private List<Vector3> _targetLocations = new List<Vector3>();

    private List<Vector3> _currentActiveTargetLocations = new List<Vector3>();

    private Coroutine _targetZoneSpawningProcess;
    private Coroutine _damageZoneSpawningProcess;
    
    private const int VOLCANO_FUTURE_TARGET_ZONE_SPAWNED_AUDIO_ID = 0;
    private const int VOLCANO_IMPACT_AUDIO_ID = 1;

    [Space]
    [SerializeField] private int _futureTargetZonesRequiredToActivate;
    [SerializeField] private int _futureTargetZonesCertainChanceToActivate;

    [Space]
    [SerializeField] private GameObject _volcanoHeroTrackingObject;
    private VolcanoHeroMovementTracking[] _volcanoHeroMovementTracking;

    [Space]
    [SerializeField] private GameObject _volcanoFutureTargetZone;
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _volcanoDamageZone;
    
    private List<BossTargetZoneParent> _currentVolcanoFutureTargetZones = new();
    private List<BossTargetZoneParent> _currentActiveVolcanoFutureTargetZones = new();
    

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

    private void CreateVolcanoHeroTrackingOnHeroes()
    {
        List<HeroBase> heroes = HeroesManager.Instance.GetCurrentHeroes();
        for (int i = 0; i < heroes.Count; i++)
        {
            VolcanoHeroMovementTracking volcanoHeroMovementTracking = 
                Instantiate(_volcanoHeroTrackingObject,heroes[i].transform.position, Quaternion.identity).GetComponent<VolcanoHeroMovementTracking>();

            volcanoHeroMovementTracking.SetUpVolcanoTracking(this, heroes[i]);
            
            _volcanoHeroMovementTracking[i] = volcanoHeroMovementTracking;
        }
    }

    public void VolcanoTargetHitMax(VolcanoHeroMovementTracking volcanoHeroMovementTracking)
    {
        BossTargetZoneParent targetZoneParent = 
            Instantiate(_volcanoFutureTargetZone, volcanoHeroMovementTracking.transform.position, Quaternion.identity).GetComponent<BossTargetZoneParent>();

        targetZoneParent.transform.position =
            EnvironmentManager.Instance.GetClosestPointToFloor(targetZoneParent.transform.position);
        
        /*targetZoneParent.transform.position = new Vector3(targetZoneParent.transform.position.x,
            targetZoneParent.transform.position.y - _specificAreaTarget.y, targetZoneParent.transform.position.z);*/
        
        _currentVolcanoFutureTargetZones.Add(targetZoneParent);
        _targetLocations.Add(targetZoneParent.transform.position);

        PlayVolcanoFutureTargetZoneSpawnedAudio();
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
            
            _currentTargetZones.Add(Instantiate(_targetZone, _currentActiveTargetLocations[i], Quaternion.identity).GetComponent<BossTargetZoneParent>());

            if (i != 0)
            {
                PlayTargetZoneSpawnedAudio();
            }
            
            yield return _projectileDelayWaitForSeconds;
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
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[VOLCANO_FUTURE_TARGET_ZONE_SPAWNED_AUDIO_ID]);
    }

    private void PlayVolcanoAbilityAudio(int volcanoSpawned)
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[VOLCANO_IMPACT_AUDIO_ID], out EventInstance eventInstance);
        
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
        //DetermineAttackLocations();
        
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

        float requiredMinChance = (float)_currentVolcanoFutureTargetZones.Count / (float)_futureTargetZonesCertainChanceToActivate;
        float randomChance = Random.Range(0, 1f);
        
        return randomChance <= requiredMinChance;
    }
        
    

    #endregion
}