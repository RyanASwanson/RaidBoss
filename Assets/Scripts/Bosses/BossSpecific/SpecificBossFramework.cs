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
    [SerializeField] protected SpecificBossAbilityFramework[] _startingBossAbilities;
    [SerializeField] protected int _attackRepititionProtection;
    protected SpecificBossAbilityFramework[] _allActivatableBossAbilities;
    protected int _attackRepetitionCounter = 0;
    

    [Space]
    [SerializeField] protected SpecificBossAbilityFramework _abilityLocked;

    protected List<SpecificBossAbilityFramework> _readyBossAttacks = new List<SpecificBossAbilityFramework>();
    protected Queue<SpecificBossAbilityFramework> _bossCooldownQueue = new Queue<SpecificBossAbilityFramework>();
    protected SpecificBossAbilityFramework _currentAbility;

    protected bool _canAutomaticallyUseAbilities = true;
        
    protected BossBase _myBossBase;

    [Header("GameObjects")]
    [SerializeField] protected GameObject _bossVisualsBase;
    [SerializeField] protected GameObject _bossSpecificUI;

    protected GameObject _storedBossUI;

    private List<HeroBase> _bossAttackTargets = new List<HeroBase>();
    private List<HeroBase> _aggroOverrides = new List<HeroBase>();

    protected Coroutine _nextAttackProcess;
    protected Coroutine _preventAttacksCoroutine;

    [Header("Animator")]
    [SerializeField] private Animator _bossSpecificAnimator;

    [Space] 
    [Header("Audio")]
    [SerializeField] private float _bossDeathAudioDelay;

    #region Fight Start
    /// <summary>
    /// Called when the fight begins to start boss functionality
    /// </summary>
    protected virtual void StartFight()
    {
        CreateSpecificBossInstance();
        
        SetUpReadyBossAbilities();

        AssignInitialHeroTargets();
        
        StartCoroutine(InitialAttackDelay());
    }

    protected virtual void CreateSpecificBossInstance()
    {
        
    }

    /// <summary>
    /// Adds a delay to the initial actions of the boss
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitialAttackDelay()
    {
        yield return new WaitForSeconds(_myBossBase.GetBossSO().GetFightStartDelay());
        StartNextAbility(false);
    }

    /// <summary>
    /// Adds the starting abilities into the list of ready attacks
    /// </summary>
    protected virtual void SetUpReadyBossAbilities()
    {
        bool[] usableAbilities;
        if (SelectionManager.Instance.GetSelectedMissionStatModifiersOut(out MissionStatModifiers missionStatModifiers))
        {
            usableAbilities = missionStatModifiers.GetBossAbilitiesUsable();
            _canAutomaticallyUseAbilities = missionStatModifiers.GetCanBossAutomaticallyUseAbilities();
        }
        else
        {
            usableAbilities = new bool[]{true, true, true, true, true};
        }

        int totalActivatableAbilities = _startingBossAbilities.Length;
        if (!_abilityLocked.IsUnityNull())
        {
            totalActivatableAbilities++;
        }
        _allActivatableBossAbilities = new SpecificBossAbilityFramework[totalActivatableAbilities];
        
        // Iterates through each ability
        for (int i = 0; i < _startingBossAbilities.Length; i++)
        {
            _allActivatableBossAbilities[i] = _startingBossAbilities[i];
            
            if (!usableAbilities[i])
            {
                _attackRepititionProtection = Mathf.Clamp(_attackRepititionProtection -1, 0, int.MaxValue);
                continue;
            }
            AddAbilityInitiallyToBossReadyAttacks(_startingBossAbilities[i]);
        }


        if (!_abilityLocked.IsUnityNull())
        {
            _allActivatableBossAbilities[_startingBossAbilities.Length] = _abilityLocked;
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
    public virtual void RemoveHeroTarget(HeroBase heroBase)
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
                if (hb.IsUnityNull())
                {
                    Debug.Log("Could not determine hero target");
                }
                return hb;
            }
        }

        return null;
    }

    public virtual void AddHeroOverrideAggro(HeroBase heroBase)
    {
        // Prevent dead Heroes from overriding aggro
        if (heroBase.GetHeroStats().IsHeroDead())
        {
            return;
        }
        
        if (_aggroOverrides.Contains(heroBase))
        {
            return;
        }
        
        _aggroOverrides.Add(heroBase);
    }

    public virtual void RemoveHeroOverrideAggro(HeroBase heroBase)
    {
        if (!_aggroOverrides.Contains(heroBase))
        {
            return;
        }
        
        _aggroOverrides.Remove(heroBase);
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

    public virtual void DamageHero(HeroBase heroBase, float damage)
    {
        if (damage <= 0)
        {
            return;
        }

        if (heroBase.IsUnityNull())
        {
            return;
        }
            
        heroBase.GetHeroStats()
            .DealDamageToHero(damage * BossStats.Instance.GetCombinedBossDamageMultiplier());
    }

    protected virtual void AddAbilityInitiallyToBossReadyAttacks(SpecificBossAbilityFramework newAbility)
    {
        newAbility.SetIsAbilityActive(true);
        AddAbilityToBossReadyAttacks(newAbility);
    }
    
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
    protected virtual void StartNextAbility(bool isAbilityForceActivated)
    {
        if (GameStateManager.Instance.GetIsFightOver())
        {
            return;
        }
        
        if (!_canAutomaticallyUseAbilities && !isAbilityForceActivated)
        {
            return;
        }

        // Attempts to use the ability
        // Returns true if the ability was used
        if (!StartAbility(SelectNextAbility(),isAbilityForceActivated) && !isAbilityForceActivated)
        {
            // If the ability was not able to be used, try another ability
            StartNextAbility(isAbilityForceActivated);
        }
    }
    
    public virtual bool StartAbility(SpecificBossAbilityFramework bossAbility, bool isAbilityForceActivated)
    {
        if (!_canAutomaticallyUseAbilities && !isAbilityForceActivated)
        {
            return false;
        }
        
        _currentAbility = bossAbility;
        
        if (_currentAbility.GetCanAbilityBeUsed())
        {
            AddAbilityToEndOfCooldownQueue(_currentAbility);
            _nextAttackProcess = StartCoroutine(UseNextAbilityProcess(_currentAbility));
            return true;
        }
        return false;
    }

    public virtual bool StartAbility(int abilityID, bool isAbilityForceActivated)
    {
        return StartAbility(_allActivatableBossAbilities[abilityID], isAbilityForceActivated);
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
        StartNextAbility(false);
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
            //Debug.LogError("Boss was unable to stop next attack process");
            return;
        }
        
        StopCoroutine(_nextAttackProcess);
        _nextAttackProcess = null;
    }

    public virtual void SkipCurrentAttack()
    {
        StopCurrentAttack();
        
        StopNextAttackProcess();
        StartNextAbility(false);
    }

    public virtual void StopCurrentAttack()
    {
        if (!_currentAbility.IsUnityNull())
        {
            _currentAbility.StopBossAbility();
        }
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
        StartNextAbility(false);

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

        StopCurrentAttack();
    }

    /// <summary>
    /// Called when the boss is no longer staggered
    /// Virtual to allow for it to be overridden by specific bosses
    /// </summary>
    protected virtual void BossNoLongerStaggeredOccured()
    {

    }

    #region Battle Over
    /// <summary>
    /// Called when the boss is killed
    /// </summary>
    protected virtual void BossDied()
    {
        StopCurrentAttack();

        AttemptPlaySpecificBossDiedAudio();
        CheckToUnlockSpecialistAchievement();

        GeneralBattleEnd();
    }

    protected virtual void AttemptPlaySpecificBossDiedAudio()
    {
        if (_bossDeathAudioDelay > 0)
        {
            StartCoroutine(DelayPlaySpecificBossDiedAudio());
        }
        else
        {
            PlaySpecificBossDiedAudio();
        }
    }

    protected virtual IEnumerator DelayPlaySpecificBossDiedAudio()
    {
        yield return new WaitForSeconds(_bossDeathAudioDelay);
        PlaySpecificBossDiedAudio();
    }

    protected virtual void PlaySpecificBossDiedAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].BossDeathAudio);
    }

    protected virtual void CheckToUnlockSpecialistAchievement()
    {
        
    }

    protected virtual void UnlockedSpecialistAchievement()
    {
        if (_myBossBase.GetBossSO().GetAssociatedSpecialistAchievement().IsUnityNull())
        {
            return;
        }
        
        AchievementManager.Instance.UnlockAchievement(_myBossBase.GetBossSO().GetAssociatedSpecialistAchievement());
    }

    protected virtual void BossWonBattle()
    {
        //StopCurrentAttack();
        GeneralBattleEnd();
    }

    protected virtual void GeneralBattleEnd()
    {
        
    }
    #endregion

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

        if (SelectionManager.Instance.GetSelectedMissionOut(out MissionSO mission))
        {
            if (!mission.GetMissionStatModifiers().GetBossAbilitiesUsable()[_abilityLocked.GetAbilityID()])
            {
                return;
            }
        }
        
        AddAbilityInitiallyToBossReadyAttacks(_abilityLocked);
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

        GameStateManager.Instance.GetBattleLostEvent().AddListener(BossWonBattle);
        GameStateManager.Instance.GetBattleWonEvent().AddListener(BossDied);
        
        //Listens for when the boss is staggered
        BossBase.Instance.GetBossStaggeredEvent().AddListener(BossStaggerOccured);
        //Listens for when the boss stagger ends
        _myBossBase.GetBossNoLongerStaggeredEvent().AddListener(BossNoLongerStaggeredOccured);
        
        _myBossBase.GetBossHalfHealthEvent().AddListener(UnlockNewAbility);
    }
    #endregion

    #region Getters
    public Vector3 ClosestFloorSpaceOfTarget(GameObject target) =>
        EnvironmentManager.Instance.GetClosestPointToFloor(target.transform.position);

    public GameObject GetBossVisualBase() => _bossVisualsBase;

    public List<HeroBase> GetBossAttackTargets() => _bossAttackTargets;

    public Animator GetBossSpecificAnimator() => _bossSpecificAnimator;
    #endregion
    
    #region Setters

    public void SetCanAutomaticallyUseAbilities(bool canAutomaticallyUseAbilities) => _canAutomaticallyUseAbilities = canAutomaticallyUseAbilities;

    #endregion
}
