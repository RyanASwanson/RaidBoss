using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionModifierSelectionButton : MonoBehaviour
{
    [SerializeField] private Image _associatedModifierImage;
    
    [Space]
    [SerializeField] private MissionModifierSO _associatedMissionModifier;
    
    private bool _buttonHasBeenPressed = false;

    private void Start()
    {
        SetButtonModifierIconVisuals();
    }
    
    private void SetButtonModifierIconVisuals()
    {
        if (_associatedMissionModifier.IsUnityNull())
        {
            gameObject.SetActive(false);
            return;
        }
        
        _associatedModifierImage.sprite = _associatedMissionModifier.GetModifierSprite();

        /*SetDefaultIconColor();
        //Sets the image to be the hero icon
        _iconVisuals.sprite = _associatedHero.GetHeroIcon();

        if (!_heroButton.IsUnityNull())
        {
            //Get the colorblock for the button
            ColorBlock colorVar = _heroButton.colors;
            //Set the highlighted color for the button to be the hero highlighted color
            colorVar.highlightedColor = _associatedHero.GetHeroHighlightedColor();
            //Set the pressed color for the button to be the hero highlighted color
            colorVar.pressedColor = _associatedHero.GetHeroPressedColor();
            //Sets the colorblock for the button
            _heroButton.colors = colorVar;
        }*/
    }

    public void ButtonPressed()
    {
        if (!_buttonHasBeenPressed)
        {
            HeroSelect();
        }
        else
        {
            HeroDeselect();
        }
        
        _buttonHasBeenPressed = !_buttonHasBeenPressed;
    }
    
    private void HeroSelect()
    {
        SelectionManager.Instance.AddMissionModifier(_associatedMissionModifier);
        //UpdateHeroIconColor(_associatedHero.GetHeroSelectedColor());
    }

    private void HeroDeselect()
    {
        SelectionManager.Instance.RemoveMissionModifier(_associatedMissionModifier);
        //UpdateHeroIconColor(_defaultColor);
    }
}
