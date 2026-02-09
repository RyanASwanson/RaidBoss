using System;
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
    [SerializeField] private float _manualBufferDuration;
    [SerializeField] private float _manualAbilityHealingIncrease;

    [Space] 
    [SerializeField] private GeneralRotation _batSpiralRotation;
    [SerializeField] private CustomObjectEmitter _manualObjectEmitter;
    private WaitForSeconds _manualBufferWait;
    private WaitForSeconds _manualAbilityWait;

    [Space]
    [SerializeField] private float _passiveAbilityLifestealMultiplier;
    [SerializeField] private float _passiveHealingDelay;
    private WaitForSeconds _passiveAbilityWait;

    [SerializeField] private GameObject _passiveHealProjectile;

    private float _currentPassiveHealingStored;
    private float _recentPassiveHealing;

    private Coroutine _passiveProcess;

    public const int BASIC_PROJECTILE_SPLIT_AUDIO_ID = 0;

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
        projectileFunc.SetUpProjectile(_myHeroBase, EHeroAbilityType.Basic);
        projectileFunc.AdditionalSetup(this);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }
    #endregion

    #region Manual Abilities

    public override void ActivateManualAbilities()
    {
        base.ActivateManualAbilities();

        StartCoroutine(ManualAbilityProcess());
    }

    protected IEnumerator ManualAbilityProcess()
    {
        HeroStats heroStats = _myHeroBase.GetHeroStats();

        heroStats.AddDamageTakenOverrideCounter();
        heroStats.ChangeCurrentHeroHealingReceivedMultiplier(_manualAbilityHealingIncrease);
        
        yield return _manualBufferWait;
        
        _manualObjectEmitter.StartEmittingObject();

        yield return _manualAbilityWait;

        heroStats.RemoveDamageTakenOverrideCounter();
        heroStats.ChangeCurrentHeroHealingReceivedMultiplier(-_manualAbilityHealingIncrease);
    }

    #endregion

    #region Passive Abilities
    /// <summary>
    /// Stores damage dealt as healing and starts the process of activating it
    /// </summary>
    /// <param name="damageDealt"> The amount of damage that was dealt </param>
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
        yield return _passiveAbilityWait;
        //ActivatePassiveAbilities();
        CreatePassiveProjectile();

        _passiveProcess = null;
    }

    private void CreatePassiveProjectile()
    {
        GameObject spawnedProjectile = Instantiate(_passiveHealProjectile, BossBase.Instance.transform.position, Quaternion.identity);
        
        SHP_VampirePassiveProjectile projectileFunc = spawnedProjectile.GetComponent<SHP_VampirePassiveProjectile>();
        projectileFunc.SetUpProjectile(_myHeroBase, EHeroAbilityType.Basic);
        projectileFunc.AdditionalSetup(this, _currentPassiveHealingStored, _myHeroBase.GetHeroStats().GetCurrentHealingReceivedMultiplier());
        
        _currentPassiveHealingStored = 0;
    }

    public void ActivatePassiveHeal(float healing)
    {
        _recentPassiveHealing = healing;
        ActivatePassiveAbilities();
    }

    /// <summary>
    /// The stored healing is used to heal the hero
    /// </summary>
    public override void ActivatePassiveAbilities()
    {
        base.ActivatePassiveAbilities();

        HealTargetHero(_recentPassiveHealing,_myHeroBase);
    }
    #endregion

    #region Base Hero
    /// <summary>
    /// Performs any needed set up for the hero
    /// </summary>
    /// <param name="heroBase"> The associated hero base </param>
    /// <param name="heroSO"> The associated hero scriptable object </param>
    public override void SetUpSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        base.SetUpSpecificHero(heroBase, heroSO);
        
        _batSpiralRotation.SetRotationIndependentParent(_myHeroBase.gameObject);
            
        _manualBufferWait = new WaitForSeconds(_manualBufferDuration);
        _manualAbilityWait = new WaitForSeconds(_manualAbilityDuration-_manualBufferDuration);
        _passiveAbilityWait = new WaitForSeconds(_passiveHealingDelay);
    }
    
    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroDealtDamageEvent().AddListener(AddToPassiveHealingCounter);
    }
    #endregion
    
}
