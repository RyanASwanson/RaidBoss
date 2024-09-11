using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_GlacialLord : SpecificBossFramework
{
    [Space]
    [SerializeField] private float _delayBetweenFiendSpawns;

    [Space]
    [SerializeField] private GameObject _frostFiend;
    [SerializeField] private List<Vector3> _frostFiendSpawnLocations;
    private List<GlacialLord_FrostFiend> _allFrostFiends = new();


    protected override void SetupReadyBossAbilities()
    {
        base.SetupReadyBossAbilities();
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
        GameObject newFiend = Instantiate(_frostFiend, spawnLocation, Quaternion.identity);
        _allFrostFiends.Add(newFiend.GetComponent<GlacialLord_FrostFiend>());
    }

    #endregion

    #region Getters

    #endregion
}
