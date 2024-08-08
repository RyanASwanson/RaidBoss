using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDescriptionFunctionality : MonoBehaviour
{
    [SerializeField] private bool _bossOrHero;

    [Space]
    [SerializeField] private SelectionController _selectionController;
    
    public void ChangeBossHeroAbilityText()
    {
        if (_bossOrHero)
            ChangeBossAbilityText();
        else
            ChangeHeroAbilityText();
    }

    private void ChangeHeroAbilityText()
    {
        _selectionController.HeroAbilityDescriptionChanged();
    }

    private void ChangeBossAbilityText()
    {
        _selectionController.BossAbilityDescriptionChanged();
    }
}
