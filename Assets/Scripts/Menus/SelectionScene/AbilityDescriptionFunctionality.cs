using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityDescriptionFunctionality : ScrollUIContents
{
    [SerializeField] private bool _isBoss;
    
    [Space]
    [SerializeField] private TextWithBackground _abilityNameText;
    [SerializeField] private TextWithBackground _abilityTypeText;
    [SerializeField] private TextWithBackground _abilityDescriptionText;

    public override int UpdateContentsAndCountLines()
    {
        if (_isBoss)
        {
            ChangeBossAbilityDescription();
        }
        else
        {
            ChangeHeroAbilityDescription();
        }
        return CountLines();
    }
    
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
    }

    private int CountLines()
    {
        if (_doesCountLines)
        {
            int topLines = _abilityNameText.CurrentString.Split('\n').Length;
            int middleLines = _abilityTypeText.CurrentString.Split('\n').Length;
            int bottomLines = _abilityDescriptionText.CurrentString.Split('\n').Length;
            
            TotalLines = topLines + middleLines + bottomLines;
            bool isEven = TotalLines % 2 == 0;
            
            
            LineLength = TotalLines * _lineDistance;

            //int currentLine = TotalLines / 2;
            float currentHeight = (int)(TotalLines / 2) * _lineDistance;
            if (isEven)
            {
                currentHeight -= _lineDistance / 2;
            }
            
            
            _abilityNameText.GetRectTransform().localPosition = new Vector3(0,currentHeight, 0);

            //Debug.Log("Current line" + currentLine + " - " + topLines);
            currentHeight -= topLines * _lineDistance;
            
            
            _abilityTypeText.GetRectTransform().localPosition = new Vector3(0,currentHeight, 0);
            
            //Debug.Log("Current line" + currentLine + " - " + middleLines);
            currentHeight -= middleLines * _lineDistance;
            
            //Debug.Log("Ability description text " + currentHeight + " " + currentLine + " " + _lineDistance);
            
            _abilityDescriptionText.GetRectTransform().localPosition = new Vector3(0,currentHeight, 0);
            
            
            Debug.Log("Total lines " +TotalLines + " " + topLines + " " + middleLines + " " + bottomLines);
        }
        return TotalLines;
    }
    
}
