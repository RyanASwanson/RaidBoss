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
    
    [Space]
    [SerializeField] private CurveProgression _buttonHoverScaleCurve;
    [SerializeField] private CurveProgression _selectionCurve;
    
    [Space]
    [SerializeField] private MissionModifierSO _associatedMissionModifier;
    
    private bool _buttonHasBeenPressed = false;
    
    private Color _defaultColor;

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
    
    private void ModifierSelect()
    {
        SelectionManager.Instance.AddMissionModifier(_associatedMissionModifier);
        UpdateModifierIconColor(_associatedMissionModifier.GetModifierSelectedColor());
        
        _selectionCurve.StartMovingUpOnCurve();
    }

    private void ModifierDeselect()
    {
        SelectionManager.Instance.RemoveMissionModifier(_associatedMissionModifier);
        UpdateModifierIconColor(_defaultColor);
    }
    
    #region Getters

    public MissionModifierSO GetMissionModifier() => _associatedMissionModifier;

    #endregion
}
