using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Icicle : BossProjectileFramework
{
    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private GeneralBossDamageArea _damageArea;
    
    [SerializeField] private GlacialLordSelfMinionHit _minionHit;

    private bool _didHitMinion;

    public void IcicleContactCheck()
    {
        _didHitMinion = _minionHit.MinionContactFromDistance();

        DestructionCheck();
    }

    private void DestructionCheck()
    {
        if (_didHitMinion || GroundHit())
            _damageArea.DestroyProjectile();
    }

    private bool GroundHit()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundLayer);
    }
    
}
