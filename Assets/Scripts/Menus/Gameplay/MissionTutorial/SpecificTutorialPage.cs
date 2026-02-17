using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpecificTutorialPage : MonoBehaviour
{
    [SerializeField] private TextWithBackground _defaultText;

    [Space] 
    [SerializeField] private Transform _missionPageHolder;
    
    public virtual void SetUpPage(TutorialPage tutorialPage)
    {
        _defaultText.UpdateText(tutorialPage.DefaultText);
        _defaultText.UpdateLocation(tutorialPage.DefaultTextLocation);

        if (!tutorialPage.TutorialPageObject.IsUnityNull())
        {
            GameObject newPage = Instantiate(tutorialPage.TutorialPageObject, _missionPageHolder);
            newPage.GetComponent<RectTransform>().anchoredPosition = tutorialPage.TutorialPageObjectLocation;
            Debug.Log("Created tutorial page at " + tutorialPage.TutorialPageTitle);
        }
        
    }
}
