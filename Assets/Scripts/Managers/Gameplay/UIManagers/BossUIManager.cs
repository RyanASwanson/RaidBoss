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


    private BossManager _bossManager;
    private BossBase _bossBase;


    #region Health Bar
    private void BossTookDamage(float damage)
    {

        SetHealthBarPercentage(damage);
        StartRecentHealthBarDrain();

        CreateDamageStaggerNumber(true, damage, _damageNumber, _damageNumbersOrigin);
    }

    private void SetHealthBarPercentage(float damage)
    {
        float fillPercent = GameplayManagers.Instance.
                GetBossManager().GetBossBase().GetBossStats().GetBossHealthPercentage();

        foreach (Image bar in _healthBars)
        {
            bar.fillAmount = fillPercent;
        }
    }

    private void StartRecentHealthBarDrain()
    {
        if (_startHealthBarDrainCoroutine != null)
            StopCoroutine(_startHealthBarDrainCoroutine);

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
        SetStaggerBarPercentage(stagger);
        StartRecentStaggerBarDrain();

        CreateDamageStaggerNumber(false, stagger, _staggerNumber, _staggerNumbersOrigin);
    }

    private void BossFullyStaggered()
    {
        StopCoroutine(_startStaggerBarDrainCoroutine);

        SetRecentStaggerBarPercentage(1);
    }


    private void ResetStaggerBar()
    {
        foreach (Image bar in _staggerBars)
            bar.fillAmount = 0;
        foreach (Image bar in _staggerRecentBars)
            bar.fillAmount = 0;
    }

    private void SetStaggerBarPercentage(float stagger)
    {
        float fillPercent = GameplayManagers.Instance.
                GetBossManager().GetBossBase().GetBossStats().GetBossStaggerPercentage();
        foreach (Image bar in _staggerBars)
        {
            bar.fillAmount = fillPercent;
        }
    }

    private void StartRecentStaggerBarDrain()
    {
        if (_startStaggerBarDrainCoroutine != null)
            StopCoroutine(_startStaggerBarDrainCoroutine);

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

    #endregion

    #region Boss Specific UI

    public GameObject AddBossUIToHolder(GameObject bossUI)
    {
        GameObject newUI = Instantiate(bossUI, _bossSpecificUIHolder.transform);
        //newUI.GetComponent<SpecificBossUIFramework>().SetupBossSpecificUIFunctionality(_myBossBase, this)
        return newUI;
    }    
    #endregion

    /*/// <summary>
    /// Create the damage numbers from the boss taking damage
    /// </summary>
    /// <param name="damage"></param> The amount of damage the boss took
    private void CreateDamageNumbers(float damage)
    {
        //Spawns in the damage number at the bosses location
        //Attached to boss canvas at the damage number origin
        GameObject newDamageNumber = Instantiate(_damageNumber, _damageNumbersOrigin);

        //Rounds the damage to the nearest int to create cleaner damage numbers
        damage = Mathf.RoundToInt(damage);

        //Updates the text on the damage number
        newDamageNumber.GetComponentInChildren<Text>().text = damage.ToString();
        newDamageNumber.GetComponentInChildren<TMP_Text>().text = damage.ToString();

        //Add variance to the spawn location
        AddSpawnVarianceToDamageStaggerNumber(newDamageNumber);

        //Performs a different animation on the damage number depending on how much damage was dealt
        if(damage >= _strongDamage)
        {
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageStrongAnimTrigger);
            //Shows time on large damage dealt
            UniversalManagers.Instance.GetTimeManager().LargeHeroDamageStaggerTimeSlow();
        }
        else if (damage >= _averageDamage)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageAverageAnimTrigger);
        else
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageStaggerWeakAnimTrigger);
    }


    private void CreateStaggerNumbers(float stagger)
    {
        GameObject newStaggerNumber = Instantiate(_staggerNumber, _staggerNumbersOrigin);

        stagger = Mathf.RoundToInt(stagger);
        newStaggerNumber.GetComponentInChildren<Text>().text = stagger.ToString();
        newStaggerNumber.GetComponentInChildren<TMP_Text>().text = stagger.ToString();

        AddSpawnVarianceToDamageStaggerNumber(newStaggerNumber);

        if (stagger >= _strongStagger)
        {
            newStaggerNumber.GetComponent<Animator>().SetTrigger(_staggerStrongAnimTrigger);
            UniversalManagers.Instance.GetTimeManager().LargeHeroDamageStaggerTimeSlow();
        }
        else if (stagger >= _averageStagger)
            newStaggerNumber.GetComponent<Animator>().SetTrigger(_staggerAverageAnimTrigger);
        else
            newStaggerNumber.GetComponent<Animator>().SetTrigger(_damageStaggerWeakAnimTrigger);
    }*/

    private void CreateDamageStaggerNumber(bool isDamage, float damageStagger, GameObject number, RectTransform spawnOrigin)
    {
        GameObject newNumber = Instantiate(number, spawnOrigin);

        damageStagger = Mathf.RoundToInt(damageStagger);

        //Makes sure the value shown isn't less than 1
        if (damageStagger <= 0)
            damageStagger = 1;

        newNumber.GetComponentInChildren<Text>().text = damageStagger.ToString();
        newNumber.GetComponentInChildren<TMP_Text>().text = damageStagger.ToString();

        AddSpawnVarianceToDamageStaggerNumber(newNumber);

        PerformAnimationOnDamageStaggerNumber(isDamage, damageStagger, newNumber);
        
    }

    

    /// <summary>
    /// Adds variance to the location of the damage/stagger number
    /// </summary>
    /// <param name="number"></param>
    private void AddSpawnVarianceToDamageStaggerNumber(GameObject number)
    {
        number.transform.position += new Vector3(Random.Range(-_damageNumbersXVariance, _damageNumbersXVariance), 0, 0);
    }

    private void PerformAnimationOnDamageStaggerNumber(bool isDamage, float damageStagger, GameObject newNumber)
    {
        Animator numberAnimator = newNumber.GetComponent<Animator>();

        if (isDamage)
        {
            //Performs a different animation on the damage number depending on how much damage was dealt
            if (damageStagger >= _strongDamage)
            {
                numberAnimator.SetTrigger(DAMAGE_STRONG_ANIM_TRIGGER);
                //Shows time on large damage dealt
                UniversalManagers.Instance.GetTimeManager().LargeHeroDamageStaggerTimeSlow();
            }
            else if (damageStagger >= _averageDamage)
                numberAnimator.SetTrigger(DAMAGE_AVERAGE_ANIM_TRIGGER);
            else
                numberAnimator.SetTrigger(DAMAGE_STAGGER_WEAK_ANIM_TRIGGER);
        }
        else
        {
            if (damageStagger >= _strongStagger)
            {
                numberAnimator.SetTrigger(STAGGER_STRONG_ANIM_TRIGGER);
                UniversalManagers.Instance.GetTimeManager().LargeHeroDamageStaggerTimeSlow();
            }
            else if (damageStagger >= _averageStagger)
                numberAnimator.SetTrigger(STAGGER_AVERAGE_ANIM_TRIGGER);
            else
                numberAnimator.SetTrigger(DAMAGE_STAGGER_WEAK_ANIM_TRIGGER);
        }
    }

    /*protected override void GetManagers()
    {
        base.GetManagers();
        _bossManager = GameplayManagers.Instance.GetBossManager();
    }*/

    protected override void SubscribeToEvents()
    {
        _bossManager = GameplayManagers.Instance.GetBossManager();
        _bossManager.GetBossBase().GetBossDamagedEvent().AddListener(BossTookDamage);
        _bossManager.GetBossBase().GetBossStaggerDealtEvent().AddListener(BossTookStagger);
        _bossManager.GetBossBase().GetBossStaggeredEvent().AddListener(BossFullyStaggered);
        _bossManager.GetBossBase().GetBossNoLongerStaggeredEvent().AddListener(ResetStaggerBar);
    }
}
