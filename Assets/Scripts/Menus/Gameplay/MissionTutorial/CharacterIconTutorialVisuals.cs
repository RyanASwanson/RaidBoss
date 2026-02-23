using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIconTutorialVisuals : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _backgroundImage;
    
    private RectTransform _rectTransform;

    public void SetUp(TutorialPageCharacterTutorial tutorialPageCharacterTutorial)
    {
        if (TryGetComponent(out _rectTransform))
        {
            _rectTransform.anchoredPosition = tutorialPageCharacterTutorial.TutorialCharacterIconLocation;
        }

        _iconImage.enabled = !tutorialPageCharacterTutorial.TutorialPageCharacterIcon.IsUnityNull();
        if (_iconImage.enabled)
        {
            _iconImage.sprite = tutorialPageCharacterTutorial.TutorialPageCharacterIcon;
        }

        _backgroundImage.enabled = !tutorialPageCharacterTutorial.TutorialPageCharacterBackground.IsUnityNull();
        if (_backgroundImage.enabled)
        {
            _backgroundImage.sprite = tutorialPageCharacterTutorial.TutorialPageCharacterBackground;
        }
        
        _iconImage.transform.localScale *= tutorialPageCharacterTutorial.TutorialCharacterIconScaleMultiplier;
    }
}
