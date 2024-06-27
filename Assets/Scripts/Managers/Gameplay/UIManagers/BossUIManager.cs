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

    private const string _damageStaggerWeakAnimTrigger = "WeakDamage";
    private const string _damageAverageAnimTrigger = "AverageDamage";
    private const string _staggerAverageAnimTrigger = "AverageStagger";
    private const string _damageStrongAnimTrigger = "StrongDamage";
    private const string _staggerStrongAnimTrigger = "StrongStagger";

    private void BossTookDamage(float damage)
    {

        SetHealthBarPercentage(damage);
        StartRecentHealthBarDrain();

        CreateDamageNumbers(damage);
    }


    #region Health Bar
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

    private void BossTookStagger(float stagger)
    {
        SetStaggerBarPercentage(stagger);
        StartRecentStaggerBarDrain();
        CreateStaggerNumbers(stagger);
    }

    private void BossFullyStaggered()
    {
        StopCoroutine(_startStaggerBarDrainCoroutine);

        SetRecentStaggerBarPercentage(1);
    }

    #region Stagger Bar
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

    private void CreateDamageNumbers(float damage)
    {
        /*Vector3 damageNumberSpawnLocation = new Vector3(_damageStaggerOrigin.transform.localPosition.x,
            _damageStaggerOrigin.transform.localPosition.y, _damageStaggerOrigin.transform.localPosition.z);*/

        GameObject newDamageNumber = Instantiate(_damageNumber, _damageNumbersOrigin);

        damage = Mathf.RoundToInt(damage);
        newDamageNumber.GetComponentInChildren<Text>().text = damage.ToString();
        newDamageNumber.GetComponentInChildren<TMP_Text>().text = damage.ToString();

        AddSpawnVarianceToDamageStaggerNumber(newDamageNumber);

        if(damage >= _strongDamage)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageStrongAnimTrigger);
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
            newStaggerNumber.GetComponent<Animator>().SetTrigger(_staggerStrongAnimTrigger);
        else if (stagger >= _averageStagger)
            newStaggerNumber.GetComponent<Animator>().SetTrigger(_staggerAverageAnimTrigger);
        else
            newStaggerNumber.GetComponent<Animator>().SetTrigger(_damageStaggerWeakAnimTrigger);
    }

    private void AddSpawnVarianceToDamageStaggerNumber(GameObject number)
    {
        number.transform.position += new Vector3(Random.Range(-_damageNumbersXVariance, _damageNumbersXVariance), 0, 0);
    }

    public override void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossDamagedEvent().AddListener(BossTookDamage);
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossStaggerDealtEvent().AddListener(BossTookStagger);
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossStaggeredEvent().AddListener(BossFullyStaggered);
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossNoLongerStaggeredEvent()
            .AddListener(ResetStaggerBar);
    }
}
