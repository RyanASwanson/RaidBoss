using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDescriptionFunctionality : MonoBehaviour
{
    [SerializeField] private SelectionController _selectionController;
    

    public void ChangeHeroAbilityText()
    {
        _selectionController.HeroAbilityDescriptionChanged();
    }

    public void ChangeBossAbilityText()
    {
        //_selectionController.HeroAbilityDescriptionChanged(_abilityID);
    }
}
