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

    protected int _abilityID;

    /// <summary>
    /// Tells the boss projectile it's owner
    /// </summary>
    /// <param name="bossBase"></param>
    /// <param name= "newAbilityID"></param>
    public virtual void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        _myBossBase = bossBase;
        _mySpecificBoss = bossBase.GetSpecificBossScript();
        _abilityID = newAbilityID;
    }
}
