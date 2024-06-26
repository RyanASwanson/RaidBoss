using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificBossFramework : MonoBehaviour
{
    [Header("Attacks")]
    [SerializeField] protected List<SpecificBossAbilityFramework> _startingBossAbilities;
    [SerializeField] protected int _attackRepititionProtection;
    protected int _attackRepitionCounter = 0;

    [Space]
    [SerializeField] protected SpecificBossAbilityFramework _abilityLocked;

    protected List<SpecificBossAbilityFramework> _readyBossAttacks = new List<SpecificBossAbilityFramework>();
    protected Queue<SpecificBossAbilityFramework> _bossCooldownQueue = new Queue<SpecificBossAbilityFramework>();

    internal BossBase myBossBase;

    [Header("GameObjects")]
    [SerializeField] private GameObject _bossVisualsBase;

    private List<HeroBase> _aggroOverrides = new List<HeroBase>();

    private Coroutine _nextAttackProcess;

    private Coroutine _preventAttacksCoroutine;

    #region Fight Start
    private void StartFight()
    {
        SetupReadyBossAbilities();

        StartNextAbility();
    }

    /// <summary>
    /// Adds the starting abilities into the list of ready attacks
    /// </summary>
    private void SetupReadyBossAbilities()
    {
        foreach(SpecificBossAbilityFramework sbaf in _startingBossAbilities)
        {
            AddAbilityToBossReadyAttacks(sbaf);
        }
    }

    

    #endregion

    #region Aggro
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

    /// <summary>
    /// Lets a hero override the bosses aggro for a short period of time
    /// </summary>
    /// <param name="heroBase"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public virtual IEnumerator AggroOverride(HeroBase heroBase, float duration)
    {
        _aggroOverrides.Add(heroBase);
        yield return new WaitForSeconds(duration);
        _aggroOverrides.Remove(heroBase);
    }
    #endregion

    #region Ability Functionality

    private void AddAbilityToBossReadyAttacks(SpecificBossAbilityFramework newAbility)
    {
        _readyBossAttacks.Add(newAbility);
    }

    /// <summary>
    /// Removes the next ability from the queue and adds it back into the list of
    /// available abilities that the boss can use
    /// </summary>
    private void TakeAbilityFromQueueToReady()
    {
        AddAbilityToBossReadyAttacks(_bossCooldownQueue.Dequeue());
    }

    private void AddAbilityToEndOfCooldownQueue(SpecificBossAbilityFramework newAbility)
    {
        _readyBossAttacks?.Remove(newAbility);


        _bossCooldownQueue.Enqueue(newAbility);
    }

    /// <summary>
    /// For the first few abilities used iterate a counter
    /// When the counter is at max call RepitionCounterAtMax
    /// </summary>
    protected void IterateRepitionCounter()
    {
        _attackRepitionCounter++;
        if (_attackRepitionCounter >= _attackRepititionProtection)
            RepitionCounterAtMax();
    }

    /// <summary>
    /// Removes the iteration repition counter
    /// Replaces it with TakeAbilityFromQueueToReady
    /// </summary>
    protected void RepitionCounterAtMax()
    {
        myBossBase.GetBossAbilityUsedEvent().RemoveListener(IterateRepitionCounter);
        myBossBase.GetBossAbilityUsedEvent().AddListener(TakeAbilityFromQueueToReady);
    }


    /// <summary>
    /// Activates the next ability for the boss to use
    /// </summary>
    /// <param name="nextAbility"></param>
    protected virtual void StartNextAbility()
    {
        if (GameplayManagers.Instance.GetGameStateManager().GetIsFightOver()) return;

        SpecificBossAbilityFramework nextAbility = SelectNextAbility();
        AddAbilityToEndOfCooldownQueue(nextAbility);

        _nextAttackProcess = StartCoroutine(UseNextAttackProcess(nextAbility));
    }

    /// <summary>
    /// Randomly selected an ability from the list of readied abilities
    /// </summary>
    /// <returns></returns>
    protected SpecificBossAbilityFramework SelectNextAbility()
    {
        int randomAbility = Random.Range(0, _readyBossAttacks.Count);

        return _readyBossAttacks[randomAbility];
    }

    protected virtual IEnumerator UseNextAttackProcess(SpecificBossAbilityFramework currentAbility)
    {
        HeroBase newTarget = DetermineAggroTarget();

        myBossBase.GetBossVisuals().BossLookAt(newTarget.transform.position);

        currentAbility.ActivateAbility(
            ClosestFloorSpaceOfHeroTarget(newTarget), newTarget);

        myBossBase.InvokeBossAbilityUsedEvent();

        yield return new WaitForSeconds(currentAbility.GetTimeUntilNextAbility());

        _nextAttackProcess = null;

        StartNextAbility();
    }

    /// <summary>
    /// Stops the boss from attacking
    /// </summary>
    protected virtual void StopNextAttackProcess()
    {
        StopCoroutine(_nextAttackProcess);
        _nextAttackProcess = null;
    }

    protected virtual IEnumerator StaggerBossForDuration(float stopDuration)
    {
        StopNextAttackProcess();
        yield return new WaitForSeconds(stopDuration);
        StartNextAbility();

        myBossBase.InvokeBossNoLongerStaggeredEvent();
    }


    #endregion

    protected virtual void BossStaggerOccured()
    {
        _preventAttacksCoroutine = StartCoroutine(StaggerBossForDuration
            (myBossBase.GetBossStats().GetStaggerDuration()));
    }

    protected virtual void UnlockNewAbility()
    {
        AddAbilityToBossReadyAttacks(_abilityLocked);
    }

    public virtual void SetupSpecificBoss(BossBase bossBase)
    {
        myBossBase = bossBase;
        SubscribeToEvents();
    }

    #region Events
    public virtual void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetGameStateManager().GetStartOfBattleEvent().AddListener(StartFight);

        //Listens for when the boss uses an ability
        myBossBase.GetBossAbilityUsedEvent().AddListener(IterateRepitionCounter);

        //Listens for when the boss is staggered
        myBossBase.GetBossStaggeredEvent().AddListener(BossStaggerOccured);

        myBossBase.GetBossHalfHealthEvent().AddListener(UnlockNewAbility);
    }

    #endregion

    #region Getters
    public Vector3 ClosestFloorSpaceOfHeroTarget(HeroBase heroBase) =>
        GameplayManagers.Instance.GetEnvironmentManager().
        GetClosestPointToFloor(heroBase.gameObject.transform.position);

    public GameObject GetBossVisualBase() => _bossVisualsBase;
    #endregion
}
