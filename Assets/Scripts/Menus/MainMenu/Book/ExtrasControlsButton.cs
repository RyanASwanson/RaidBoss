using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtrasControlsButton : MonoBehaviour
{
    [SerializeField] private TextWithBackground _associatedText;
    [SerializeField] private Image _associatedGlow;

    private Color _defaultColor;
    
    [Space]
    [SerializeField] private CurveProgression _associatedCurveProgression;

    public void SetUpButtonDropdown(ExtrasUIOptionsDropdownData dropdownData)
    {
        _associatedText.UpdateText(dropdownData._buttonText);
        
        _defaultColor = _associatedGlow.color;
    }
    
    public void SetButtonColor(Color newColor)
    {
        _associatedText.UpdateTextColor(newColor);
        _associatedGlow.color = newColor;
    }

    public void ResetButtonColor()
    {
        _associatedText.UpdateTextColor(_defaultColor);
        _associatedGlow.color = _defaultColor;
    }
    
    public void ToggleButtonDropdown()
    {
        if (_associatedCurveProgression.IsOppositeDirectionUpOnCurve())
        {
            ExtrasUIFunctionality.Instance.OpenControlsDropdownButton(this);
        }
        else
        {
            CloseButtonDropdown();
        }
        
    }
    
    public void OpenButtonDropdown()
    {
        _associatedCurveProgression.StartMovingUpOnCurve();
    }

    public void CloseButtonDropdown()
    {
        _associatedCurveProgression.StartMovingDownOnCurve();
        ResetButtonColor();
    }
}

[System.Serializable]
public class ExtrasUIOptionsDropdownData
{
    [TextArea(1, 2)] public string _buttonText;
    public Sprite[] _associatedSprites;
}