using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Chronomancer : SpecificHeroFramework
{
    #region Basic Abilities

    public override bool ConditionsToActivateBasicAbilities()
    {
        return true;
    }

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();
        Debug.Log("Activate Chrono Basic Abilities");

        DamageBoss(myHeroBase.GetHeroStats().GetDefaultBasicAbilityStrength());
        StaggerBoss(myHeroBase.GetHeroStats().GetDefaultBasicAbilityStagger());
    }


    #endregion
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        throw new System.NotImplementedException();
    }

    public override void ActivatePassiveAbilities()
    {
        throw new System.NotImplementedException();
    }

    

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
