using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Entomb : BossProjectileFramework
{
    [Space]
    [SerializeField] private float _wallCreationDelay;

    [SerializeField] private GameObject _entombWalls;

    private int _heroContactCounter;

    public override void SetUpProjectile(BossBase bossBase)
    {
        StartCoroutine(WallCreationProcess());
        base.SetUpProjectile(bossBase);
    }

    public void UpdateHeroContactCounter(int changeVal)
    {
        _heroContactCounter += changeVal;
    }

    private IEnumerator WallCreationProcess()
    {
        yield return new WaitForSeconds(_wallCreationDelay);
        if(_heroContactCounter < 1)
            CreateWalls();
    }

    private void CreateWalls()
    {
        GameObject wallObject = Instantiate(_entombWalls, transform.position, transform.rotation);
        wallObject.GetComponent<SBP_EntombWalls>().SetUpProjectile(_myBossBase);
    }
}
