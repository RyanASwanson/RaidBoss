using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossProjectileFramework : MonoBehaviour
{
    protected BossBase _ownerBossBase;
    protected SpecificBossFramework _mySpecificBoss;

    public virtual void SetUpProjectile(BossBase bossBase)
    {
        _ownerBossBase = bossBase;
        _mySpecificBoss = bossBase.GetSpecificBossScript();
    }
}
