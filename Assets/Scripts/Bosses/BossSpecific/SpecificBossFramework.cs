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

    protected BossBase _myBossBase;

    [Header("GameObjects")]
    [SerializeField] private GameObject _bossVisualsBase;
    [SerializeField] protected GameObject _bossSpecificUI;

    protected GameObject _storedBossUI;

    private List<HeroBase> _bossAttackTargets = new List<HeroBase>();
    private List<HeroBase> _aggroOverrides = new List<HeroBase>();

    protected Coroutine _nextAttackProcess;
    protected Coroutine _preventAttacksCoroutine;

    [Header("Animator")]
    [SerializeField] private Animator _bossSpecificAnimator;

    #region Fight Start
    protected virtual void StartFight()
    {
        SetupReadyBossAbilities();

        AssignInitialHeroTargets();

        StartNextAbility();
    }

    /// <summary>
    /// Adds the starting abilities into the list of ready attacks
    /// </summary>
    protected virtual void SetupReadyBossAbilities()
    {
        foreach(SpecificBossAbilityFramework sbaf in _startingBossAbilities)
        {
            AddAbilityToBossReadyAttacks(sbaf);
        }
    }

    /// <summary>
    /// Creates an ui elements that are unique to the specific boss
    /// </summary>
    protected virtual void CreateBossSpecificUI()
    {
        if (_bossSpecificUI == null) return;
        _storedBossUI = GameplayManagers.Instance.GetGameUIManager().
            GetBossUIManager().AddBossUIToHolder(_bossSpecificUI);

        _storedBossUI.GetComponent<SpecificBossUIFramework>().SetupBossSpecificUIFunctionality(_myBossBase, this);
    }

    #endregion

    #region Aggro
    /// <summary>
    /// Alls all heroes to the list of heroes that can be targeted
    /// </summary>
    protected virtual void AssignInitialHeroTargets()
    {
        _bossAttackTargets = new List<HeroBase>(HeroesManager.Instance.GetCurrentHeroes());

        _myBossBase.InvokeBossTargetsAssignedEvent();
    }

    /// <summary>
    /// Adds a specific hero to the list of heroes that can be targeted
    /// </summary>
    /// <param name="heroBase"></param>
    public void AddHeroTarget(HeroBase heroBase)
    {
        _bossAttackTargets.Add(heroBase);
    }

    /// <summary>
    /// Removes a specific hero from the list of heroes that can be targeted
    /// </summary>
    /// <param name="heroBase"></param>
    protected virtual void RemoveHeroTarget(HeroBase heroBase)
    {
        if (!_bossAttackTargets.Contains(heroBase)) return;

        _bossAttackTargets.Remove(heroBase);
    }

    /// <summary>
    /// Determine which hero should be targeted
    /// </summary>
    /// <returns></returns>
    public virtual HeroBase DetermineAggroTarget()
    {
        /*//If there is no aggro override just check the current living heroes
        if (_aggroOverrides.Count < 1)
            return DetermineAggroFromHeroes(_bossAttackTargets);
        //If there are aggro overrides just check them
        return DetermineAggroFromHeroes(_aggroOverrides);*/
        return DetermineAggroTargetFromLists(_bossAttackTargets, _aggroOverrides);
    }

    public virtual HeroBase DetermineAggroTargetWithoutHero(HeroBase removeHero)
    {
        List<HeroBase> newBossAttackTargets = new List<HeroBase>(_bossAttackTargets);
        List<HeroBase> newAggroOverrides = new List<HeroBase>(_aggroOverrides);

        newBossAttackTargets.Remove(removeHero);
        newAggroOverrides?.Remove(removeHero);
        
        return DetermineAggroTargetFromLists(newBossAttackTargets, newAggroOverrides);
    }


    public virtual HeroBase DetermineAggroTargetFromLists(List<HeroBase> attackTargets, List<HeroBase> overrideTargets)
    {
        //If there is no aggro override just check the current living heroes
        if (overrideTargets.Count < 1)
            return DetermineAggroFromHeroes(attackTargets);
        //If there are aggro overrides just check them
        return DetermineAggroFromHeroes(overrideTargets);
    }

    /// <summary>
    /// Determines what hero to target based on their aggro
    /// </summary>
    /// <param name="aggroTargetBases"> The viable targets for attacking </param>
    /// <returns></returns>
    public virtual HeroBase DetermineAggroFromHeroes(List<HeroBase> aggroTargetBases)
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
                return hb;
            }
        }

        return null;
    }

    /// <summary>
    /// Starts the process of having a hero be the sole target for boss abilities
    /// </summary>
    /// <param name="heroBase"></param>
    /// <param name="duration"></param>
    public virtual void StartHeroOverrideAggro(HeroBase heroBase, float duration)
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
    /// <summary>
    /// Adds an ability to the list of attacks that the boss can randomly pick to use
    /// </summary>
    /// <param name="newAbility"></param>
    private void AddAbilityToBossReadyAttacks(SpecificBossAbilityFramework newAbility)
    {
        _readyBossAttacks.Add(newAbility);
    }

    /// <summary>
    /// Removes the next ability from the cooldown queue and adds it back into the list of
    /// available abilities that the boss can use
    /// </summary>
    private void TakeAbilityFromQueueToReady()
    {
        AddAbilityToBossReadyAttacks(_bossCooldownQueue.Dequeue());
    }

    /// <summary>
    /// Removes the ability that was just used and puts it at the end of the cooldown queue
    /// </summary>
    /// <param name="newAbility"></param>
    private void AddAbilityToEndOfCooldownQueue(SpecificBossAbilityFramework newAbility)
    {
        _readyBossAttacks?.Remove(newAbility);
        
        _bossCooldownQueue.Enqueue(newAbility);
    }

    /// <summary>
    /// For the first few abilities used iterate a counter.
    /// Counter prevents the ability from immediately moving right back into the ready list
    /// When the counter is at max call RepitionCounterAtMax
    /// </summary>
    protected void IterateRepitionCounter()
    {
        _attackRepitionCounter++;
        if (_attackRepitionCounter >= _attackRepititionProtection)
        {
            RepitionCounterAtMax();
        }
    }

    /// <summary>
    /// Removes the iteration repition counter
    /// Replaces it with TakeAbilityFromQueueToReady
    /// </summary>
    protected void RepitionCounterAtMax()
    {
        _myBossBase.GetBossAbilityUsedEvent().RemoveListener(IterateRepitionCounter);
        _myBossBase.GetBossAbilityUsedEvent().AddListener(TakeAbilityFromQueueToReady);
    }


    /// <summary>
    /// Activates the next ability for the boss to use
    /// </summary>
    protected virtual void StartNextAbility()
    {
        if (GameStateManager.Instance.GetIsFightOver())
        {
            return;
        }

        SpecificBossAbilityFramework nextAbility = SelectNextAbility();
        AddAbilityToEndOfCooldownQueue(nextAbility);

        _nextAttackProcess = StartCoroutine(UseNextAttackProcess(nextAbility));
    }

    /// <summary>
    /// Randomly selected an ability from the list of readied abilities
    /// </summary>
    /// <returns> The next ability selected</returns>
    protected SpecificBossAbilityFramework SelectNextAbility()
    {
        int randomAbility = Random.Range(0, _readyBossAttacks.Count);

        return _readyBossAttacks[randomAbility];
    }

    protected virtual IEnumerator UseNextAttackProcess(SpecificBossAbilityFramework currentAbility)
    {
        HeroBase newTarget;
        //Determines where the boss is targetting based on the abilities target method
        Vector3 targetLocation = DetermineBossTargetLocation(currentAbility, out newTarget);

        //Causes the boss to turn to look at the current location of their target
        _myBossBase.GetBossVisuals().BossLookAt(targetLocation);

        //Uses the current ability
        currentAbility.ActivateAbility(targetLocation, newTarget);
        
        _myBossBase.InvokeBossAbilityUsedEvent();

        //Waits for a specified amount of time determined by the current ability
        yield return new WaitForSeconds(currentAbility.GetTimeUntilNextAbility());

        _nextAttackProcess = null;

        //Uses the next ability to repeat the cycle
        StartNextAbility();
    }

    protected virtual Vector3 DetermineBossTargetLocation(SpecificBossAbilityFramework currentAbility, out HeroBase targetHero)
    {
        targetHero = null;

        switch(currentAbility.GetTargetMethod())
        {
            case (EBossAbilityTargetMethod.HeroTarget):
                targetHero = DetermineAggroTarget();
                return ClosestFloorSpaceOfTarget(targetHero.gameObject);
            case (EBossAbilityTargetMethod.HeroTargetWithIgnore):

            case (EBossAbilityTargetMethod.SpecificHeroTarget):

            case (EBossAbilityTargetMethod.SpecificAreaTarget):
                return currentAbility.GetSpecificLookTarget();
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Stops the boss from attacking
    /// </summary>
    protected virtual void StopNextAttackProcess()
    {
        StopCoroutine(_nextAttackProcess);
        _nextAttackProcess = null;
    }

    /// <summary>
    /// Stuns the boss and prevents them from attacking
    /// </summary>
    /// <param name="stopDuration"></param>
    /// <returns></returns>
    protected virtual IEnumerator StaggerBossForDuration(float stopDuration)
    {
        //Prevents the next attack from being used
        StopNextAttackProcess();
        //Waits for the boss stagger duration
        yield return new WaitForSeconds(stopDuration);
        //Starts up the process of using abilities again
        StartNextAbility();

        _myBossBase.InvokeBossNoLongerStaggeredEvent();
    }

    #endregion

    protected virtual void BossStaggerOccured()
    {
        _preventAttacksCoroutine = StartCoroutine(StaggerBossForDuration
            (_myBossBase.GetBossStats().GetStaggerDuration()));
    }

    protected virtual void BossNoLongerStaggeredOccured()
    {

    }

    /// <summary>
    /// If the boss has an abilities locked it is unlocked under half health
    /// </summary>
    protected virtual void UnlockNewAbility()
    {
        //Checks to see if there actually is an ability to unlock
        if (_abilityLocked == null)
        {
            return;
        }
        
        AddAbilityToBossReadyAttacks(_abilityLocked);
    }

    /// <summary>
    /// Removes the dead hero from the list of targets
    /// </summary>
    /// <param name="heroBase"></param>
    public virtual void HeroDied(HeroBase heroBase)
    {
        RemoveHeroTarget(heroBase);
    }

    public virtual void SetupSpecificBoss(BossBase bossBase)
    {
        _myBossBase = bossBase;
        CreateBossSpecificUI();
        SubscribeToEvents();
    }

    #region Events
    public virtual void SubscribeToEvents()
    {
        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(StartFight);

        //Listens for when the boss uses an ability
        _myBossBase.GetBossAbilityUsedEvent().AddListener(IterateRepitionCounter);

        //Listens for when the boss is staggered
        _myBossBase.GetBossStaggeredEvent().AddListener(BossStaggerOccured);
        //Listens for when the boss stagger ends
        _myBossBase.GetBossNoLongerStaggeredEvent().AddListener(BossNoLongerStaggeredOccured);
        
        _myBossBase.GetBossHalfHealthEvent().AddListener(UnlockNewAbility);
    }

    #endregion

    #region Getters
    public Vector3 ClosestFloorSpaceOfTarget(GameObject target) =>
        GameplayManagers.Instance.GetEnvironmentManager().
        GetClosestPointToFloor(target.transform.position);

    public GameObject GetBossVisualBase() => _bossVisualsBase;

    public Animator GetBossSpecificAnimator() => _bossSpecificAnimator;
    #endregion
}
