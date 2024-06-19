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

    [Header("RightSide")]
    [SerializeField] private Image _associatedHeroRecentHealthBar;
    [SerializeField] private Image _associatedHeroCurrentHealthBar;

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
    }

    private void AssociatedHeroTookDamage(float damageTaken)
    {
        SetHealthBarPercent(_associatedHeroBase.GetHeroStats().GetHeroHealthPercentage());
        CreateDamageNumbers(damageTaken);
    }

    private void AssociatedHeroTookHealing(float healingTaken)
    {
        SetHealthBarPercent(_associatedHeroBase.GetHeroStats().GetHeroHealthPercentage());
        CreateHealingNumbers(healingTaken);
    }

    private void SetHealthBarPercent(float percent)
    {
        _associatedHeroCurrentHealthBar.fillAmount = percent;
    }

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
        else*/
        newDamageNumber.GetComponent<Animator>().SetTrigger(_damageHealingWeakAnimTrigger);
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
        else*/
        newHealingNumber.GetComponent<Animator>().SetTrigger(_damageHealingWeakAnimTrigger);
    }

    private void AddSpawnVarianceToDamageHealingNumber(GameObject number)
    {
        number.transform.position += new Vector3(Random.Range(-_damageNumbersXVariance, _damageNumbersXVariance), 0, 0);
    }

    private void ManualFullyCharged()
    {
        CreateAbilityChargedIconAboveHero();
    }

    private void CreateAbilityChargedIconAboveHero()
    {
        GameObject newAbilityChargedIcon = Instantiate(_abilityChargedIcon, _abilityChargedOrigin);

        newAbilityChargedIcon.GetComponentInChildren<Image>().sprite =
            _associatedHeroBase.GetHeroSO().GetHeroManualAbilityIcon();
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
        //Update manual charge bar when cooling down
        _associatedHeroBase.GetHeroManualAbilityChargingEvent().AddListener(AssociatedHeroManualCharging);
        //Creates 
        _associatedHeroBase.GetHeroManualAbilityFullyChargedEvent().AddListener(ManualFullyCharged);
    }
}
