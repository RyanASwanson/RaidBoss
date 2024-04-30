using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Guardian : SpecificHeroFramework
{
    #region Basic Abilities
    public override bool ConditionsToActivateBasicAbilities()
    {
        if (InAttackRangeOfBoss(myHeroBase.GetHeroStats().GetBasicAbilityRangeWithMultipliers()))
        {
            return true;
        }
        return false;  
    }
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();
        Debug.Log("Activate Basic Abilities");

        DamageBoss(myHeroBase.GetHeroStats().GetDefaultBasicAbilityStrength());
        StaggerBoss(myHeroBase.GetHeroStats().GetDefaultBasicAbilityStagger());
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        
    }
    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        
    }
    #endregion

    public override void ActivateHeroSpecificActivity()
    {
        base.ActivateHeroSpecificActivity();
    }

    public override void DeactivateHeroSpecificActivity()
    {
        base.DeactivateHeroSpecificActivity();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }

    
}
