using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SB_GlacialLord : SpecificBossFramework
{
    [Space]
    [SerializeField] private float _delayBetweenFiendSpawns;
    [Space]
    [SerializeField] private float _minionFreezeDuration;

    [Space]
    [SerializeField] private GameObject _frostFiend;
    [SerializeField] private List<Vector3> _frostFiendSpawnLocations;
    private List<GlacialLord_FrostFiend> _allFrostFiends = new();

    private UnityEvent<GlacialLord_FrostFiend> _frostFiendSpawned = new();

    public override void SetUpSpecificBoss(BossBase bossBase)
    {
        base.SetUpSpecificBoss(bossBase);
        StartCoroutine(SpawnStartingFrostFiends());
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
        newFiend.AdditionalSetUp(_minionFreezeDuration);

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
    #endregion

    #region BaseBoss

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        GameStateManager.Instance.GetBattleWonEvent().AddListener(FrostFiendDeath);
    }

    #endregion

    #region Events
    private void InvokeFrostFiendSpawned(GlacialLord_FrostFiend frostFiend)
    {
        _frostFiendSpawned?.Invoke(frostFiend);
    }
    #endregion

    #region Getters
    public List<Vector3> GetFrostFiendSpawnLocations() => _frostFiendSpawnLocations;
    public List<GlacialLord_FrostFiend> GetAllFrostFiends() => _allFrostFiends;

    public UnityEvent<GlacialLord_FrostFiend> GetFrostFiendSpawnedEvent() => _frostFiendSpawned;
    #endregion
}
