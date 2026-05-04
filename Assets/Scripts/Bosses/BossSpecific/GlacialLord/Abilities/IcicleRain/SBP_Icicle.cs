using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Icicle : BossProjectileFramework
{
    [SerializeField] private float _groundCheckDistance;

    [Space] [SerializeField] private GeneralVFXFunctionality _fallingIcicleVfx;

    [Space]
    [SerializeField] private GeneralBossDamageArea _damageArea;
    
    [SerializeField] private GlacialLordSelfMinionHit _minionHit;

    public void MinionContactCheck()
    {
        if (_minionHit.MinionContactFromDistance())
        {
            DestroyIcicle();
        }
    }

    public void GroundContactCheck()
    {
        float distance = Mathf.Abs(transform.position.x) + Mathf.Abs(transform.position.z);
        if (distance < EnvironmentManager.Instance.GetMapRadius())
        {
            DestroyIcicle();
        }
    }

    private void DestroyIcicle()
    {
        _fallingIcicleVfx.SetLoopOfParticleSystems(false);
        _fallingIcicleVfx.DetachVisualEffect();
        _damageArea.DestroyProjectile();
    }
}