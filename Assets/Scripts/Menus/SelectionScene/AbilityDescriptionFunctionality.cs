using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityDescriptionFunctionality : MonoBehaviour
{
    [SerializeField] private TextWithBackground _abilityNameText;
    [SerializeField] private TextWithBackground _abilityTypeText;
    [SerializeField] private TextWithBackground _abilityDescriptionText;

    public void ChangeBossAbilityDescription()
    {
        int abilityID = SelectionController.Instance.GetCurrentBossAbilityID();
        
        _abilityNameText.UpdateText(SelectionController.Instance.GetBossUIToDisplay().GetBossAbilityInformation()[abilityID]._abilityName);
        _abilityTypeText.UpdateText(SelectionController.Instance.GetBossUIToDisplay().GetBossAbilityInformation()[abilityID]._abilityType.ToString());
        _abilityDescriptionText.UpdateText(SelectionController.Instance.GetBossUIToDisplay().GetBossAbilityInformation()[abilityID]._abilityDescription);
    }
    
    public void ChangeHeroAbilityDescription()
    {
        int abilityID = SelectionController.Instance.GetCurrentHeroAbilityID();
        
        _abilityNameText.UpdateText(SelectionController.Instance.GetHeroUIToDisplay().GetAbilityNameFromID(abilityID));
        
        EHeroAbilityType type = (EHeroAbilityType)abilityID;
        _abilityTypeText.UpdateText(type.ToString());
        
        _abilityDescriptionText.UpdateText(SelectionController.Instance.GetHeroUIToDisplay().GetAbilityDescriptionFromID(abilityID));

        int lineCount = _abilityNameText.CurrentString.Split('\n').Length;
        lineCount += _abilityTypeText.CurrentString.Split('\n').Length;
        lineCount += _abilityDescriptionText.CurrentString.Split('\n').Length;
        Debug.Log("Total lines " +lineCount);
    }
    
}
