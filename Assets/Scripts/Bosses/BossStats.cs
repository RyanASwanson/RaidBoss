using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Holds all general stats that bosses have as well as the functionality for
///     affecting those stats
/// Holds health and stagger
/// </summary>
public class BossStats : BossChildrenFunctionality
{
    public static BossStats Instance;
    
    private float _bossMaxHealth;
    private float _currentHealth;

    private float _bossDefaultStaggerMax;
    private float _currentStaggerCounter;

    private float _bossStaggerDuration;

    private float _bossDamageIncrementMultiplier;
    private float _bossDamageResistanceChangeOnStagger;

    private bool _isBossDead = false;
    private bool _isBossStaggered = false;
    private bool _isBossEnraged = false;

    private float _baseBossDamageMultiplier = 1;
    private float _bossDamageResistanceMultiplier =1;

    private float _bossLowHealthDamageMultiplier = 1;
    
    private float _bossEnrageDamageMultiplier = 1;
    private float _storedEnrageMultiplier;

    private float _scalingEnrageDamageMultiplierScaleRate;
    private float _storedScalingEnrageDamageMultiplier = 1;

    private float _currentTimeUntilEnrage;
    private float _enrageMaxTime;
    private float _timeUntilEnrageProgress = 0;
    private float _timeSpentEnraged = 0;
    private Coroutine _enrageCoroutine;
    
    [SerializeField] private float _bossEnrageWarningTime;
    [SerializeField] private float _bossEnrageImpendingTime;
    private bool _hasEnrageWarningBegun = false;
    private bool _hasEnrageImpendingBegun = false;

    private float _bossEnragedWarningProgress = 0;

    #region Set Up
    /// <summary>
    /// Called when the Scriptable Object for the boss is assigned
    /// </summary>
    /// <param name="bossSO"> The scriptable object of the boss </param>
    private void BossSOAssigned(BossSO bossSO)
    {
        StatsSetUp(bossSO);
    }
    
    private void StatsSetUp(BossSO bossSO)
    {
        //Set the boss max health to be the max hp from the SO and multiplied by the difficulty modifier
        _bossMaxHealth = bossSO.GetMaxHP() * 
            SelectionManager.Instance.GetHealthMultiplierFromDifficulty();
        
        _bossDefaultStaggerMax = bossSO.GetBaseStaggerMax() *
            SelectionManager.Instance.GetStaggerMultiplierFromDifficulty();

        //Sets the stagger duration from the SO
        _bossStaggerDuration = bossSO.GetStaggerDuration();
        //Sets the damage scaling from the SO
        _bossDamageIncrementMultiplier = bossSO.GetBossDamageIncrementMultiplier();
        //Sets the damage resistance taken when staggered from the SO
        _bossDamageResistanceChangeOnStagger = bossSO.GetDamageResistanceChangeOnStagger();
        
        //Sets the damage dealt multiplier based on the difficulty
        _baseBossDamageMultiplier = SelectionManager.Instance.GetDamageMultiplierFromDifficulty();

        _enrageMaxTime = bossSO.GetEnrageTime();
        _currentTimeUntilEnrage = _enrageMaxTime;
        _storedEnrageMultiplier = bossSO.GetEnrageDamageMultiplier();

        // Gets the scaling enrage damage multiplier rate and divides it by 60 to get the rate in seconds
        _scalingEnrageDamageMultiplierScaleRate = bossSO.GetEnrageScalingDamageMultiplierIncreasePerSecond();

        if (SelectionManager.Instance.GetSelectedMissionStatModifiersOut(out MissionStatModifiers missionStatModifiers))
        {
            _bossMaxHealth *= missionStatModifiers.GetBossHealthMultiplier();

            _bossDefaultStaggerMax *= missionStatModifiers.GetBossStaggerMultiplier();
            
            _bossDamageResistanceChangeOnStagger *=
                missionStatModifiers.GetBossDamageResistanceChangeOnStaggerMultiplier();
            
            _currentTimeUntilEnrage *= missionStatModifiers.GetBossEnrageTimeMultiplier();

            _storedEnrageMultiplier *= missionStatModifiers.GetBossEnrageDamageMultiplier();
        }
        
        GameplayModifiersManager.Instance.AdjustBossStatsFromModifiers(this);

        if (SelectionManager.Instance.IsPlayingFreeMode())
        {
            _bossMaxHealth *= SelectionManager.Instance.GetHealthMultiplierFromMythicPlusLevel();

            _bossDefaultStaggerMax *= SelectionManager.Instance.GetStaggerMultiplierFromMythicPlusLevel();
            
            _baseBossDamageMultiplier *= SelectionManager.Instance.GetDamageMultiplierFromMythicPlusLevel();
        }
        
        //Sets the starting health and stagger values
        _currentHealth = _bossMaxHealth;
        _currentStaggerCounter = 0;

        _storedEnrageMultiplier += 1;
        _enrageMaxTime = _currentTimeUntilEnrage;
    }
#endregion

