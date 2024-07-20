using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Entomb : BossProjectileFramework
{
    [SerializeField] private GameObject _entombWall;
    

    public override void SetUpProjectile(BossBase bossBase)
    {
        WallCreationCheck();
        base.SetUpProjectile(bossBase);
    }



    private void WallCreationCheck()
    {
        //RaycastHit rayHit = Physics.Bo
    }

    private void CreateWalls()
    {
        GameObject wallObject = Instantiate(_entombWall, transform.position, transform.rotation);
        wallObject.GetComponent<SBP_EntombWalls>().SetUpProjectile(_myBossBase);
    }
}
