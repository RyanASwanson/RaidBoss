using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificBossFramework : MonoBehaviour
{
    internal BossBase myBossBase;


    public virtual HeroBase DetermineAggroTarget()
    {
        float totalAggroWeight = 0;
        foreach(HeroBase hb in GameplayManagers.Instance.GetHeroesManager().GetCurrentLivingHeroes())
        {
            totalAggroWeight += hb.GetHeroStats().GetCurrentAggro();
        }

        float randomWeightValue = Random.Range(0, totalAggroWeight);

        float currentWeightProgress = 0;
        foreach (HeroBase hb in GameplayManagers.Instance.GetHeroesManager().GetCurrentLivingHeroes())
        {
            currentWeightProgress += hb.GetHeroStats().GetCurrentAggro();

            if (randomWeightValue <= currentWeightProgress)
                return hb;
        }

        return null;
    }

    
    public virtual void SetupSpecificBoss(BossBase bossBase)
    {
        myBossBase = bossBase;
        SubscribeToEvents();
    }

    #region Events
    public virtual void SubscribeToEvents()
    {
        
    }

    #endregion
}
