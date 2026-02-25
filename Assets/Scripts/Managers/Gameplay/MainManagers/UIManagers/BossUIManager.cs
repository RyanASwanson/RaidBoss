using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles all ui relating to the boss
/// Used in combat scenes
/// Visualizes the health bar
/// Visualizes the stamina bar
/// </summary>
public class BossUIManager : GameUIChildrenFunctionality
{
    public static BossUIManager Instance;
    
    [Header("BossHealthBar")]
    [SerializeField] private List<Image> _healthRecentBars;
    [SerializeField] private List<Image> _healthBars;

    [SerializeField] private float _timeForRecentHealthDrainStart;
    [SerializeField] private float _recentHealthDrainPercentPerSecond;

    private Coroutine _startHealthBarDrainCoroutine;

    [Header("BossStaggerBar")]
    [SerializeField] private List<Image> _staggerRecentBars;
    [SerializeField] private List<Image> _staggerBars;

    [SerializeField] private float _timeForRecentStaggerDrainStart;
    [SerializeField] private float _recentStaggerDrainPercentPerSecond;

    private Coroutine _startStaggerBarDrainCoroutine;

    [Space]
    [Header("BossSpecificUI")]
    [SerializeField] private GameObject _bossSpecificUIHolder;

    [Space]
    [Header("BossWorldCanvas")]
    [SerializeField] private RectTransform _damageNumbersOrigin;
    [SerializeField] private GameObject _damageNumber;
    [SerializeField] private int _averageDamage;
    [SerializeField] private int _strongDamage;
    [Space]
    [SerializeField] private RectTransform _staggerNumbersOrigin;
    [SerializeField] private GameObject _staggerNumber;
    [SerializeField] private int _averageStagger;
    [SerializeField] private int _strongStagger;
    [Space]
    [SerializeField] private float _damageNumbersXVariance;

    private const string DAMAGE_STAGGER_WEAK_ANIM_TRIGGER = "WeakDamage";
    private const string DAMAGE_AVERAGE_ANIM_TRIGGER = "AverageDamage";
    private const string STAGGER_AVERAGE_ANIM_TRIGGER = "AverageStagger";
    private const string DAMAGE_STRONG_ANIM_TRIGGER = "StrongDamage";
    private const string STAGGER_STRONG_ANIM_TRIGGER = "StrongStagger";
    
    private BossBase _bossBase;
    
    #region Health Bar
    /// <summary>
    /// Updates UI accordingly when the boss takes damage
    /// </summary>
    /// <param name="damage"></param>
    private void BossTookDamage(float damage)
    {
        SetHealthBarPercentage();
        StartRecentHealthBarDrain();

        CreateDamageStaggerNumber(EDamageNumberType.Damage, damage, _damageNumber, _damageNumbersOrigin);
    }

    /// <summary>
    /// Sets the health bar of the boss to accurately reflect the boss health
    /// </summary>
    private void SetHealthBarPercentage()
    {
        float fillPercent = BossStats.Instance.GetBossHealthPercentage();

        foreach (Image bar in _healthBars)
        {
            bar.fillAmount = fillPercent;
        }
    }

    private void StartRecentHealthBarDrain()
    {
        if (_startHealthBarDrainCoroutine != null)
        {
            StopCoroutine(_startHealthBarDrainCoroutine);
        }

        _startHealthBarDrainCoroutine = StartCoroutine(RecentHealthBarDrain());
    }

    private IEnumerator RecentHealthBarDrain()
    {
        yield return new WaitForSeconds(_timeForRecentHealthDrainStart);

        float currentFill = _healthRecentBars[0].fillAmount;
        float targetFill = _healthBars[0].fillAmount;

        while (currentFill > targetFill)
        {
            currentFill -= _recentHealthDrainPercentPerSecond * Time.deltaTime;
            SetRecentHealthBarPercentage(currentFill);
            yield return null;
        }

        _startHealthBarDrainCoroutine = null;
    }

    private void SetRecentHealthBarPercentage(float newFillAmount)
    {
        foreach (Image bar in _healthRecentBars)
        {
            bar.fillAmount = newFillAmount;
        }
    }

    #endregion

    #region Stagger Bar

    private void BossTookStagger(float stagger)
    {
        SetStaggerBarPercentage();
        StartRecentStaggerBarDrain();

        CreateDamageStaggerNumber(EDamageNumberType.Stagger, stagger, _staggerNumber, _staggerNumbersOrigin);
    }

    private void BossFullyStaggered()
    {
        StopCoroutine(_startStaggerBarDrainCoroutine);
        
        SetRecentStaggerBarPercentage(1);
    }

    #region Reset Stagger Bars
    /// <summary>
    /// Resets the stagger bars and the recent stagger bars to their starting values
    /// </summary>
    private void ResetAllStaggerBars()
    {
        ResetStaggerBars();

        ResetRecentStaggerBars();
    }

    /// <summary>
    /// Resets the stagger bars to 0%
    /// </summary>
    private void ResetStaggerBars()
    {
        SetStaggerBarPercentage(0);
    }

    /// <summary>
    /// Resets the recent stagger bars to 0%
    /// </summary>
    private void ResetRecentStaggerBars()
    {
        SetRecentStaggerBarPercentage(0);
    }
    #endregion
    /// <summary>
    /// Sets the stagger bar percentage without any input.
    /// Uses the current stagger percentage to set the stagger bars
    /// </summary>
    private void SetStaggerBarPercentage()
    {
        SetStaggerBarPercentage(BossStats.Instance.GetBossStaggerPercentage());
    }
    
