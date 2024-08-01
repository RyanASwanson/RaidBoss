using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Vampire : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private float _manualAbilityDuration;
    [SerializeField] private float _manualAbilityHealingMultiplier;

    [Space]
    [SerializeField] private float _passiveAbilityLifestealMultiplier;

    #region Basic Abilities

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    protected void CreateBasicAttackProjectiles()
    {
        //Creates the projectile at the hero location
        GameObject spawnedProjectile = Instantiate(_basicProjectile, transform.position, Quaternion.identity);

        spawnedProjectile.transform.eulerAngles = GameplayManagers.Instance.GetBossManager().GetDirectionToBoss();


        //Does the universal projectile setup
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        //Does the projectile specific setup
        spawnedProjectile.GetComponent<SHP_ReaperBasicProjectile>().AdditionalSetup();

        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(myHeroBase);
    }


    #endregion

    #region Manual Abilities

    #endregion

    #region Passive Abilities

    #endregion
}
