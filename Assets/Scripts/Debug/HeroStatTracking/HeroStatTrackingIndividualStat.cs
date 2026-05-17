using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatTrackingIndividualStat : MonoBehaviour
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    
    [SerializeField] private TextWithBackground _statNumber;
    [SerializeField] private Image _statImage;

    public void PerformInitialStatSetUp(HeroSO hero)
    {
        _statImage.color = hero.GetHeroUIColor();
        _statImage.fillAmount = 0;
    }
    
    public void UpdateIndividualStat(float statNumberAmount, float statPercentage)
    {
        if (float.IsNaN(statPercentage))
        {
            return;
        }
        
        _statNumber.UpdateText(statNumberAmount.ToString("F1"));
        _statImage.fillAmount = statPercentage;
    }
    
    #endif
}
