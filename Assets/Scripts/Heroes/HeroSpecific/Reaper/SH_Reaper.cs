using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Reaper : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicAbilityProjectile;

    [SerializeField] private GameObject _manualAbilityProjectile;

    #region Basic Abilities
    protected override void StartCooldownBasicAbility()
    {
        ActivateBasicAbilities();
    }

    public override void ActivateBasicAbilities()
    {
        CreateBasicAbilityProjectile();
    }

    private void CreateBasicAbilityProjectile()
    {
        //Creates the projectile at the hero location
        GameObject spawnedProjectile = Instantiate(_basicAbilityProjectile, transform.position, Quaternion.identity);

        //Does the universal projectile setup
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        //Does the projectile specific setup
        spawnedProjectile.GetComponent<SHA_ReaperBasicProjectile>().AdditionalSetup();
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
        //Creates the projectile where the mouse is
        GameObject spawnedProjectile = Instantiate(_manualAbilityProjectile, attackLocation, Quaternion.identity);

        //Does the universal projectile setup
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        //Does the projectile specific setup
        spawnedProjectile.GetComponent<SHA_ReaperManualProjectile>().AdditionalSetup();
    
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
