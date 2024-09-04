using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The framework that boss projectiles inherit from
/// </summary>
public abstract class BossProjectileFramework : MonoBehaviour
{
    protected BossBase _myBossBase;
    protected SpecificBossFramework _mySpecificBoss;

    /// <summary>
    /// Tells the boss projectile it's owner
    /// </summary>
    /// <param name="bossBase"></param>
    public virtual void SetUpProjectile(BossBase bossBase)
    {
        _myBossBase = bossBase;
        _mySpecificBoss = bossBase.GetSpecificBossScript();
    }
}
