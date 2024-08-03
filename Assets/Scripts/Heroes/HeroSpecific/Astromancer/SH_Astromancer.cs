using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Astromancer : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private GameObject _manualProjectiles;

    [Space]
    [SerializeField] private float _passiveRechargeManualAmount;

    #region Basic Abilities
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    protected void CreateBasicAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_basicProjectile, transform.position, Quaternion.identity);

        SHP_AstromancerBasicProjectile projectileFunc = spawnedProjectile.GetComponent<SHP_AstromancerBasicProjectile>();
        projectileFunc.SetUpProjectile(_myHeroBase);

        Vector3 storedProjectileDirection = GameplayManagers.Instance.GetBossManager().GetDirectionToBoss(transform.position);
        projectileFunc.AdditionalSetup(this, storedProjectileDirection);

        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        spawnedProjectile.GetComponent<GeneralHeroHealArea>().SetUpHealingArea(_myHeroBase);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        CreateManualAttackProjectiles();
    }

    protected void CreateManualAttackProjectiles()
    {
    }

    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        AddToManualAbilityChargeTime(_passiveRechargeManualAmount);
    }

    #endregion

}

