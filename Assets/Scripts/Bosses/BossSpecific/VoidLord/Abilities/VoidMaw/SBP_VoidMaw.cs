using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_VoidMaw : BossProjectileFramework
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        //StartCoroutine(AbilityProcess());
    }
    #endregion
}
