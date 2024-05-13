using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificBossFramework : MonoBehaviour
{
    [SerializeField] protected List<SpecificBossAbilityFramework> _currentBossAbilities;

    internal BossBase myBossBase;

    private List<HeroBase> _aggroOverrides = new List<HeroBase>();

    private Coroutine _nextAttackProcess;

    /// <summary>
    /// Determine which hero should be targeted
    /// </summary>
    /// <returns></returns>
    public virtual HeroBase DetermineAggroTarget()
    {
        //If there is no aggro override just check the current living heroes
        if (_aggroOverrides.Count < 1)
            return DetermineAggroFromList(GameplayManagers.Instance.GetHeroesManager().GetCurrentLivingHeroes());
        //If there are aggro overrides just check them
        return DetermineAggroFromList(_aggroOverrides);
    }

    /// <summary>
    /// Checks
    /// </summary>
    /// <param name="aggroTargetBases"></param>
    /// <returns></returns>
    public virtual HeroBase DetermineAggroFromList(List<HeroBase> aggroTargetBases)
    {
        //Adds the aggro of all living heroes to a total value
        float totalAggroWeight = 0;
        foreach (HeroBase hb in aggroTargetBases)
        {
            totalAggroWeight += hb.GetHeroStats().GetCurrentAggro();
        }

        //Creates a random value somewhere between 1 and the aggro total plus 1
        int randomWeightValue = (int)Random.Range(1, totalAggroWeight + 1);

        //Goes through all living heroes to find where the random value resides
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

    /// <summary>
    /// Activates the next ability for the boss to use
    /// </summary>
    /// <param name="nextAbility"></param>
    protected virtual void StartNextAbility(SpecificBossAbilityFramework nextAbility)
    {
        _nextAttackProcess = StartCoroutine(UseNextAttackProcess(nextAbility));
    }

    protected virtual IEnumerator UseNextAttackProcess(SpecificBossAbilityFramework currentAbility)
    {
        yield return new WaitForSeconds(currentAbility.GetTimeUntilNextAbility());

        _nextAttackProcess = null;
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

    #region Getters
    public Vector3 ClosestFloorSpaceOfHeroTarget(HeroBase heroBase) =>
        GameplayManagers.Instance.GetEnvironmentManager().
        GetClosestPointToFloor(heroBase.gameObject.transform.position);
    #endregion
}
