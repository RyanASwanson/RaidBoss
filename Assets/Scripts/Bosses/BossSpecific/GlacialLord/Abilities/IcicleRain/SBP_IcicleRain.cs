using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_IcicleRain : BossProjectileFramework
{
    [SerializeField] private float _iciclesSeperationTime;
    [SerializeField] private int _iciclesPerSet;

    [SerializeField] private List<GameObject> _icicles;

    private IEnumerator SpikeSpawningProcess()
    {
        int setCounter = 1;
        for (int i = 0; i <= _icicles.Count - 1; i++)
        {
            SpawnIcicle(_icicles[i]);

            setCounter++;

            if(setCounter > _iciclesPerSet)
            {
                setCounter = 1;
                yield return new WaitForSeconds(_iciclesSeperationTime);
            }
            
        }
    }


    private void SpawnIcicle(GameObject icicle)
    {
        icicle.GetComponent<SBP_Icicle>().SetUpProjectile(_myBossBase);
        icicle.SetActive(true);
    }


    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase)
    {
        base.SetUpProjectile(bossBase);
        StartCoroutine(SpikeSpawningProcess());
    }
    #endregion
}
