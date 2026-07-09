using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_RayOfHope : BossProjectileFramework
{
    [SerializeField] private GeneralBossBuffArea _bossBuffArea;
    
    public void RemoveRayOfHope()
    {
        Destroy(this.gameObject);
    }
    
    #region BaseProjectile
    
    #endregion
}
