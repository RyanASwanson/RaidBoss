using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAbilityDetails : MonoBehaviour
{
    [SerializeField] private Image _characterAbilityIcon;
    [SerializeField] private TextWithBackground _characterAbilityNameText;
    [SerializeField] private TextWithBackground _characterAbilityTypeText;
    [SerializeField] private TextWithBackground _characterAbilityDescriptionText;

    public void UpdateBossAbilityDetails(BossSO bossSO,int abilityID)
    {
        _characterAbilityIcon.sprite = bossSO.GetAbilityIconFromID(abilityID);
            
        _characterAbilityNameText.UpdateText(bossSO.GetAbilityNameFromID(abilityID));
        
        _characterAbilityTypeText.UpdateText(bossSO.GetAbilityTypeFromID(abilityID).ToString());
        
        _characterAbilityDescriptionText.UpdateText(bossSO.GetAbilityWideDescriptionFromID(abilityID));
    }

    public void UpdateHeroAbilityDetails(HeroSO heroSO, int abilityID)
    {
        _characterAbilityIcon.sprite = heroSO.GetAbilityIconFromID(abilityID);
            
        _characterAbilityNameText.UpdateText(heroSO.GetAbilityNameFromID(abilityID));
            
        _characterAbilityDescriptionText.UpdateText(heroSO.GetAbilityWideDescriptionFromID(abilityID));
    }
}
