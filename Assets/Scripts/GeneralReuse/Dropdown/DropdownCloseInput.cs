using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DropdownCloseInput : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    
    private UniversalPlayerInputActions _universalPlayerInputActions;
    
    private void OnEnable()
    {
        SubscribeToInput();
    }

    private void OnDisable()
    {
        UnsubscribeFromInput();
    }

    private void CloseDropdown(InputAction.CallbackContext context)
    {
        _dropdown.Hide();
    }

    private void SubscribeToInput()
    {
        _universalPlayerInputActions = new UniversalPlayerInputActions();
        _universalPlayerInputActions.GameplayActions.Enable();
        
        _universalPlayerInputActions.GameplayActions.DirectClick.started += CloseDropdown;
    }

    private void UnsubscribeFromInput()
    {
        _universalPlayerInputActions.GameplayActions.DirectClick.started -= CloseDropdown;
        
        _universalPlayerInputActions.GameplayActions.Disable();
    }
}
