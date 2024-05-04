using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Reaper : SpecificHeroFramework
{
    [SerializeField] private GameObject _manualAbilityProjectile;
    [SerializeField] protected float _manualProjectileLifetime;
    [SerializeField] protected float _manualProjectileSpeed;

    #region Basic Abilities
    public override bool ConditionsToActivateBasicAbilities()
    {
        return true;
    }
    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);
        CreateManualAbilityProjectile(attackLocation);
    }

    private void CreateManualAbilityProjectile(Vector3 attackLocation)
    {
        GameObject spawnedProjectile = Instantiate(_manualAbilityProjectile, attackLocation, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        spawnedProjectile.GetComponent<SHA_ReaperActiveProjectile>().AdditionalSetup
            (_manualProjectileLifetime,_manualProjectileSpeed);
    
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
