using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Reaper : SpecificHeroFramework
{
    #region Basic Abilities
    public override bool ConditionsToActivateBasicAbilities()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        throw new System.NotImplementedException();
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
