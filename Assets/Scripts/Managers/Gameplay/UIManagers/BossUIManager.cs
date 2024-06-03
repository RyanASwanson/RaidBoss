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
    [SerializeField] private Image _healthRecentBar;
    [SerializeField] private Image _healthBar;

    [SerializeField] private float _timeForRecentHealthDrainStart;

    private Coroutine _startHealthBarDrainCoroutine;

    [Header("BossStaggerBar")]
    [SerializeField] private Image _staggerRecentBar;
    [SerializeField] private Image _staggerBar;

    [SerializeField] private float _timeForRecentStaminaDrainStart;

    private Coroutine _startStaminaBarDrainCoroutine;

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

    private const string _damageStaggerWeakAnimTrigger = "WeakDamage";
    private const string _damageAverageAnimTrigger = "AverageDamage";
    private const string _staggerAverageAnimTrigger = "AverageStagger";
    private const string _damageStrongAnimTrigger = "StrongDamage";
    private const string _staggerStrongAnimTrigger = "StrongStagger";

    private void BossTookDamage(float damage)
    {
        SetHealthBarPercentage(damage);
        CreateDamageNumbers(damage);
    }

    private void SetHealthBarPercentage(float damage)
    {
        _healthBar.fillAmount = GameplayManagers.Instance.
            GetBossManager().GetBossBase().GetBossStats().GetBossHealthPercentage();
    }

    private void BossTookStagger(float stagger)
    {
        SetStaggerBarPercentage(stagger);
        CreateStaggerNumbers(stagger);
    }

    private void SetStaggerBarPercentage(float stagger)
    {
        _staggerBar.fillAmount = GameplayManagers.Instance.
            GetBossManager().GetBossBase().GetBossStats().GetBossStaggerPercentage();
    }

    private void CreateDamageNumbers(float damage)
    {
        /*Vector3 damageNumberSpawnLocation = new Vector3(_damageStaggerOrigin.transform.localPosition.x,
            _damageStaggerOrigin.transform.localPosition.y, _damageStaggerOrigin.transform.localPosition.z);*/

        GameObject newDamageNumber = Instantiate(_damageNumber, _damageNumbersOrigin);

        newDamageNumber.GetComponentInChildren<Text>().text = damage.ToString();
        newDamageNumber.GetComponentInChildren<TMP_Text>().text = damage.ToString();

        if(damage >= _strongDamage)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageStrongAnimTrigger);
        else if (damage >= _averageDamage)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageAverageAnimTrigger);
        else
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageStaggerWeakAnimTrigger);
    }

    private void CreateStaggerNumbers(float stagger)
    {
        GameObject newDamageNumber = Instantiate(_staggerNumber, _staggerNumbersOrigin);

        newDamageNumber.GetComponentInChildren<Text>().text = stagger.ToString();
        newDamageNumber.GetComponentInChildren<TMP_Text>().text = stagger.ToString();

        
        if (stagger >= _strongStagger)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_staggerStrongAnimTrigger);
        else if (stagger >= _averageStagger)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_staggerAverageAnimTrigger);
        else
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageStaggerWeakAnimTrigger);
    }

    public override void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossDamagedEvent().AddListener(BossTookDamage);
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossStaggerDealtEvent().AddListener(BossTookStagger);
    }
}
