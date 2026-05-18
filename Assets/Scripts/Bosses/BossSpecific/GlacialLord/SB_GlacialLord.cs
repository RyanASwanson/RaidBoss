using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SB_GlacialLord : SpecificBossFramework
{
    public static SB_GlacialLord Instance;
    
    [Space]
    [SerializeField] private float _delayBetweenFiendSpawns;
    [Space]
    [SerializeField] private float _minionFreezeDuration;

    [SerializeField] private float[] _frostFiendDifficultyFreezeDurationMultiplier;

    [SerializeField] private float _minionEnrageUnfreezeSpeedMultiplier;
    [SerializeField] private float _minionEnrageUnfreezeSpeedScalingMulitplierIncreasePerMinute;
    
    private float _currentMinionUnfreezeSpeedMultiplier = 1;

    [Space]
    [SerializeField] private GameObject _frostFiend;
    [SerializeField] private List<Vector3> _frostFiendSpawnLocations;
    private List<GlacialLord_FrostFiend> _allFrostFiends = new();

    private UnityEvent<GlacialLord_FrostFiend> _frostFiendSpawned = new();

    private void CalculateEnrageUnfreezeMultiplier()
    {
        _currentMinionUnfreezeSpeedMultiplier = _minionEnrageUnfreezeSpeedMultiplier;
        _currentMinionUnfreezeSpeedMultiplier *= 1 +  (_minionEnrageUnfreezeSpeedScalingMulitplierIncreasePerMinute *
                                                 BossStats.Instance.GetMinutesSpentEnraged());
    }
    
    
    #region Frost Fiends
    private IEnumerator SpawnStartingFrostFiends()
    {
        WaitForSeconds frostFiendSpawnWait = new WaitForSeconds(_delayBetweenFiendSpawns);
        
        yield return frostFiendSpawnWait;
        foreach(Vector3 spawnLocation in _frostFiendSpawnLocations)
        {
            SpawnFrostFiend(spawnLocation);
            yield return frostFiendSpawnWait;
        }
    }

    private void SpawnFrostFiend(Vector3 spawnLocation)
    {
        GlacialLord_FrostFiend newFiend = 
            Instantiate(_frostFiend, spawnLocation, Quaternion.identity).GetComponent<GlacialLord_FrostFiend>();

        newFiend.transform.LookAt(transform);
        
        //Set does not work
        newFiend.transform.eulerAngles = new Vector3(0, newFiend.transform.eulerAngles.y, 0);

        newFiend.SetUpMinion(_myBossBase, this);
        newFiend.SetUpMinionFreezeDuration(_minionFreezeDuration);

        _allFrostFiends.Add(newFiend);

        InvokeFrostFiendSpawned(newFiend);
    }

    private void FrostFiendDeath()
    {
        foreach (GlacialLord_FrostFiend frostFiend in _allFrostFiends)
        {
            frostFiend.FrostFiendDeath();
        }
    }

    public void FreezeAllFrostFiends()
    {
        foreach (GlacialLord_FrostFiend frostFiend in _allFrostFiends)
        {
            frostFiend.FreezeMinion();
        }
    }
    #endregion

    #region BaseBoss
    protected override void CreateSpecificBossInstance()
    {
        Instance = this;
    }
    
    public override void SetUpSpecificBoss(BossBase bossBase)
    {
        base.SetUpSpecificBoss(bossBase);
        
        _minionFreezeDuration *=
            _frostFiendDifficultyFreezeDurationMultiplier[SelectionManager.Instance.GetSelectedDifficultyID()];
        StartCoroutine(SpawnStartingFrostFiends());
    }
    
    protected override void CheckToUnlockSpecialistAchievement()
    {
        base.CheckToUnlockSpecialistAchievement();
        
        if (SelectionManager.Instance.GetSelectedDifficulty() < EGameDifficulty.Mythic)
        {
            return;
        }

        foreach (GlacialLord_FrostFiend fiend in _allFrostFiends)
        {
            if (!fiend.GetHasFrostFiendAttacked())
            {
                UnlockedSpecialistAchievement();
                return;
            }
        }
        
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        GameStateManager.Instance.GetBattleWonEvent().AddListener(FrostFiendDeath);
        _myBossBase.GetSecondPassedEnrageEvent().AddListener(CalculateEnrageUnfreezeMultiplier);
    }

    #endregion

    #region Events
    private void InvokeFrostFiendSpawned(GlacialLord_FrostFiend frostFiend)
    {
        _frostFiendSpawned?.Invoke(frostFiend);
    }
    #endregion

    #region Getters
    public float GetMinionFreezeDuration() => _minionFreezeDuration;
    
    public float GetMinionUnfreezeSpeedMultiplier() => _currentMinionUnfreezeSpeedMultiplier;
    
    public List<Vector3> GetFrostFiendSpawnLocations() => _frostFiendSpawnLocations;
    public List<GlacialLord_FrostFiend> GetAllFrostFiends() => _allFrostFiends;

    public UnityEvent<GlacialLord_FrostFiend> GetFrostFiendSpawnedEvent() => _frostFiendSpawned;
    #endregion
    
    #region Setters

    public void SetMinionFreezeDuration(float freezeDuration)
    {
        foreach (GlacialLord_FrostFiend frostFiend in _allFrostFiends)
        {
            frostFiend.SetUpMinionFreezeDuration(freezeDuration);
        }
    }
    #endregion
}
