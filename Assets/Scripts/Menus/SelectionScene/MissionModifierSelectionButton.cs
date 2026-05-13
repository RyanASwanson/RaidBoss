using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionModifierSelectionButton : MonoBehaviour
{
    [SerializeField] private Image _associatedModifierImage;
    [SerializeField] private Button _associatedModifierButton;

    [SerializeField] private GameObject _lockVisuals;
    
    [Space]
    [SerializeField] private CurveProgression _buttonHoverScaleCurve;
    [SerializeField] private CurveProgression _selectionCurve;
    
    [Space]
    [SerializeField] private MissionModifierSO _associatedMissionModifier;
    
    private bool _buttonHasBeenPressed = false;
    
    private Color _defaultColor;
    

    public void SetUpMissionModifierSelectionButton()
    {
        SetButtonLockInteractability();
        SetButtonModifierIconVisuals();
    }

    public void SetStartingMissionModifierStatusToPressed()
    {
        _buttonHasBeenPressed = true;
        UpdateModifierSelectedIconColor();
    }

    private void SetButtonLockInteractability()
    {
        if (_associatedMissionModifier.IsUnityNull())
        {
            return;
        }
        
        SetButtonInteractability(SaveManager.Instance.IsMissionModifierUnlocked(_associatedMissionModifier));
    }
    
    private void SetButtonModifierIconVisuals()
    {
        if (_associatedMissionModifier.IsUnityNull())
        {
            gameObject.SetActive(false);
            return;
        }

        SetDefaultIconColor();
        //Sets the image to be the modifier icon
        _associatedModifierImage.sprite = _associatedMissionModifier.GetModifierSprite();

        if (!_associatedModifierButton.IsUnityNull())
        {
            //Get the colorblock for the button
            ColorBlock colorVar = _associatedModifierButton.colors;
            //Set the highlighted color for the button to be the modifier highlighted color
            colorVar.highlightedColor = _associatedMissionModifier.GetModifierHighlightedColor();
            //Sets the colorblock for the button
            _associatedModifierButton.colors = colorVar;
        }
    }
    
    private void SetDefaultIconColor()
    {
        _defaultColor = _associatedModifierImage.color;
    }

    public void ButtonPressed()
    {
        if (!_buttonHasBeenPressed)
        {
            ModifierSelect();
        }
        else
        {
            ModifierDeselect();
        }
        
        _buttonHasBeenPressed = !_buttonHasBeenPressed;
    }

    public void SelectMissionModifierHoverBegin()
    {
        _buttonHoverScaleCurve.StartMovingUpOnCurve();
        SelectionManager.Instance.MissionModifierHoveredOver(_associatedMissionModifier);
    }
    
    public void SelectMissionModifierHoverEnd()
    {
        _buttonHoverScaleCurve.StartMovingDownOnCurve();
        SelectionManager.Instance.MissionModifierNotHoveredOver(_associatedMissionModifier);
    }
    
    private void UpdateModifierIconColor(Color newColor)
    {
        _associatedModifierImage.color = newColor;
    }

    private void UpdateModifierSelectedIconColor()
    {
        UpdateModifierIconColor(_associatedMissionModifier.GetModifierSelectedColor());
    }

    private void UpdateModifierDefaultIconColor()
    {
        UpdateModifierIconColor(_defaultColor);
    }
    
    private void ModifierSelect()
    {
        SelectionManager.Instance.AddMissionModifier(_associatedMissionModifier);
        UpdateModifierSelectedIconColor();
        
        _selectionCurve.StartMovingUpOnCurve();
    }

    private void ModifierDeselect()
    {
        SelectionManager.Instance.RemoveMissionModifier(_associatedMissionModifier);
        UpdateModifierDefaultIconColor();
    }
    
    #region Getters

    public MissionModifierSO GetMissionModifier() => _associatedMissionModifier;

    #endregion
    
    #region Setters
    public void SetButtonInteractability(bool interactable)
    {
        if (!_associatedModifierButton.IsUnityNull())
        {
            _associatedModifierButton.interactable = interactable;
        }
        
        _lockVisuals.SetActive(!interactable);
    }
    #endregion
}
