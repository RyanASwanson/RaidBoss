using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Guardian : SpecificHeroFramework
{
    [SerializeField] private float _heroDefaultBasicAbilityRange;

    #region Basic Abilities
    public override bool ConditionsToActivateBasicAbilities()
    {

        return InAttackRangeOfBoss(_heroDefaultBasicAbilityRange) && 
            !myHeroBase.GetPathfinding().IsHeroMoving();  
    }
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();
        Debug.Log("Activate Basic Abilities");

        DamageBoss(_heroBasicAbilityStrength);
        StaggerBoss(_heroBasicAbilityStagger);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);
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