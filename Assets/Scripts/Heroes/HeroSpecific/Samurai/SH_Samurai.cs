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

    #region Basic Abilities
    /// <summary>
    /// Holds the conditions needed for a successful basic attack
    /// Requires the samurai to be stationary
    /// </summary>
    /// <returns></returns>
    public override bool ConditionsToActivateBasicAbilities()
    {
        return !myHeroBase.GetPathfinding().IsHeroMoving();
    }

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
        GameObject spawnedProjectile = Instantiate(_basicProjectile, myHeroBase.transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

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
        myHeroBase.GetHeroStats().AddDamageTakenOverrideCounter();
        myHeroBase.GetHeroDamagedOverrideEvent().AddListener(ParryAttack);
    }

    private void EndParry()
    {
        myHeroBase.GetHeroStats().RemoveDamageTakenOverrideCounter();
        myHeroBase.GetHeroDamagedOverrideEvent().RemoveListener(ParryAttack);
    }

    private void StopParryEarly()
    {
        StopCoroutine(_parryCoroutine);
        _parryCoroutine = null;
        myHeroBase.GetHeroStats().RemoveDamageTakenOverrideCounter();
        myHeroBase.GetHeroDamagedOverrideEvent().RemoveListener(ParryAttack);
    }

    public void ParryAttack(float damagePrevented)
    {
        DamageBoss(_manualAbilityParryDamage);
        StaggerBoss(_manualAbilityParryStagger);
        StopParryEarly();
        ActivatePassiveAbilities();

        StartSuccessfulParryIFrames();
    }

    private void StartSuccessfulParryIFrames()
    {
        StartCoroutine(SuccessfulParryIFrames());
    }

    private IEnumerator SuccessfulParryIFrames()
    {
        myHeroBase.GetHeroStats().AddDamageTakenOverrideCounter();

        yield return new WaitForSeconds(_parryBonusIFrames);

        myHeroBase.GetHeroStats().RemoveDamageTakenOverrideCounter();
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
