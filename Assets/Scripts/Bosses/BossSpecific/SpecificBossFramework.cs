using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality specific to each boss that can be overriden
/// </summary>
public abstract class SpecificBossFramework : MonoBehaviour
{
    [Header("Attacks")]
    [SerializeField] protected List<SpecificBossAbilityFramework> _startingBossAbilities;
    [SerializeField] protected int _attackRepititionProtection;
    protected int _attackRepetitionCounter = 0;

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
    /// <summary>
    /// Called when the fight begins to start boss functionality
    /// </summary>
    protected virtual void StartFight()
    {
        SetUpReadyBossAbilities();

        AssignInitialHeroTargets();

        StartCoroutine(InitialAttackDelay());
    }

    /// <summary>
    /// Adds a delay to the initial actions of the boss
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitialAttackDelay()
    {
        yield return new WaitForSeconds(_myBossBase.GetBossSO().GetFightStartDelay());
        StartNextAbility();
    }

    /// <summary>
    /// Adds the starting abilities into the list of ready attacks
    /// </summary>
    protected virtual void SetUpReadyBossAbilities()
    {
        // Iterates through each ability
        foreach(SpecificBossAbilityFramework ability in _startingBossAbilities)
        {
            AddAbilityToBossReadyAttacks(ability);
        }
    }

