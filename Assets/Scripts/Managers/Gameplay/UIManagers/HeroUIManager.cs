using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using TMPro;

public class HeroUIManager : GameUIChildrenFunctionality
{
    [SerializeField] private GameObject _associatedHeroIcon;
    [SerializeField] private GameObject _associatedHeroHealthBar;
    [SerializeField] private GameObject _associatedHeroManualAbilityChargeBar;
    [SerializeField] private GameObject _associatedHeroManualAbilityIcon;
    private Image _manualChargeImage;

    private HeroBase _associatedHeroBase;


    public void AssignSpecificHero(HeroBase heroBase)
    {
        _associatedHeroBase = heroBase;

        GeneralSetup();
        SubscribeToEvents();
    }

    private void GeneralSetup()
    {
        _manualChargeImage = _associatedHeroManualAbilityChargeBar.GetComponent<Image>();
    }

    private void AssociatedHeroTookDamage(float damageTaken)
    {
        SetHealthBarPercent(_associatedHeroBase.GetHeroStats().GetHeroHealthPercentage());
    }

    private void SetHealthBarPercent(float percent)
    {
        
    }

    private void AssociatedHeroManualCharging()
    {
        SetHeroManualAbilityCharge(_associatedHeroBase.GetSpecificHeroScript().GetManualAbilityChargePercent());
    }

    private void SetHeroManualAbilityCharge(float percent)
    {
        _manualChargeImage.fillAmount = percent;
    }

    public override void ChildFuncSetup()
    {
        
    }

    public override void SubscribeToEvents()
    {
        //Update health bar when hero takes damage
        _associatedHeroBase.GetHeroDamagedEvent().AddListener(AssociatedHeroTookDamage);
        //Update manual charge bar when cooling down
        _associatedHeroBase.GetHeroManualAbilityChargingEvent().AddListener(AssociatedHeroManualCharging);
    }
}
