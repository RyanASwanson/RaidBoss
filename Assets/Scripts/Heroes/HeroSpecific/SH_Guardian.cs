using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Guardian : SpecificHeroFramework
{
    #region Basic Abilities
    public override IEnumerator CheckingToAttemptBasicAbilities()
    {
        while (!AttemptBasicAbilities())
            yield return new WaitForFixedUpdate();
    }

    public override bool AttemptBasicAbilities()
    {
        //TESTING - UPDATE TO CURRENT ABILITY RANGE
        if (InAttackRangeOfBoss(myHeroBase.GetHeroStats().GetDefaultBasicAbilityRange()))
        {
            ActivateBasicAbilities();
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

    public override void ActivateToHeroSpecificActivity()
    {
        base.ActivateToHeroSpecificActivity();
    }

    public override void DeactivateToHeroSpecificActivity()
    {
        base.DeactivateToHeroSpecificActivity();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }

    
}
