using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificBossFramework : MonoBehaviour
{
    [SerializeField] protected List<SpecificBossAbilityFramework> _currentBossAbilities;

    internal BossBase myBossBase;

    private List<HeroBase> _aggroOverrides = new List<HeroBase>();

    /// <summary>
    /// Determine which hero should be targeted
    /// </summary>
    /// <returns></returns>
    public virtual HeroBase DetermineAggroTarget()
    {
        Debug.Log(_aggroOverrides.Count);
        //If there is no aggro override just check the current living heroes
        if (_aggroOverrides.Count < 1)
            return DetermineAggroFromList(GameplayManagers.Instance.GetHeroesManager().GetCurrentLivingHeroes());
        //If there are aggro overrides just check them
        return DetermineAggroFromList(_aggroOverrides);
    }

    public virtual HeroBase DetermineAggroFromList(List<HeroBase> aggroTargetBases)
    {
        float totalAggroWeight = 0;
        foreach (HeroBase hb in aggroTargetBases)
        {
            totalAggroWeight += hb.GetHeroStats().GetCurrentAggro();
        }

        int randomWeightValue = (int)Random.Range(1, totalAggroWeight + 1);

        float currentWeightProgress = 0;
        foreach (HeroBase hb in aggroTargetBases)
        {
            currentWeightProgress += hb.GetHeroStats().GetCurrentAggro();

            if (randomWeightValue <= currentWeightProgress)
            {
                //Debug.Log("Random " + randomWeightValue + " current " + currentWeightProgress + " total " + totalAggroWeight);
                return hb;
            }

        }

        return null;
    }

    public virtual void HeroOverrideAggro(HeroBase heroBase, float duration)
    {
        StartCoroutine(AggroOverride(heroBase, duration));
    }

    public virtual IEnumerator AggroOverride(HeroBase heroBase, float duration)
    {
        _aggroOverrides.Add(heroBase);
        yield return new WaitForSeconds(duration);
        _aggroOverrides.Remove(heroBase);
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
