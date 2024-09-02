using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageClone : SpecificHeroFramework
{
    [Space]
    private bool _canCastBasic;

    private SH_Mirage _mirageOwner;
    private BossBase _bossBase;

    private void StartCloneCastBasic()
    {
        StartCoroutine(BasicCastProcess());
    }

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

    private void EndBasicCastProcess()
    {
        _canCastBasic = false;
        _myHeroBase.GetHeroStoppedMovingEvent().RemoveListener(EndBasicCastProcess);
    }

    private void AssignSelfAsBossTarget()
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetSpecificBossScript().AddHeroTarget(_myHeroBase);
    }

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

    #region Base Hero

    #endregion
}
