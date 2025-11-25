using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Icicle : BossProjectileFramework
{
    [SerializeField] private float _groundCheckDistance;

    [SerializeField] private GeneralBossDamageArea _damageArea;
    
    [SerializeField] private GlacialLordSelfMinionHit _minionHit;

    public void IcicleContactCheck()
    {
        float distance = Mathf.Abs(transform.position.x) + Mathf.Abs(transform.position.z);

        if (_minionHit.MinionContactFromDistance() 
            || distance < EnvironmentManager.Instance.GetMapRadius())
        {
            _damageArea.DestroyProjectile();
        }

    }
}
