using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpecificTutorialPage : MonoBehaviour
{
    [SerializeField] private TextWithBackground _defaultText;

    [Space] 
    [SerializeField] private Transform _missionPageHolder;
    
    [Space]
    [SerializeField] private GameObject _characterIconTutorialVisual;
    
    public virtual void SetUpPage(TutorialPage tutorialPage)
    {
        _defaultText.UpdateText(tutorialPage.DefaultText);
        _defaultText.UpdateLocation(tutorialPage.DefaultTextLocation);

        if (!tutorialPage.TutorialPageObject.IsUnityNull())
        {
            GameObject newPage = Instantiate(tutorialPage.TutorialPageObject, _missionPageHolder);
            newPage.GetComponent<RectTransform>().anchoredPosition = tutorialPage.TutorialPageObjectLocation;
        }

        if (tutorialPage.TutorialPageCharacterTutorial.HasCharacterDisplayIcon())
        {
            CharacterIconTutorialVisuals iconTutorialVisuals = 
                Instantiate(_characterIconTutorialVisual, _missionPageHolder).GetComponent<CharacterIconTutorialVisuals>();
            
            iconTutorialVisuals.SetUp(tutorialPage.TutorialPageCharacterTutorial);
        }
    }
}
