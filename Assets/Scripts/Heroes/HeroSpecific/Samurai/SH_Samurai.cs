using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private const string _parrySuccessTrigger = "ParrySuccess";

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
        //Samurai Projectile Script Not Currently in use
        //GameObject spawnedProjectile = 
        GameObject spawnedProjectile = Instantiate(_basicProjectile, _myHeroBase.transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        //spawnedProjectile.GetComponent<SHP_SamuraiBasicProjectile>().AdditionalSetup(1);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        _parryCoroutine = StartCoroutine(ParryCoroutine());
    }

    private IEnumerator ParryCoroutine()
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

        StartSuccessfulParryIFrames();
    }

    private void StartSuccessfulParryIFrames()
    {
        StartCoroutine(SuccessfulParryIFrames());
    }

    private void SuccessfulParryAnimation()
    {
        _myHeroBase.GetHeroVisuals().HeroSpecificAnimationTrigger(_parrySuccessTrigger);
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


    public override void ActivateHeroSpecificActivity()
    {
        base.ActivateHeroSpecificActivity();
    }

    public override void DeactivateHeroSpecificActivity()
    {
        base.DeactivateHeroSpecificActivity();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }

    
}
