using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_IcicleSet : BossProjectileFramework
{
    [SerializeField] private SBP_Icicle[] _icicleSet;

    private void SetUpIcicleSet()
    {
        foreach (SBP_Icicle icicle in _icicleSet)
        {
            icicle.gameObject.SetActive(true);
            icicle.SetUpProjectile(_myBossBase,_abilityID);
        }
    }
    
    #region Base Projectile
    
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        SetUpIcicleSet();
    }
    
    #endregion
}
