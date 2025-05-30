using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Vampire hero
/// </summary>
public class SH_Vampire : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private float _manualAbilityDuration;
    [SerializeField] private float _manualAbilityHealingIncrease;

    [Space]
    [SerializeField] private float _passiveAbilityLifestealMultiplier;
    [SerializeField] private float _passiveHealingDelay;

    private float _currentPassiveHealingStored;

    private Coroutine _passiveProcess;

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

        spawnedProjectile.transform.LookAt(BossBase.Instance.transform);
        spawnedProjectile.transform.eulerAngles = new Vector3(0, spawnedProjectile.transform.eulerAngles.y, 0);

        //Does the universal projectile set up
        SHP_VampireBasicProjectile projectileFunc = spawnedProjectile.GetComponent<SHP_VampireBasicProjectile>();
        projectileFunc.SetUpProjectile(_myHeroBase);
        projectileFunc.AdditionalSetup(this);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }
    #endregion

    #region Manual Abilities

    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        StartCoroutine(ManualAbilityProcess());
    }

    protected IEnumerator ManualAbilityProcess()
    {
        HeroStats heroStats = _myHeroBase.GetHeroStats();

        heroStats.AddDamageTakenOverrideCounter();
        heroStats.ChangeCurrentHeroHealingReceivedMultiplier(_manualAbilityHealingIncrease);

        yield return new WaitForSeconds(_manualAbilityDuration);

        heroStats.RemoveDamageTakenOverrideCounter();
        heroStats.ChangeCurrentHeroHealingReceivedMultiplier(-_manualAbilityHealingIncrease);
    }

    #endregion

    #region Passive Abilities
    /// <summary>
    /// Stores damage dealt as healing and starts the process of activating it
    /// </summary>
    /// <param name="damageDealt"></param>
    public void AddToPassiveHealingCounter(float damageDealt)
    {
        _currentPassiveHealingStored += damageDealt * _passiveAbilityLifestealMultiplier;

        if (!_passiveProcess.IsUnityNull())
        {
            StopCoroutine(_passiveProcess);
        }

        _passiveProcess = StartCoroutine(PassiveProcess());
    }

    /// <summary>
    /// Delays the activation of the passive.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PassiveProcess()
    {
        yield return new WaitForSeconds(_passiveHealingDelay);
        ActivatePassiveAbilities();

        _passiveProcess = null;
    }

    /// <summary>
    /// The stored healing is used to heal the hero
    /// </summary>
    public override void ActivatePassiveAbilities()
    {
        base.ActivatePassiveAbilities();

        HealTargetHero(_currentPassiveHealingStored,_myHeroBase);
        _currentPassiveHealingStored = 0;
    }
    #endregion

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroDealtDamageEvent().AddListener(AddToPassiveHealingCounter);
    }
}