    /// <summary>
    /// Sets the stagger bar percentage given a specific percentage
    /// </summary>
    /// <param name="percent"> The amount the stagger bar should be filled by </param>
    private void SetStaggerBarPercentage(float percent)
    {
        foreach (Image bar in _staggerBars)
        {
            bar.fillAmount = percent;
        }
    }

    private void StartRecentStaggerBarDrain()
    {
        if (_startStaggerBarDrainCoroutine != null)
        {
            StopCoroutine(_startStaggerBarDrainCoroutine);
        }

        _startStaggerBarDrainCoroutine = StartCoroutine(RecentStaggerBarDrain());
    }

    private IEnumerator RecentStaggerBarDrain()
    {
        yield return new WaitForSeconds(_timeForRecentStaggerDrainStart);

        float currentFill = _staggerRecentBars[0].fillAmount;
        float targetFill = _staggerBars[0].fillAmount;

        while (currentFill < targetFill)
        {
            currentFill += _recentStaggerDrainPercentPerSecond * Time.deltaTime;
            SetRecentStaggerBarPercentage(currentFill);
            yield return null;
        }

        _startStaggerBarDrainCoroutine = null;
    }

    private void SetRecentStaggerBarPercentage(float newFillAmount)
    {
        foreach (Image bar in _staggerRecentBars)
        {
            bar.fillAmount = newFillAmount;
        }
    }
    
    private void SetRecentStaggerBarToStaggerDurationPercentage(float staggerPercent)
    {
        SetRecentStaggerBarPercentage(1-staggerPercent);
    }
    #endregion

    #region Boss Specific UI

    public GameObject AddBossUIToHolder(GameObject bossUI)
    {
        GameObject newUI = Instantiate(bossUI, _bossSpecificUIHolder.transform);
        return newUI;
    }    
    #endregion

    #region Damage Stagger Numbers
    private void CreateDamageStaggerNumber(EDamageNumberType numberType, float damageStagger, GameObject number, RectTransform spawnOrigin)
    {
        if (damageStagger <= 0)
        {
            return;
        }
        
        GameObject newNumber = Instantiate(number, spawnOrigin);

        damageStagger = Mathf.RoundToInt(damageStagger);

        //Makes sure the value shown isn't less than 1
        if (damageStagger < 1)
        {
            damageStagger = 1;
        }

        newNumber.GetComponentInChildren<Text>().text = damageStagger.ToString();
        newNumber.GetComponentInChildren<TMP_Text>().text = damageStagger.ToString();

        AddSpawnVarianceToDamageStaggerNumber(newNumber);

        PerformAnimationOnDamageStaggerNumber(numberType, damageStagger, newNumber);
    }

    /// <summary>
    /// Adds variance to the location of the damage/stagger number
    /// </summary>
    /// <param name="number"></param>
    private void AddSpawnVarianceToDamageStaggerNumber(GameObject number)
    {
        number.transform.position += new Vector3(Random.Range(-_damageNumbersXVariance, _damageNumbersXVariance), 0, 0);
    }

    private void PerformAnimationOnDamageStaggerNumber(EDamageNumberType numberType, float damageStagger, GameObject newNumber)
    {
        Animator numberAnimator = newNumber.GetComponent<Animator>();

        switch (numberType)
        {
            case EDamageNumberType.Damage:
            {
                //Performs a different animation on the damage number depending on how much damage was dealt
                if (damageStagger >= _strongDamage)
                {
                    numberAnimator.SetTrigger(DAMAGE_STRONG_ANIM_TRIGGER);
                    //Shows time on large damage dealt
                    TimeManager.Instance.LargeHeroDamageStaggerTimeSlow();
                }
                else if (damageStagger >= _averageDamage)
                {
                    numberAnimator.SetTrigger(DAMAGE_AVERAGE_ANIM_TRIGGER);
                }
                else
                {
                    numberAnimator.SetTrigger(DAMAGE_STAGGER_WEAK_ANIM_TRIGGER);
                }

                break;
            }

            case EDamageNumberType.Stagger:
            {
                if (damageStagger >= _strongStagger)
                {
                    numberAnimator.SetTrigger(STAGGER_STRONG_ANIM_TRIGGER);
                    TimeManager.Instance.LargeHeroDamageStaggerTimeSlow();
                }
                else if (damageStagger >= _averageStagger)
                {
                    numberAnimator.SetTrigger(STAGGER_AVERAGE_ANIM_TRIGGER);
                }
                else
                {
                    numberAnimator.SetTrigger(DAMAGE_STAGGER_WEAK_ANIM_TRIGGER);
                }

                break;
            }
            case EDamageNumberType.Healing:
            {
                Debug.LogWarning("Healing attempted on boss");
                break;
            }
        }
    }
    #endregion

    #region Base UI Children
    /// <summary>
    /// Establishes the instance for the Boss UI Manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    
    /// <summary>
    /// Subscribes to any events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        BossBase.Instance.GetBossDamagedEvent().AddListener(BossTookDamage);
        BossBase.Instance.GetBossStaggerDealtEvent().AddListener(BossTookStagger);
        BossBase.Instance.GetBossStaggeredEvent().AddListener(BossFullyStaggered);
        BossBase.Instance.GetBossStaggerProcessEvent().AddListener(SetRecentStaggerBarToStaggerDurationPercentage);
        BossBase.Instance.GetBossNoLongerStaggeredEvent().AddListener(ResetAllStaggerBars);
    }
    #endregion
}
