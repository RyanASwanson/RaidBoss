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


    public void AssignSpecificHero(HeroBase heroBase)
    {
        _associatedHeroBase = heroBase;

        GeneralSetup();
        SubscribeToEvents();
    }

    private void GeneralSetup()
    {

    }

    private void AssociatedHeroTookDamage(float damageTaken)
    {
        SetHealthBarPercent(_associatedHeroBase.GetHeroStats().GetHeroHealthPercentage());
    }

    private void AssociatedHeroTookHealing(float healingTaken)
    {
        SetHealthBarPercent(_associatedHeroBase.GetHeroStats().GetHeroHealthPercentage());
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
        _associatedHeroManualAbilityChargeBar.fillAmount = percent;
    }

    public override void ChildFuncSetup()
    {
        
    }

    public override void SubscribeToEvents()
    {
        //Update health bar when hero takes damage
        _associatedHeroBase.GetHeroDamagedEvent().AddListener(AssociatedHeroTookDamage);
        //Update health bar when hero is healed
        _associatedHeroBase.GetHeroHealedEvent().AddListener(AssociatedHeroTookHealing);
        //Update manual charge bar when cooling down
        _associatedHeroBase.GetHeroManualAbilityChargingEvent().AddListener(AssociatedHeroManualCharging);
    }
}