    /// <summary>
    /// Creates an ui elements that are unique to the specific boss
    /// </summary>
    protected virtual void CreateBossSpecificUI()
    {
        // Stops if there is no boss specific UI
        if (_bossSpecificUI.IsUnityNull())
        {
            return;
        }

        if (_bossSpecificUI == null)
        {
            Debug.Log("NOTFOUND");
        }

        if (BossUIManager.Instance == null)
        {
            Debug.Log("NOTFOUND2");
        }
        _storedBossUI = BossUIManager.Instance.AddBossUIToHolder(_bossSpecificUI);

        _storedBossUI.GetComponent<SpecificBossUIFramework>().SetUpBossSpecificUIFunctionality(_myBossBase, this);
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
        if (!_bossAttackTargets.Contains(heroBase))
        {
            return;
        }

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
        {
            return DetermineAggroFromHeroes(attackTargets);
        }
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
    protected virtual IEnumerator AggroOverride(HeroBase heroBase, float duration)
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
    protected virtual void AddAbilityToBossReadyAttacks(SpecificBossAbilityFramework newAbility)
    {
        _readyBossAttacks.Add(newAbility);
    }

    /// <summary>
    /// Removes the next ability from the cooldown queue and adds it back into the list of
    /// available abilities that the boss can use
    /// </summary>
    protected virtual void TakeAbilityFromQueueToReady()
    {
        AddAbilityToBossReadyAttacks(_bossCooldownQueue.Dequeue());
    }

    /// <summary>
    /// Removes the ability that was just used and puts it at the end of the cooldown queue
    /// </summary>
    /// <param name="newAbility"> The ability we are removing from ready and into cooldown </param>
    protected virtual void AddAbilityToEndOfCooldownQueue(SpecificBossAbilityFramework newAbility)
    {
        _readyBossAttacks?.Remove(newAbility);
        
        _bossCooldownQueue.Enqueue(newAbility);
    }

    /// <summary>
    /// For the first few abilities used iterate a counter.
    /// Counter prevents the ability from immediately moving right back into the ready list
    /// When the counter is at max call RepetitionCounterAtMax
    /// </summary>
    protected virtual void IterateRepetitionCounter()
    {
        // Increment the repetition counter
        _attackRepetitionCounter++;
        if (_attackRepetitionCounter >= _attackRepititionProtection)
        {
            RepetitionCounterAtMax();
        }
    }

    /// <summary>
    /// Removes the iteration repetition counter
    /// Replaces it with TakeAbilityFromQueueToReady
    /// </summary>
    protected virtual void RepetitionCounterAtMax()
    {
        _myBossBase.GetBossAbilityUsedEvent().RemoveListener(IterateRepetitionCounter);
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

        _nextAttackProcess = StartCoroutine(UseNextAbilityProcess(nextAbility));
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

    /// <summary>
    /// The process by which the boss uses the next ability
    /// </summary>
    /// <param name="currentAbility"> The ability to use </param>
    /// <returns></returns>
    protected virtual IEnumerator UseNextAbilityProcess(SpecificBossAbilityFramework currentAbility)
    {
        // Determines where the boss is targeting based on the abilities target method
        Vector3 targetLocation = DetermineBossTargetLocation(currentAbility, out HeroBase newTarget);

        if (currentAbility.GetDoesBossFollowTarget())
        {
            BossVisuals.Instance.BossLookAt(newTarget.gameObject, currentAbility.GetAbilityWindUpTime());
        }
        else
        {
            // Causes the boss to turn to look at the current location of their target
            BossVisuals.Instance.BossLookAt(targetLocation);
        }

        // Uses the current ability
        currentAbility.ActivateAbility(targetLocation, newTarget);
        
        _myBossBase.InvokeBossAbilityUsedEvent();

        // Waits for a specified amount of time determined by the current ability
        yield return new WaitForSeconds(currentAbility.GetTimeUntilNextAbility());

        _nextAttackProcess = null;

        // Uses the next ability to repeat the cycle
        StartNextAbility();
    }

    /// <summary>
    /// Determines where or who the boss will attack based on the current ability
    /// </summary>
    /// <param name="currentAbility"> The ability to determine the location for </param>
    /// <param name="targetHero"> The out variable for a hero if there is one </param>
    /// <returns></returns>
    protected virtual Vector3 DetermineBossTargetLocation(SpecificBossAbilityFramework currentAbility, out HeroBase targetHero)
    {
        targetHero = null;

        // Uses a different way to determine the target location based on the ability target method
        switch(currentAbility.GetTargetMethod())
        {
            // If the ability targets heroes
            case (EBossAbilityTargetMethod.HeroTarget):
                targetHero = DetermineAggroTarget();
                return ClosestFloorSpaceOfTarget(targetHero.gameObject);
            
            // If the ability targets heroes with a specific ignore
            // Currently has no functionality
            case (EBossAbilityTargetMethod.HeroTargetWithIgnore):
                
            // If the ability targets a specific hero
            // Currently has no functionality
            case (EBossAbilityTargetMethod.SpecificHeroTarget):
                
            // If the ability targets a specific location
            case (EBossAbilityTargetMethod.SpecificAreaTarget):
                return currentAbility.GetSpecificLookTarget();
        }

        // In case the ability were to not fall into anything above
        Debug.LogError("Boss was unable to determine target location or hero");
        return Vector3.zero;
    }

    /// <summary>
    /// Stops the boss from attacking
    /// </summary>
    protected virtual void StopNextAttackProcess()
    {
        if (_nextAttackProcess.IsUnityNull())
        {
            Debug.LogError("Boss was unable to stop next attack process");
            return;
        }
        
        StopCoroutine(_nextAttackProcess);
        _nextAttackProcess = null;
    }

    /// <summary>
    /// Stuns the boss and prevents them from attacking
    /// </summary>
    /// <param name="stopDuration"> The duration of the stagger </param>
    /// <returns></returns>
    protected virtual IEnumerator StaggerBossForDuration(float stopDuration)
    {
        float timer = 0;
        
        // Prevents the next attack from being used
        StopNextAttackProcess();

        // Waits for the boss stagger duration
        while (timer < stopDuration)
        {
            timer += Time.deltaTime;
            BossBase.Instance.InvokeBossStaggerProcess(timer/stopDuration);
            yield return null;
        }
        
        // Starts up the process of using abilities again
        StartNextAbility();

        // Invokes the event for the boss no longer staggered
        _myBossBase.InvokeBossNoLongerStaggeredEvent();
    }

    #endregion

    /// <summary>
    /// Called when the boss is staggered.
    /// Starts the boss stagger duration.
    /// Virtual to allow for it to be overridden by specific bosses
    /// </summary>
    protected virtual void BossStaggerOccured()
    {
        // Starts the coroutine for the boss stagger
        _preventAttacksCoroutine = StartCoroutine(StaggerBossForDuration
            (BossStats.Instance.GetStaggerDuration()));
    }

    /// <summary>
    /// Called when the boss is no longer staggered
    /// Virtual to allow for it to be overridden by specific bosses
    /// </summary>
    protected virtual void BossNoLongerStaggeredOccured()
    {

    }

    /// <summary>
    /// If the boss has an abilities locked it is unlocked under half health
    /// </summary>
    protected virtual void UnlockNewAbility()
    {
        // Checks to see if there actually is an ability to unlock
        if (_abilityLocked.IsUnityNull())
        {
            return;
        }
        
        AddAbilityToBossReadyAttacks(_abilityLocked);
    }

    /// <summary>
    /// Removes the dead hero from the list of targets
    /// </summary>
    /// <param name="heroBase"> The base of the hero that died </param>
    public virtual void HeroDied(HeroBase heroBase)
    {
        RemoveHeroTarget(heroBase);
    }

    /// <summary>
    /// Performs any set up that is unique to the boss
    /// </summary>
    /// <param name="bossBase"> The base of the boss </param>
    public virtual void SetUpSpecificBoss(BossBase bossBase)
    {
        _myBossBase = bossBase;
        CreateBossSpecificUI();
        SubscribeToEvents();
    }

    #region Events
    /// <summary>
    /// Subscribes to any needed events for the specific boss
    /// </summary>
    public virtual void SubscribeToEvents()
    {
        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(StartFight);

        //Listens for when the boss uses an ability
        _myBossBase.GetBossAbilityUsedEvent().AddListener(IterateRepetitionCounter);

        //Listens for when the boss is staggered
        _myBossBase.GetBossStaggeredEvent().AddListener(BossStaggerOccured);
        //Listens for when the boss stagger ends
        _myBossBase.GetBossNoLongerStaggeredEvent().AddListener(BossNoLongerStaggeredOccured);
        
        _myBossBase.GetBossHalfHealthEvent().AddListener(UnlockNewAbility);
    }
    #endregion

    #region Getters
    public Vector3 ClosestFloorSpaceOfTarget(GameObject target) =>
        EnvironmentManager.Instance.GetClosestPointToFloor(target.transform.position);

    public GameObject GetBossVisualBase() => _bossVisualsBase;

    public Animator GetBossSpecificAnimator() => _bossSpecificAnimator;
    #endregion
}
