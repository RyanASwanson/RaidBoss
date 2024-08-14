using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specific hero script for the Vampire
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

        spawnedProjectile.transform.LookAt(GameplayManagers.Instance.GetBossManager().GetBossBaseGameObject().transform);
        spawnedProjectile.transform.eulerAngles = new Vector3(0, spawnedProjectile.transform.eulerAngles.y, 0);

        //Does the universal projectile setup
        SHP_VampireBasicProjectile projectileFunc = spawnedProjectile.GetComponent<SHP_VampireBasicProjectile>();
        projectileFunc.SetUpProjectile(_myHeroBase);
        projectileFunc.AdditionalSetup(this);

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

    public void AddToPassiveHealingCounter(float damageDealt)
    {
        _currentPassiveHealingStored += damageDealt * _passiveAbilityLifestealMultiplier;
        
        if (_passiveProcess != null) StopCoroutine(_passiveProcess);

        _passiveProcess = StartCoroutine(PassiveProcess());
    }

    private IEnumerator PassiveProcess()
    {
        yield return new WaitForSeconds(_passiveHealingDelay);
        ActivatePassiveAbilities();

        _passiveProcess = null;
    }


    public override void ActivatePassiveAbilities()
    {
        base.ActivatePassiveAbilities();

        _myHeroBase.GetHeroStats().HealHero(_currentPassiveHealingStored);
        _currentPassiveHealingStored = 0;
    }
    #endregion

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _heroDealtDamageEvent?.AddListener(AddToPassiveHealingCounter);
    }
}
