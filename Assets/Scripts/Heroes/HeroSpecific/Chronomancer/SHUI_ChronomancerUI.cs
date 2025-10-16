using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SHUI_ChronomancerUI : SpecificHeroUIFramework
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Text _textBackground;
    
    private SH_Chronomancer _associatedChronomancer;

    public void AdditionalSetUp(SH_Chronomancer chronomancer)
    {
        _associatedChronomancer = chronomancer;
    }
    
    private void UpdateSpecificHeroText(float value)
    {
        int textNum = Mathf.RoundToInt(value);
        _text.text = textNum.ToString();
        _textBackground.text = textNum.ToString();
    }
    
    #region BaseHeroUI

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        _associatedChronomancer.GetOnStoredHealingUpdated().AddListener(UpdateSpecificHeroText);
    }

    protected override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        _associatedChronomancer.GetOnStoredHealingUpdated().RemoveListener(UpdateSpecificHeroText);
    }
    #endregion
}
