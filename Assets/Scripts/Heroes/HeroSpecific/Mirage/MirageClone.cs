using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageClone : SpecificHeroFramework
{
    [Space]
    private bool _canCastBasic;

    private SH_Mirage _mirageOwner;
    private BossBase _bossBase;

    /// <summary>
    /// Starts the process of the clone casting it's own copy of the Mirage
    ///     basic ability
    /// </summary>
    private void StartCloneCastBasic()
    {
        StartCoroutine(BasicCastProcess());
    }

    /// <summary>
    /// Continues casting the Mirage basic ability until it stops moving
    /// </summary>
    /// <returns></returns>
    private IEnumerator BasicCastProcess()
    {
        _myHeroBase.GetHeroStoppedMovingEvent().AddListener(EndBasicCastProcess);
        _canCastBasic = true;
        float castCounter = 0;

        while (_canCastBasic)
        {
            castCounter += Time.deltaTime;

            if (castCounter >= _basicAbilityChargeTime)
            {
                castCounter -= _basicAbilityChargeTime;

                _mirageOwner.CloneBasicAbility();
            }

            yield return null;
        }

        _canCastBasic = false;
    }

    /// <summary>
    /// Disables the ability to cast the Mirage basic ability
    /// Called when it stops moving
    /// </summary>
    private void EndBasicCastProcess()
    {
        _canCastBasic = false;
        _myHeroBase.GetHeroStoppedMovingEvent().RemoveListener(EndBasicCastProcess);
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

    protected override void SubscribeToEvents()
    {
        _myHeroBase.GetHeroStartedMovingEvent().AddListener(StartCloneCastBasic);
        _bossBase.GetBossTargetsAssignedEvent().AddListener(AssignSelfAsBossTarget);
        base.SubscribeToEvents();
    }
    #endregion
}
