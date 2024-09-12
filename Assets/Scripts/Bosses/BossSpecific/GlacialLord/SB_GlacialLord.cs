using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void SetupSpecificBoss(BossBase bossBase)
    {
        base.SetupSpecificBoss(bossBase);
        StartCoroutine(SpawnStartingFrostFiends());
    }


    #region Frost Fiends
    private IEnumerator SpawnStartingFrostFiends()
    {
        foreach(Vector3 spawnLocation in _frostFiendSpawnLocations)
        {
            SpawnFrostFiend(spawnLocation);
            yield return new WaitForSeconds(_delayBetweenFiendSpawns);
        }
    }

    private void SpawnFrostFiend(Vector3 spawnLocation)
    {
        GlacialLord_FrostFiend newFiend = 
            Instantiate(_frostFiend, spawnLocation, Quaternion.identity).GetComponent<GlacialLord_FrostFiend>();

        newFiend.SetupMinion(_myBossBase, this);
        newFiend.AdditionalSetup(_minionFreezeDuration);

        _allFrostFiends.Add(newFiend.GetComponent<GlacialLord_FrostFiend>());
    }

    #endregion

    #region Getters
    public List<Vector3> GetFrostFiendSpawnLocations() => _frostFiendSpawnLocations;
    public List<GlacialLord_FrostFiend> GetAllFrostFiends() => _allFrostFiends;
    #endregion
}
