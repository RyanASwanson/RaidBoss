using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Reaper : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [SerializeField] private GameObject _manualProjectile;

    [Space]
    [SerializeField] private float _deathPersistDuration;

    #region Basic Abilities
    protected override void StartCooldownBasicAbility()
    {
        ActivateBasicAbilities();
    }

    public override void ActivateBasicAbilities()
    {
        //Doesn't use base.basic ability

        CreateBasicAbilityProjectile();
    }

    private void CreateBasicAbilityProjectile()
    {
        //Creates the projectile at the hero location
        GameObject spawnedProjectile = Instantiate(_basicProjectile, transform.position, Quaternion.identity);

        //Does the universal projectile setup
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
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
        GameObject spawnedProjectile = Instantiate(_manualProjectile, attackLocation, Quaternion.identity);

        //Does the universal projectile setup
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);

    }
    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        StartCoroutine(PassiveProcess());
    }

    /// <summary>
    /// Upon the death override ocurring the Reaper stops taking damage and healing,
    /// persists for a set period of time, then has the death override removed and dies
    /// </summary>
    /// <returns></returns>
    private IEnumerator PassiveProcess()
    {
        _myHeroBase.GetHeroStats().AddDamageTakenOverrideCounter();
        _myHeroBase.GetHeroStats().AddHealingTakenOverrideCounter();
        yield return new WaitForSeconds(_deathPersistDuration);
        _myHeroBase.GetHeroStats().RemoveDeathOverrideCounter();
        _myHeroBase.GetHeroStats().KillHero();
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

        _myHeroBase.GetHeroStats().AddDeathOverrideCounter();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroDeathOverrideEvent().AddListener(ActivatePassiveAbilities);
    }
}
