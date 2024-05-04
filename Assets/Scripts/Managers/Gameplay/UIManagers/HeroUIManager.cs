using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class HeroUIManager : GameUIChildrenFunctionality
{
    [SerializeField] private GameObject _associatedHeroIcon;
    [SerializeField] private GameObject _associatedHeroHealthBar;
    [SerializeField] private GameObject _associatedHeroManualAbilityChargeBar;

    private HeroBase _associatedHeroBase;


    public void AssignSpecificHero(HeroBase heroBase)
    {
        _associatedHeroBase = heroBase;

        SubscribeToEvents();
    }

    private void AssociatedHeroTookDamage(float damageTaken)
    {
        SetHealthBarPercent(_associatedHeroBase.GetHeroStats().GetHeroHealthPercentage());
    }

    private void SetHealthBarPercent(float percent)
    {
        
    }

    private void SetHeroManualAbilityCharge()
    {

    }

    public override void SubscribeToEvents()
    {
        _associatedHeroBase.GetHeroDamagedEvent().AddListener(AssociatedHeroTookDamage);
    }
}
