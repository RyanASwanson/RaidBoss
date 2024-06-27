using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using TMPro;

/// <summary>
/// Shows the ui for all heroes that is used while in a combat scene
/// </summary>
public class HeroUIManager : GameUIChildrenFunctionality
{
    [Header("Bottom UI")]
    [Header("Left Side")]
    [SerializeField] private Image _associatedHeroIcon;
    
    [SerializeField] private Image _associatedHeroManualAbilityChargeBar;
    [SerializeField] private Image _associatedHeroManualAbilityIcon;

    [SerializeField] private Animator _heroFullyChargedIconAnimator;

    private const string _heroFullyChargedIconTrigger = "Flash";

    [Header("RightSide")]
    [SerializeField] private Image _associatedHeroRecentHealthBar;
    [SerializeField] private Image _associatedHeroHealingHealthBar;
    [SerializeField] private Image _associatedHeroCurrentHealthBar;

    [SerializeField] private Animator _associatedHeroHealthBarCombined;

    [SerializeField] private float _timeForRecentHealthDrainStart;
    [SerializeField] private float _recentHealthDrainPercentPerSecond;

    [SerializeField] private float _timeForCurrentHealthGainStart;
    [SerializeField] private float _currentHealthGainPercentPerSecond;

    private Coroutine _startHealthBarDrainCoroutine;
    private Coroutine _startHealthBarGainCoroutine;

    private const string _combinedHealthBarStatusIntAnim = "HealthStatus";

    private HeroBase _associatedHeroBase;

    [Header("HeroWorldCanvas")]
    [SerializeField] private GameObject _damageNumber;
    [Space]
    [SerializeField] private GameObject _healingNumber;
    [Space]
    [SerializeField] private float _damageNumbersXVariance;
    [Space]
    [SerializeField] private GameObject _abilityChargedIcon;

    private RectTransform _damageNumbersOrigin;
    private RectTransform _healingNumbersOrigin;
    private RectTransform _abilityChargedOrigin;

    private const string _damageHealingWeakAnimTrigger = "WeakDamage";


    public void AssignSpecificHero(HeroBase heroBase)
    {
        _associatedHeroBase = heroBase;

        GeneralSetup();
        SetUpHeroIcons();
        SubscribeToEvents();
    }

    private void GeneralSetup()
    {
        _damageNumbersOrigin = _associatedHeroBase.GetHeroVisuals().GetDamageNumbersOrigin();
        _healingNumbersOrigin = _associatedHeroBase.GetHeroVisuals().GetHealingNumbersOrigin();
        _abilityChargedOrigin = _associatedHeroBase.GetHeroVisuals().GetAbilityChargedIconOrigin();
    }

    private void SetUpHeroIcons()
    {
        HeroSO heroSO = _associatedHeroBase.GetHeroSO();
        _associatedHeroIcon.sprite = heroSO.GetHeroIcon();
        _associatedHeroManualAbilityIcon.sprite = heroSO.GetHeroManualAbilityIcon();
        _heroFullyChargedIconAnimator.gameObject.GetComponent<Image>().sprite =
            heroSO.GetHeroManualAbilityIcon();
    }

    #region Health
    private void AssociatedHeroTookDamage(float damageTaken)
    {
        CorrectRecentHealthBar();
        SetHealthBarPercent(_associatedHeroBase.GetHeroStats().GetHeroHealthPercentage());
        StartRecentHealthBarDrain();
        ReduceRecentHealingOnDamage(damageTaken);
        CreateDamageNumbers(damageTaken);
    }

    private void AssociatedHeroTookHealing(float healingTaken)
    {
        //SetHealthBarPercent(_associatedHeroBase.GetHeroStats().GetHeroHealthPercentage());
        SetRecentHealingHealthBarPercent(_associatedHeroBase.GetHeroStats().GetHeroHealthPercentage());
        StartCurrentHealthBarGain();

        CreateHealingNumbers(healingTaken);
    }

    #region Health Bar
    private void SetHealthBarPercent(float percent)
    {
        _associatedHeroCurrentHealthBar.fillAmount = percent;
    }

    private void StartCurrentHealthBarGain()
    {
        if (_startHealthBarGainCoroutine != null)
            StopCoroutine(_startHealthBarGainCoroutine);

        _startHealthBarGainCoroutine = StartCoroutine(CurrentHealthBarGain());
    }

    private IEnumerator CurrentHealthBarGain()
    {
        yield return new WaitForSeconds(_timeForCurrentHealthGainStart);

        float currentFill = _associatedHeroCurrentHealthBar.fillAmount;

        while (currentFill < _associatedHeroHealingHealthBar.fillAmount)
        {
            currentFill += _currentHealthGainPercentPerSecond * Time.deltaTime;
            SetHealthBarPercent(currentFill);
            yield return null;
        }

        _startHealthBarGainCoroutine = null;
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

        float currentFill = _associatedHeroRecentHealthBar.fillAmount;

        while (currentFill > _associatedHeroCurrentHealthBar.fillAmount)
        {
            currentFill -= _recentHealthDrainPercentPerSecond * Time.deltaTime;
            SetRecentDamageHealthBarPercent(currentFill);
            yield return null;
        }

        _startHealthBarDrainCoroutine = null;
    }

    private void CorrectRecentHealthBar()
    {
        if (_associatedHeroRecentHealthBar.fillAmount < _associatedHeroCurrentHealthBar.fillAmount)
            SetRecentDamageHealthBarPercent(_associatedHeroCurrentHealthBar.fillAmount) ;
    }

