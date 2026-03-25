using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActiveMissionModifierSelectionButton : MonoBehaviour
{
    [SerializeField] private int _activeMissionModifierID;
    private MissionModifierSO _hoveredMissionModifier;

    [SerializeField] private Color _hoverColor;
    private Color _defaultColor;
    
    [SerializeField] private Image _associatedModifierImage;

    [SerializeField] private CurveProgression _selectedScaleCurve;

    private void Start()
    {
        SetButtonModifierIconVisuals();
    }

    private void SetButtonModifierIconVisuals()
    {
        _defaultColor = _associatedModifierImage.color;
        _associatedModifierImage.enabled = false;
    }
    
    public void UpdateModifierImage(bool isNewModifier)
    {
        if (SelectionManager.Instance.GetMissionModifierCount() <= _activeMissionModifierID)
        {
            RemoveModifierImage();
            return;
        }

        _associatedModifierImage.enabled = true;
        _associatedModifierImage.sprite =
            SelectionManager.Instance.GetCurrentMissionModifiers()[_activeMissionModifierID].GetModifierSprite();
        _associatedModifierImage.color = _defaultColor;

        if (isNewModifier)
        {
            _selectedScaleCurve.StartMovingUpOnCurve();
        }
    }

    public void UpdateHoveredModifierImage()
    {
        _associatedModifierImage.enabled = true;
        _associatedModifierImage.sprite = _hoveredMissionModifier.GetModifierSprite();
        _associatedModifierImage.color = _hoverColor;
    }

    public void RemoveModifierImage()
    {
        _associatedModifierImage.enabled = false;
    }

    public void ButtonPressed()
    {
        if (SelectionManager.Instance.GetCurrentMissionModifiers().Count <= _activeMissionModifierID)
        {
            return;
        }
        
        //SelectionController.Instance.ForceMissionModifierButtonPress(_activeMissionModifierID);
        SelectionController.Instance.ForceMissionModifierButtonPress(
            SelectionManager.Instance.GetCurrentMissionModifiers()[_activeMissionModifierID].GetModifierID());
    }
    
    #region Setters

    public void SetCurrentHoveredMissionModifierID(MissionModifierSO missionModifier)
    {
        _hoveredMissionModifier = missionModifier;
        UpdateHoveredModifierImage();
    }

    public void ResetCurrentHoveredMissionModifier()
    {
        _hoveredMissionModifier = null;
        UpdateModifierImage(false);
    }
    #endregion

}
