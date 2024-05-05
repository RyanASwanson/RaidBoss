using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Shaman : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;
    [SerializeField] private float _basicProjectileLifetime;
    [SerializeField] protected float _basicProjectileDamageCooldown;

    [SerializeField] int _manualAbilityDuration;

    #region Basic Abilities
    public override bool ConditionsToActivateBasicAbilities()
    {
        return !myHeroBase.GetPathfinding().IsHeroMoving();
    }

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    protected void CreateBasicAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_basicProjectile, myHeroBase.transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        spawnedProjectile.GetComponent<SHA_ShamanBasicAbility>().AdditionalSetup
            (_basicProjectileLifetime, _basicProjectileDamageCooldown);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

    }

    #endregion

    #region Passive Abilities
    public void ActivatePassiveAbilities(float cooldownAmount)
    {
        AddToManualAbilityChargeTime(cooldownAmount);
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