    private void SetRecentDamageHealthBarPercent(float percent)
    {
        _associatedHeroRecentHealthBar.fillAmount = percent;
    }

    private void SetRecentHealingHealthBarPercent(float percent)
    {
        _associatedHeroHealingHealthBar.fillAmount = percent;
    }

    private void ReduceRecentHealingOnDamage(float damage)
    {
        if(_associatedHeroHealingHealthBar.fillAmount > _associatedHeroCurrentHealthBar.fillAmount)
        {
            _associatedHeroHealingHealthBar.fillAmount -= damage;
        }
    }

    #endregion

    private void HeroHealthAboveHalf()
    {
        SetCombinedHealthBarAnim(0);
    }

    private void HeroInjured()
    {
        SetCombinedHealthBarAnim(1);
    }

    private void HeroCritical()
    {
        SetCombinedHealthBarAnim(2);
    }

    private void SetCombinedHealthBarAnim(int animID)
    {
        _associatedHeroHealthBarCombined.SetInteger(_combinedHealthBarStatusIntAnim, animID);
    }

    #endregion
    /// <summary>
    /// Is called as the progress of the heroes manual ability charges
    /// </summary>
    private void AssociatedHeroManualCharging()
    {
        SetHeroManualAbilityCharge(_associatedHeroBase.GetSpecificHeroScript().GetManualAbilityChargePercent());
    }

    private void SetHeroManualAbilityCharge(float percent)
    {
        _associatedHeroManualAbilityChargeBar.fillAmount = 1- percent;
    }

    private void CreateDamageNumbers(float damage)
    {
        GameObject newDamageNumber = Instantiate(_damageNumber, _damageNumbersOrigin);

        damage = Mathf.RoundToInt(damage);
        newDamageNumber.GetComponentInChildren<Text>().text = damage.ToString();
        newDamageNumber.GetComponentInChildren<TMP_Text>().text = damage.ToString();

        AddSpawnVarianceToDamageHealingNumber(newDamageNumber);

        /*if (damage >= _strongDamage)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageStrongAnimTrigger);
        else if (damage >= _averageDamage)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageAverageAnimTrigger);
        else
        newDamageNumber.GetComponent<Animator>().SetTrigger(_damageHealingWeakAnimTrigger);*/
    }

    private void CreateHealingNumbers(float healing)
    {
        GameObject newHealingNumber = Instantiate(_healingNumber, _healingNumbersOrigin);

        healing = Mathf.RoundToInt(healing);
        newHealingNumber.GetComponentInChildren<Text>().text = healing.ToString();
        newHealingNumber.GetComponentInChildren<TMP_Text>().text = healing.ToString();

        AddSpawnVarianceToDamageHealingNumber(newHealingNumber);

        /*if (damage >= _strongDamage)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageStrongAnimTrigger);
        else if (damage >= _averageDamage)
            newDamageNumber.GetComponent<Animator>().SetTrigger(_damageAverageAnimTrigger);
        else
        newHealingNumber.GetComponent<Animator>().SetTrigger(_damageHealingWeakAnimTrigger);*/
    }

    private void AddSpawnVarianceToDamageHealingNumber(GameObject number)
    {
        number.transform.position += new Vector3(Random.Range(-_damageNumbersXVariance, _damageNumbersXVariance), 0, 0);
    }

    private void ManualFullyCharged()
    {
        CreateAbilityChargedIconAboveHero();
        HeroIconOnUIFlash();
    }

    /// <summary>
    /// Causes the icon of the heroes manual ability to appear when the ability is charged
    /// </summary>
    private void CreateAbilityChargedIconAboveHero()
    {
        GameObject newAbilityChargedIcon = Instantiate(_abilityChargedIcon, _abilityChargedOrigin);

        newAbilityChargedIcon.GetComponentInChildren<Image>().sprite =
            _associatedHeroBase.GetHeroSO().GetHeroManualAbilityIcon();
    }

    private void HeroIconOnUIFlash()
    {
        _heroFullyChargedIconAnimator.SetTrigger(_heroFullyChargedIconTrigger);
    }

    public override void ChildFuncSetup()
    {
        //base.ChildFuncSetup();
    }

    public override void SubscribeToEvents()
    {
        //Update health bar when hero takes damage
        _associatedHeroBase.GetHeroDamagedEvent().AddListener(AssociatedHeroTookDamage);
        //Update health bar when hero is healed
        _associatedHeroBase.GetHeroHealedEvent().AddListener(AssociatedHeroTookHealing);

        _associatedHeroBase.GetHeroHealedAboveHalfEvent().AddListener(HeroHealthAboveHalf);
        _associatedHeroBase.GetHeroHealedAboveQuarterEvent().AddListener(HeroInjured);
        _associatedHeroBase.GetHeroDamagedUnderHalfEvent().AddListener(HeroInjured);
        _associatedHeroBase.GetHeroDamagedUnderQuarterEvent().AddListener(HeroCritical);

        //Update manual charge bar when cooling down
        _associatedHeroBase.GetHeroManualAbilityChargingEvent().AddListener(AssociatedHeroManualCharging);
        //Creates an icon to show the player that the ability is charged
        _associatedHeroBase.GetHeroManualAbilityFullyChargedEvent().AddListener(ManualFullyCharged);
    }
}
