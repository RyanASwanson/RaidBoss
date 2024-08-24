using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageClone : SpecificHeroFramework
{
    private BossBase _bossBase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AssignSelfAsBossTarget()
    {
        print("Target Assigned");
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetSpecificBossScript().AddHeroTarget(_myHeroBase);
    }

    public override void SetupSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        _bossBase = GameplayManagers.Instance.GetBossManager().GetBossBase();
        base.SetupSpecificHero(heroBase, heroSO);
    }

    protected override void SubscribeToEvents()
    {
        _bossBase.GetBossTargetsAssignedEvent().AddListener(AssignSelfAsBossTarget);
        base.SubscribeToEvents();
    }
}
