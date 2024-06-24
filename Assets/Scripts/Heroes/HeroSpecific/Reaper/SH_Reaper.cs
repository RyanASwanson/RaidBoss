using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Reaper : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicAbilityProjectile;

    [SerializeField] private GameObject _manualAbilityProjectile;

    [Space]
    [SerializeField] private float _deathPersistDuration;

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
        spawnedProjectile.GetComponent<SHP_ReaperBasicProjectile>().AdditionalSetup();
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
        attackLocation = new Vector3(attackLocation.x, transform.position.y, attackLocation.z);

        //Creates the projectile where the mouse is
        GameObject spawnedProjectile = Instantiate(_manualAbilityProjectile, attackLocation, Quaternion.identity);

        //Does the universal projectile setup
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        //Does the projectile specific setup
        spawnedProjectile.GetComponent<SHP_ReaperManualProjectile>().AdditionalSetup();
    
    }
    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        StartCoroutine(PassiveProcess());
    }

    private IEnumerator PassiveProcess()
    {
        myHeroBase.GetHeroStats().AddDamageTakenOverrideCounter();
        myHeroBase.GetHeroStats().AddHealingTakenOverrideCounter();
        yield return new WaitForSeconds(_deathPersistDuration);
        myHeroBase.GetHeroStats().RemoveDeathOverrideCounter();
        myHeroBase.GetHeroStats().KillHero();
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
    
    protected override void BattleStarted()
    {
        base.BattleStarted();

        myHeroBase.GetHeroStats().AddDeathOverrideCounter();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        myHeroBase.GetHeroDeathOverrideEvent().AddListener(ActivatePassiveAbilities);
    }
}
