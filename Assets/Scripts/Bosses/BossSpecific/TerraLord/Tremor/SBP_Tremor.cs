using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Tremor : BossProjectileFramework
{
    [SerializeField] private float _spikeSeperationTime;

    [SerializeField] private List<GameObject> _spikeSets;


    private IEnumerator SpikeSpawningProcess()
    {
        for(int i = 0; i <= _spikeSets.Count-1; i++)
        {
            _spikeSets[i].SetActive(true);
            yield return new WaitForSeconds(_spikeSeperationTime);
        }
    }


    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase)
    {
        base.SetUpProjectile(bossBase);
        StartCoroutine(SpikeSpawningProcess());
    }
    #endregion
}
