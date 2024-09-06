using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageClone : SpecificHeroFramework
{
    private SH_Mirage _mirageOwner;
    private BossBase _bossBase;

    /// <summary>
    /// Starts the process of the clone casting it's own copy of the Mirage
    ///     basic ability
    /// </summary>
    protected override void StartCooldownBasicAbility()
    {
        base.StartCooldownBasicAbility();
    }

    /// <summary>
    /// Only allows for the clone to use its basic ability while moving
    /// </summary>
    /// <returns></returns>
    public override bool ConditionsToActivateBasicAbilities()
    {
        return !base.ConditionsToActivateBasicAbilities();
    }


    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        _mirageOwner.CloneBasicAbility();
    }

    /// <summary>
    /// Disables the ability to cast the Mirage basic ability
    /// Called when it stops moving
    /// </summary>
    private void EndBasicCastProcess()
    {
        StopCooldownBasicAbility();
    }


    /// <summary>
    /// The mirage clone adds itself as a possible target for boss attacks
    /// Heroes are already set as targets, but as this is an ability it has to do so manually
    /// </summary>
    private void AssignSelfAsBossTarget()
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetSpecificBossScript().AddHeroTarget(_myHeroBase);
    }



    #region Base Hero
    public override void SetupSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        _bossBase = GameplayManagers.Instance.GetBossManager().GetBossBase();
        base.SetupSpecificHero(heroBase, heroSO);
    }

    public void AdditionalSetup(SH_Mirage mirage)
    {
        _mirageOwner = mirage;

        _myHeroBase.SetClickColliderStatus(false);
        HeroStats heroStats = _myHeroBase.GetHeroStats();

        heroStats.AddDamageTakenOverrideCounter();
        heroStats.AddHealingTakenOverrideCounter();
    }

    public override void ActivateHeroSpecificActivity()
    {
        //Overrides the base to do nothing
    }

    protected override void SubscribeToEvents()
    {
        //_myHeroBase.GetHeroStartedMovingEvent().AddListener(StartCloneCastBasic);
        _myHeroBase.GetHeroStartedMovingEvent().AddListener(StartCooldownBasicAbility);

        //Stops the clone from using the basic ability after it stops moving
        _myHeroBase.GetHeroStoppedMovingEvent().AddListener(EndBasicCastProcess);

        _bossBase.GetBossTargetsAssignedEvent().AddListener(AssignSelfAsBossTarget);
        base.SubscribeToEvents();
    }
    #endregion
}
