using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Icicle : BossProjectileFramework
{
    [SerializeField] private GlacialLordSelfMinionHit _minionHit;

    public override void SetUpProjectile(BossBase bossBase)
    {
        base.SetUpProjectile(bossBase);
    }
}
