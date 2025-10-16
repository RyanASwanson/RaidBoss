using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Guardian hero
/// </summary>
public class SH_Guardian : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;
    
    [Space]
    [SerializeField] private float _heroManualAbilityDuration;
    private WaitForSeconds _heroManualAbilityWait;

    [SerializeField] private GameObject _tauntIcon;
    private GameObject _currentTauntIcon;
    private Animator _currentTauntIconAnimator;

    private const string TAUNT_ICON_SHOW_ANIM_BOOL = "Show";

    [Space]
    [SerializeField] private float _heroPassiveAbilityDuration;
    [Range(0,1)][SerializeField] private float _heroPassiveDamageResistance;
    private WaitForSeconds _heroPassiveWait;

    [SerializeField] private List<ParticleSystem> _passiveEyeVFX;
    private Coroutine _passiveCoroutine;

    #region Basic Abilities
    /// <summary>
    /// Spawns the projectile that deals damage
    /// </summary>
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        GameObject spawnedProjectile = Instantiate(_basicProjectile, 
            _myHeroBase.transform.position + EnvironmentManager.Instance.GetFloorOffset(), Quaternion.identity);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities()
    {
        BossBase.Instance.GetSpecificBossScript().StartHeroOverrideAggro(_myHeroBase, _heroManualAbilityDuration);
        StartCoroutine(ManualDuration());

        base.ActivateManualAbilities();
    }

    private IEnumerator ManualDuration()
    {
        _currentTauntIconAnimator.SetBool(TAUNT_ICON_SHOW_ANIM_BOOL, true);
        yield return _heroManualAbilityWait;
        _currentTauntIconAnimator.SetBool(TAUNT_ICON_SHOW_ANIM_BOOL, false);
    }
    #endregion

    #region Passive Abilities
    /// <summary>
    /// Activates the passive ability of the Guardian
    /// </summary>
    public override void ActivatePassiveAbilities()
    {
        base.ActivatePassiveAbilities();
        
        // If the passive is already active
        if (!_passiveCoroutine.IsUnityNull())
        {
            _myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(-_heroPassiveDamageResistance);
            StopCoroutine(_passiveCoroutine);
        }

        _passiveCoroutine = StartCoroutine(PassiveAbilityProcess());
    }

    private IEnumerator PassiveAbilityProcess()
    {
        _myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(_heroPassiveDamageResistance);
        ActivatePassiveVFX();
        yield return _heroPassiveWait;
        _myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(-_heroPassiveDamageResistance);

        _passiveCoroutine = null;
    }

    /// <summary>
    /// Activates the visual effects of the passive
    /// </summary>
    private void ActivatePassiveVFX()
    {
        foreach (ParticleSystem particleSystem in _passiveEyeVFX)
        {
            particleSystem.Play();
        }
    }
    #endregion

    #region Base Hero
    /// <summary>
    /// Performs set up for the hero
    /// </summary>
    /// <param name="heroBase"></param>
    /// <param name="heroSO"></param>
    public override void SetUpSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        base.SetUpSpecificHero(heroBase, heroSO);
        
        // Saves the WaitForSeconds of the passive to avoid needing to use new in the coroutine
        _heroPassiveWait = new WaitForSeconds(_heroPassiveAbilityDuration);
        
        // Saves the WaitForSeconds of the manual to avoid needing to use new in the coroutine
        _heroManualAbilityWait = new WaitForSeconds(_heroManualAbilityDuration);
    }

    protected override void BattleStarted()
    {
        base.BattleStarted();
        _currentTauntIcon = _myHeroBase.GetHeroUIManager().CreateObjectOnGeneralOrigin(_tauntIcon);
        _currentTauntIconAnimator = _currentTauntIcon.GetComponent<Animator>();
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        
        _myHeroBase.GetHeroDamagedEvent().AddListener(delegate{ActivatePassiveAbilities();});
    }

    #endregion
}
