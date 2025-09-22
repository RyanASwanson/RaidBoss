using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Reaper hero
/// </summary>
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
        //Doesn't use base.ActivateBasicAbilities to not start a new cooldown
        
        CreateBasicAbilityProjectile();
    }

    private void CreateBasicAbilityProjectile()
    {
        //Creates the projectile at the hero location
        GameObject spawnedProjectile = Instantiate(_basicProjectile, transform.position, Quaternion.identity);

        //Does the universal projectile set up
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        //Performs the set up for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities()
    {
        base.ActivateManualAbilities();
        CreateManualAbilityProjectile();
    }

    private void CreateManualAbilityProjectile()
    {
        Vector3 attackLocation = new Vector3(-transform.position.x, transform.position.y, -transform.position.z);

        //Creates the projectile where the mouse is
        GameObject spawnedProjectile = Instantiate(_manualProjectile, attackLocation, Quaternion.identity);

        //Does the universal projectile set up
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        //Performs the set up for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }
    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        base.ActivatePassiveAbilities();
        StartCoroutine(PassiveProcess());
    }

    /// <summary>
    /// Upon the death override occurring the Reaper stops taking damage and healing,
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
