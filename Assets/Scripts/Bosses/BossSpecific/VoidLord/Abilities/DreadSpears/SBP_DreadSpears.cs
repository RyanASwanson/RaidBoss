using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SBP_DreadSpears : BossProjectileFramework
{
    [SerializeField] private float _projectileInterval;
    [SerializeField] private float _dreadSpearDuration;
    [SerializeField] private float _enrageSpearDurationIncrease;

    [Space]
    [SerializeField] private float _initialProjectileDistance;
    [SerializeField] private float _distancePerProjectile;
    [SerializeField] private float _edgeOfMapOffset;

    [Space] 
    [SerializeField] private float _minimumProjectileVariation;
    [SerializeField] private float _maximumProjectileVariation;
    [SerializeField] private Vector3 _projectileVariationDirection;

    [Space] 
    [SerializeField] private float _volumeDecreasePerSpear;
    [SerializeField] private float _maximumVolumeDecrease;
    
    private WaitForSeconds _projectileWait;
    private Vector3 _targetSpawnLocation;
    private float _targetSpawnDistance;
    private float _edgeOfMapDistance;
    private int _projectileCounter = 0;
    
    [Space]
    [SerializeField] private GameObject _dreadSpearsHolder;
    
    [Space]
    [SerializeField] private GameObject _dreadSpear;

    private Queue<SBP_DreadSpear> _spawnedSpears = new Queue<SBP_DreadSpear>();
    
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
        int projectileCounter = 0;
        while(_targetSpawnDistance < _edgeOfMapDistance)
        {
            SpawnProjectile(projectileCounter);
            projectileCounter++;
            yield return _projectileWait;
        }
        //StartCoroutine(SpikeDurationProcess());
    }

    /*private IEnumerator SpikeDurationProcess()
    {
        yield return new WaitForSeconds(3);
        StartCoroutine(SpikeRemovalProcess());
    }*/

    private void StartEarlySpikeRemovalProcess()
    {
        StartCoroutine(EarlySpikeRemovalProcess());
    }

    private IEnumerator EarlySpikeRemovalProcess()
    {
        while (_spawnedSpears.Count > 0)
        {
            _spawnedSpears.Peek().DreadSpearDurationOver();
            yield return _projectileWait;
        }
        
    }

    
    private void SpawnProjectile(int projectileCounter)
    {
        SBP_DreadSpear dreadSpear = Instantiate(_dreadSpear, _dreadSpearsHolder.transform).GetComponent<SBP_DreadSpear>();
        dreadSpear.transform.position = _targetSpawnLocation;
        
        dreadSpear.transform.localPosition += _projectileVariationDirection * 
                                              (Random.Range(_minimumProjectileVariation,_maximumProjectileVariation) *
                                               (projectileCounter % 2 == 1 ? -1 : 1));

        dreadSpear.SetUpProjectile(_myBossBase,_abilityID, _wasBossEnragedOnAbilityActivation);
        
        _spawnedSpears.Enqueue(dreadSpear);

        PlayDreadSpearStabSpawnAudio();
        _projectileCounter++;
        CalculateNextTargetSpawnLocation();
    }

    private void CalculateNextTargetSpawnLocation()
    {
        _targetSpawnLocation = transform.forward * ((_distancePerProjectile * _projectileCounter) + _initialProjectileDistance);
        _targetSpawnLocation.Set(_targetSpawnLocation.x, transform.position.y, _targetSpawnLocation.z);
        _targetSpawnDistance = Vector3.Distance(Vector3.zero, _targetSpawnLocation);
    }
    
    private void PlayDreadSpearStabSpawnAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_DreadSpears.DREAD_SPEAR_STAB_AUDIO_ID], out EventInstance eventInstance);
        
        eventInstance.getVolume(out float vol);
        eventInstance.setVolume(Mathf.Clamp(vol - (_projectileCounter * _volumeDecreasePerSpear),_maximumVolumeDecrease,int.MaxValue));
        
        /*eventInstance.getVolume(out vol);
        Debug.Log(vol);*/
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
