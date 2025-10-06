using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Samurai hero
/// </summary>
public class SH_Samurai : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] float _manualAbilityDuration;
    [SerializeField] float _manualAbilityParryDamage;
    [SerializeField] float _manualAbilityParryStagger;
    [SerializeField] float _parryBonusIFrames;

    [Space]
    [SerializeField] float _passiveRechargeManualAmount;

    private Coroutine _parryCoroutine;

    private const string PARRY_SUCCESS_ANIM_TRIGGER = "ParrySuccess";

    private const int PARRY_SUCCESS_AUDIO_ID = 0;

    #region Basic Abilities

    /// <summary>
    /// Performs a horizontal slash
    /// </summary>
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    /// <summary>
    /// Creates the projectile of the horizontal slash
    /// </summary>
    protected void CreateBasicAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_basicProjectile, _myHeroBase.transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }

    #endregion

    #region Manual Abilities
    /// <summary>
    /// Called when manual button is pressed
    /// </summary>
    public override void ActivateManualAbilities()
    {
        base.ActivateManualAbilities();

        _parryCoroutine = StartCoroutine(ParryProcess());
    }

    private IEnumerator ParryProcess()
    {
        StartParry();

        yield return new WaitForSeconds(_manualAbilityDuration);

        EndParry();
        _parryCoroutine = null;
    }

    private void StartParry()
    {
        _myHeroBase.GetHeroStats().AddDamageTakenOverrideCounter();
        _myHeroBase.GetHeroDamagedOverrideEvent().AddListener(ParryAttack);
    }

    private void EndParry()
    {
        _myHeroBase.GetHeroStats().RemoveDamageTakenOverrideCounter();
        _myHeroBase.GetHeroDamagedOverrideEvent().RemoveListener(ParryAttack);
    }

    private void StopParryEarly()
    {
        StopCoroutine(_parryCoroutine);
        _parryCoroutine = null;
        _myHeroBase.GetHeroStats().RemoveDamageTakenOverrideCounter();
        _myHeroBase.GetHeroDamagedOverrideEvent().RemoveListener(ParryAttack);
    }

    public void ParryAttack(float damagePrevented)
    {
        DamageBoss(_manualAbilityParryDamage);
        StaggerBoss(_manualAbilityParryStagger);

        StopParryEarly();
        ActivatePassiveAbilities();

        SuccessfulParryAnimation();
        PlayParryAudio();

        StartSuccessfulParryIFrames();
    }

    private void PlayParryAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()]
                .MiscellaneousHeroAudio[PARRY_SUCCESS_AUDIO_ID]);
    }

    private void StartSuccessfulParryIFrames()
    {
        StartCoroutine(SuccessfulParryIFrames());
    }

    private void SuccessfulParryAnimation()
    {
        _myHeroBase.GetHeroVisuals().HeroSpecificAnimationTrigger(PARRY_SUCCESS_ANIM_TRIGGER);
    }

    private IEnumerator SuccessfulParryIFrames()
    {
        _myHeroBase.GetHeroStats().AddDamageTakenOverrideCounter();

        yield return new WaitForSeconds(_parryBonusIFrames);

        _myHeroBase.GetHeroStats().RemoveDamageTakenOverrideCounter();
    }

    #endregion

    #region Passive Abilities
    /// <summary>
    /// On hitting the boss with an attack the samurai charges his manual ability
    /// </summary>
    /// <param name="cooldownAmount"></param>
    public override void ActivatePassiveAbilities()
    {
        AddToManualAbilityChargeTime(_passiveRechargeManualAmount);
    }
    #endregion
    
}
