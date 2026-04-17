using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_ImpendingStorm : BossProjectileFramework
{
    [SerializeField] private GeneralTranslate _generalTranslate;
    [SerializeField] private CurveProgression _accelerationCurve;

    public void ImpendingStormHit()
    {
        SB_ThunderLord.Instance.SetHasImpendingStormHit(true);
    }
    
    #region BaseProjectile
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase,newAbilityID);
        
        _accelerationCurve.StartMovingUpOnCurve();
        _generalTranslate.StartMoving(transform.forward);
    }
    
    #endregion
}
