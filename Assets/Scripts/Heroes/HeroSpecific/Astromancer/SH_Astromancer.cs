using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Astromancer : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private float _increasedManualRotationalSpeed;
    [SerializeField] private GameObject _manualProjectile;

    private bool _manualActive = false;

    [Space]
    [SerializeField] private float _passiveRechargeManualAmount;

    SHP_AstromancerManualProjectile _storedManual;

    #region Basic Abilities
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    protected void CreateBasicAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_basicProjectile, transform.position, Quaternion.identity);

        SHP_AstromancerBasicProjectile projectileFunc = spawnedProjectile.GetComponent<SHP_AstromancerBasicProjectile>();
        projectileFunc.SetUpProjectile(_myHeroBase);

        Vector3 storedProjectileDirection = GameplayManagers.Instance.GetBossManager().GetDirectionToBoss(transform.position);
        projectileFunc.AdditionalSetup(this, storedProjectileDirection);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        //Performs the setup for the 'healing' area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroHealArea>().SetUpHealingArea(_myHeroBase);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        _manualActive = true;

        //Does everything in the base of this function except for starting the cooldown
        _manualAbilityCurrentCharge = 0;

        _manualAbilityCooldownCoroutine = null;

        TriggerManualAbilityAnimation();

        _myHeroBase.GetHeroStats().ChangeCurrentHeroAngularSpeed(_increasedManualRotationalSpeed);


        CreateManualAttackProjectiles();
    }

    protected void CreateManualAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_manualProjectile, transform.position, Quaternion.identity);

        _storedManual = spawnedProjectile.GetComponent<SHP_AstromancerManualProjectile>();
        _storedManual.SetUpProjectile(_myHeroBase);

        _myHeroBase.GetPathfinding().BriefStopCurrentMovement();
        _myHeroBase.GetHeroStartedMovingEvent().AddListener(EndManualAbility);
    }

    protected void EndManualAbility()
    {
        _manualActive = false;

        _myHeroBase.GetHeroStartedMovingEvent().RemoveListener(EndManualAbility);

        _storedManual.StopLaser();

        _myHeroBase.GetHeroStats().ChangeCurrentHeroAngularSpeed(-_increasedManualRotationalSpeed);

        StartCooldownManualAbility();
    }

    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        if (!_manualActive)
            AddToManualAbilityChargeTime(_passiveRechargeManualAmount);
    }

    #endregion

}

