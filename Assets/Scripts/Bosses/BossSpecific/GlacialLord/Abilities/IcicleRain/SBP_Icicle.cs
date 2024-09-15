using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Icicle : BossProjectileFramework
{
    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private LayerMask _groundLayer;
    
    [SerializeField] private GlacialLordSelfMinionHit _minionHit;

    private bool _didHitMinion;

    public void IcicleContactCheck()
    {
        _didHitMinion = _minionHit.MinionContactFromDistance();

        DestructionCheck();
    }

    private void DestructionCheck()
    {
        print(_didHitMinion + " " + GroundHit());
        if(_didHitMinion || GroundHit())
            Destroy(gameObject);
    }

    private bool GroundHit()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundLayer);
    }

    public override void SetUpProjectile(BossBase bossBase)
    {
        base.SetUpProjectile(bossBase);
    }


}
