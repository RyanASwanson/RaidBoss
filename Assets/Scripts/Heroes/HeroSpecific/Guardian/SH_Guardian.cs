using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Guardian : SpecificHeroFramework
{
    [Space]
    [SerializeField] private float _heroBasicAbilityDamage;
    [SerializeField] private float _heroBasicAbilityStagger;
    [SerializeField] private GameObject _basicProjectile;

    [SerializeField] private float _heroManualAbilityDuration;

    #region Basic Abilities
    public override bool ConditionsToActivateBasicAbilities()
    {
        return !myHeroBase.GetPathfinding().IsHeroMoving();  
    }
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        Instantiate(_basicProjectile, myHeroBase.transform.position, Quaternion.identity);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetSpecificBossScript()
            .HeroOverrideAggro(myHeroBase, _heroManualAbilityDuration);
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
