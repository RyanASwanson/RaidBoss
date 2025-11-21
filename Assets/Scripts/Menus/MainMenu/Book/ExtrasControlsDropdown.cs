using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExtrasControlsDropdown : MonoBehaviour
{
    [SerializeField] private Color _selectedTextColor;
    [SerializeField] private Color _deselectedTextColor;
    
    [Space]
    [SerializeField] private ExtrasControlsButton[] _allControlsButtons;
    [SerializeField] private ExtrasUIOptionsDropdownData[] _allOptionsDropdownData;
    
    private ExtrasControlsButton _currentButton;

    private void Start()
    {
        SetUpDropdownButtons();
    }
    
    private void SetUpDropdownButtons()
    {
        for (int i = 0; i < _allControlsButtons.Length; i++)
        {
            _allControlsButtons[i].SetUpButtonDropdown(_allOptionsDropdownData[i]);
        }
    }

    public void OpenControlsButtonDropdown(ExtrasControlsButton button)
    {
        if (!_currentButton.IsUnityNull())
        {
            _currentButton.CloseButtonDropdown();
            _currentButton.SetButtonColor(_deselectedTextColor);
        }
        
        _currentButton = button;
        
        _currentButton.OpenButtonDropdown();
        _currentButton.SetButtonColor(_selectedTextColor);
    }
}
