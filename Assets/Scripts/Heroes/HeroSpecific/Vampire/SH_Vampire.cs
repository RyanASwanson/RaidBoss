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

        spawnedProjectile.transform.LookAt(GameplayManagers.Instance.GetBossManager().GetBossBaseGameObject().transform);
        spawnedProjectile.transform.eulerAngles = new Vector3(0, spawnedProjectile.transform.eulerAngles.y, 0);

        //Does the universal projectile setup
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }


    #endregion

    #region Manual Abilities

    #endregion

    #region Passive Abilities

    #endregion
}
