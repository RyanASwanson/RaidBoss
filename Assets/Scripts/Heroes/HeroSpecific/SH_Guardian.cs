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
        if (InAttackRangeOfBoss(_basicAbilityRange))
        {
            ActivateBasicAbilities();
            return true;
        }
        return false;  
    }
    public override void ActivateBasicAbilities()
    {
        Debug.Log("Activate Basic Abilities");
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

    public override void SubscribeToHeroSpecificEvents()
    {
        
    }

    public override void UnsubscribeToHeroSpecificEvents()
    {
        
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }

    
}