    #region Health
    private void CheckBossIsUnderHalf(float damage)
    {
        if(GetBossHealthPercentage() < .5f)
        {
            //Invokes any necessary functionality for reaching half health
            _myBossBase.InvokeBossHalfHealthEvent();
            IncreaseBossStatsAtHealthThresholds();

            _myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderHalf);
            _myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderQuarter);

            CheckBossIsUnderQuarter(damage);
        }
    }

    private void CheckBossIsUnderQuarter(float damage)
    {
        if (GetBossHealthPercentage() < .25f)
        {
            //Invokes any necessary functionality for reaching quarter health
            _myBossBase.InvokeBossQuarterHealthEvent();
            //Increases boss stats
            IncreaseBossStatsAtHealthThresholds();

            //No longer listens for damage event
            _myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderQuarter);
            //Causes the 10 percent health check to listen for the damage event
            _myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderTenth);

            CheckBossIsUnderTenth(damage);
        }
    }

    private void CheckBossIsUnderTenth(float damage)
    {
        if (GetBossHealthPercentage() < .1f)
        {
            //Invokes any necessary functionality for reaching tenth health
            _myBossBase.InvokeBossTenthHealthEvent();
            //Increases boss stats
            IncreaseBossStatsAtHealthThresholds();

            //No longer listens for damage event
            _myBossBase.GetBossDamagedEvent().RemoveListener(CheckBossIsUnderTenth);
            //Causes the death check to listen for the damage event
            _myBossBase.GetBossDamagedEvent().AddListener(CheckIfBossIsDead);

            CheckIfBossIsDead(damage);
        }
    }

    private void CheckIfBossIsDead(float damage)
    {
        if (_currentHealth <= 0 && !GameStateManager.Instance.GetIsFightOver())
        {
            BossDeath();
        }
    }

    private void BossDeath()
    {
        _isBossDead = true;

        StopEnrageTimer();
        
        GameStateManager.Instance.SetGameplayState(EGameplayStates.PostBattleWon);
    }

    protected virtual void IncreaseBossStatsAtHealthThresholds()
    {
        MultiplierBossLowHealthDamageMultiplier(_bossDamageIncrementMultiplier);
    }
    #endregion

    #region Stagger

    /// <summary>
    /// Checks if the boss is above their stagger cap
    /// If they are the boss becomes staggered
    /// </summary>
    private void CheckIfBossIsStaggered()
    {
        if (_currentStaggerCounter >= _bossDefaultStaggerMax)
        {
            BossStaggered();
        }
    }

    /// <summary>
    /// Staggers the boss and shoots out the event
    /// </summary>
    private void BossStaggered()
    {
        _isBossStaggered = true;
        StopEnrageTimer();
        
        // Stops the boss turning
        BossVisuals.Instance.StopBossLookAt();
        
        // Play the stagger audio sfx
        AudioManager.Instance.PlaySpecificAudio(AudioManager.Instance.GeneralBossAudio.HealthStaggerAudio.BossStaggered);
        
        _myBossBase.InvokeBossStaggeredEvent();

        DecreaseBossDamageResistanceOnStagger();
    }

    /// <summary>
    /// Activated by event
    /// Occurs when the boss stagger has run out
    /// </summary>
    private void BossNoLongerStaggered()
    {
        _isBossStaggered = false;
        StartEnrageTimer();
        _currentStaggerCounter = 0;

        IncreaseBossDamageResistanceAfterStagger();
    }

    #endregion

    #region Enrage
    /// <summary>
    /// Starts the enrage timer for the boss
    /// </summary>
    private void StartEnrageTimer()
    {
        // Checks if the boss is already enraged
        if (_isBossEnraged)
        {
            // Stop as we don't need to start the enrage timer if the boss is already enraged
            return;
        }

        StopEnrageTimer();
        
        // Start the timer as the timer isn't active yet
        _enrageCoroutine = StartCoroutine(BossEnrageCounter());
    }

    /// <summary>
    /// Stops the enrage timer
    /// </summary>
    private void StopEnrageTimer()
    {
        // Check if the enrage process is active
        if (!_enrageCoroutine.IsUnityNull())
        {
            // Stop the enrage timer
            StopCoroutine(_enrageCoroutine);
            _enrageCoroutine = null;
        }
    }

    /// <summary>
    /// The process of counting down until the boss enrages
    /// </summary>
    /// <returns></returns>
    private IEnumerator BossEnrageCounter()
    {
        while(_currentTimeUntilEnrage > 0)
        {
            DecreaseTimeUntilEnraged(Time.deltaTime);
            
            yield return null;
        }

        EnrageMax();
    }
    

    private void BossEnrageImpending()
    {
        AudioManager.Instance.PlaySpecificAudio(AudioManager.Instance.GeneralBossAudio.EnrageAudio.BossEnrageImpending);
        _hasEnrageImpendingBegun = true;
    }
    
    public void BeginBossEnrageWarning()
    {
        BossEnrageWarningStartSFX();
        BossBase.Instance.InvokeBossEnrageCountdownBegunEvent();
    }

    private void BossEnrageWarningStartSFX()
    {
        AudioManager.Instance.PlaySpecificAudio(AudioManager.Instance.GeneralBossAudio.EnrageAudio.BossEnrageWarningStart);
    }

    /// <summary>
    /// Called when the boss reaches max enrage
    /// </summary>
    private void EnrageMax()
    {
        _isBossEnraged = true;
        _bossEnrageDamageMultiplier = _storedEnrageMultiplier;
        _myBossBase.InvokeBossEnragedEvent();
        _myBossBase.InvokeBossEnrageSecondPassedEvent();
        
        AudioManager.Instance.PlaySpecificAudio(AudioManager.Instance.GeneralBossAudio.EnrageAudio.BossEnrageStarted);

        _timeSpentEnraged = 0;
        StartScalingEnrageProcess();
    }

    private void StartScalingEnrageProcess()
    {
        StartCoroutine(ScalingEnrageProcess());
    }

    private IEnumerator ScalingEnrageProcess()
    {
        float secondCounter = 0;
        while (true)
        {
            _timeSpentEnraged += Time.deltaTime;
            secondCounter += Time.deltaTime;

            while (secondCounter > 1)
            {
                secondCounter -= 1;
                SecondPassedEnraged();
            }
            
            yield return null;
        }
    }

    private void SecondPassedEnraged()
    {
        CalculateScalingEnrageDamageMultiplier();
        
        _myBossBase.InvokeBossEnrageSecondPassedEvent();
    }

    private void CalculateScalingEnrageDamageMultiplier()
    {
        _storedScalingEnrageDamageMultiplier = 1 + _scalingEnrageDamageMultiplierScaleRate * _timeSpentEnraged;
    }
    #endregion

    #region Stat Changes

    /// <summary>
    /// Increases or decreases the current aggro value
    /// </summary>
    /// <param name="changeValue"></param>
    public void ChangeBossDamageResistance(float changeValue)
    {
        _bossDamageResistanceMultiplier+= changeValue;
        
        _bossDamageResistanceMultiplier = Mathf.Clamp(_bossDamageResistanceMultiplier, 0.001f, int.MaxValue);
    }

    private void DecreaseBossDamageResistanceOnStagger()
    {
        ChangeBossDamageResistance(-_bossDamageResistanceChangeOnStagger);
    }

    private void IncreaseBossDamageResistanceAfterStagger()
    {
        ChangeBossDamageResistance(_bossDamageResistanceChangeOnStagger);
    }
    #endregion

    #region Base Children Functionality
    /// <summary>
    /// Establishes the instance for the Boss Stats
    /// </summary>
    protected override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    public override void SubscribeToEvents()
    {
        _myBossBase.GetSOSetEvent().AddListener(BossSOAssigned);

        _myBossBase.GetBossNoLongerStaggeredEvent().AddListener(BossNoLongerStaggered);

        _myBossBase.GetBossDamagedEvent().AddListener(CheckBossIsUnderHalf);

        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(StartEnrageTimer);
    }
    #endregion

    #region Getters
    public float GetBossMaxHealth() => _bossMaxHealth;
    public float GetBossCurrentHealth() => _currentHealth;
    public float GetBossHealthPercentage() => _currentHealth / _bossMaxHealth;

    public float GetBossMaxStagger() => _bossDefaultStaggerMax;
    public float GetBossCurrentStaggerAmount() => _currentStaggerCounter;
    public float GetBossStaggerPercentage() => _currentStaggerCounter / _bossDefaultStaggerMax;

    public float GetStaggerDuration() => _bossStaggerDuration;
    
    public float GetBossDamageResistanceChangeOnStagger() => _bossDamageResistanceChangeOnStagger;

    public bool GetIsBossStaggered() => _isBossStaggered;
    public bool GetIsBossEnraged() => _isBossEnraged;
    public float GetSecondsSpentEnraged() => _timeSpentEnraged;
    public float GetMinutesSpentEnraged() => _timeSpentEnraged / 60f;
    
    public float GetBaseBossDamageMultiplier() => _baseBossDamageMultiplier;

    public float GetBossEnrageDamageMultiplier() => _bossEnrageDamageMultiplier;

    public float GetCombinedBossDamageMultiplier() => _baseBossDamageMultiplier * _bossLowHealthDamageMultiplier 
        * _bossEnrageDamageMultiplier * _storedScalingEnrageDamageMultiplier;
    #endregion

    #region Setters
    public void DealDamageToBoss(float damage)
    {
        // Stop if the boss is already dead
        if (_isBossDead)
        {
            return;
        }
        
        if (damage <= 0)
        {
            return;
        }
        
        damage /= _bossDamageResistanceMultiplier;
        _currentHealth -= damage;
        _myBossBase.InvokeBossDamagedEvent(damage);
        
        AudioManager.Instance.PlaySpecificAudio(AudioManager.Instance.GeneralBossAudio.HealthStaggerAudio.BossTookDamage);
    }

    public void DealDamageToBossFromNonHeroSource(float damage)
    {
        DealDamageToBoss(damage);
    }

    public void DealStaggerToBoss(float stagger)
    {
        // Stop if the boss is already dead or staggered
        if (_isBossDead || _isBossStaggered)
        {
            return;
        }

        if (stagger <= 0)
        {
            return;
        }

        _currentStaggerCounter += stagger;
        _myBossBase.InvokeBossStaggerDealt(stagger);
        CheckIfBossIsStaggered();
    }
    
    public void DealStaggerToBossFromNonHeroSource(float damage)
    {
        DealStaggerToBoss(damage);
    }

    public void DecreaseTimeUntilEnraged(float enrageTime)
    {
        _currentTimeUntilEnrage -= enrageTime;
        _timeUntilEnrageProgress = 1 - (_currentTimeUntilEnrage / _enrageMaxTime);
        _myBossBase.InvokeBossEnrageProgressUpdatedEvent(_timeUntilEnrageProgress);

        if (!_hasEnrageWarningBegun)
        {
            if (_currentTimeUntilEnrage <= _bossEnrageWarningTime)
            {
                _hasEnrageWarningBegun = true;
                BeginBossEnrageWarning();
            }
        }
        else
        {
            if (!_hasEnrageImpendingBegun && _currentTimeUntilEnrage <= _bossEnrageImpendingTime)
            {
                BossEnrageImpending();
            }
                
            _bossEnragedWarningProgress = 1 - (_currentTimeUntilEnrage / _bossEnrageWarningTime);
            BossBase.Instance.InvokeBossEnrageCountdownProgressUpdatedEvent(_bossEnragedWarningProgress);
        }
    }

    public void MultiplyBossDamageMultiplier(float amount)
    {
        _baseBossDamageMultiplier *= amount;
    }

    public void MultiplierBossLowHealthDamageMultiplier(float amount)
    {
        _bossLowHealthDamageMultiplier *= amount;
    }
    
    public float SetBossMaxHealth(float value) => _bossMaxHealth = value;
    public float SetBossMaxStagger(float value) => _bossDefaultStaggerMax = value;
    public float SetBossDamageResistanceChangeOnStagger(float value) => _bossDamageResistanceChangeOnStagger = value;
    #endregion
}
