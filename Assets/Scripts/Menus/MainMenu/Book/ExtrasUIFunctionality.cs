using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExtrasUIFunctionality : MonoBehaviour
{
    public static ExtrasUIFunctionality Instance;
    
    [SerializeField] private CurveProgression _scaleCurve;
    [SerializeField] private CurveProgression _squishCurve;
    
    private int _currentPageIndex = 0;
    private ExtrasPage _currentPage;
    
    private ExtrasControlsDropdown _currentControlsDropdown;
    
    private UniversalPlayerInputActions _universalPlayerInputActions;


    private void Awake()
    {
        SetUpInstance();
    }

    private void SetUpInstance()
    {
        Instance = this;
    }
    
    private void OnEnable()
    {
        SubscribeToPlayerInput();
    }

    private void OnDisable()
    {
        UnsubscribeToPlayerInput();
    }
    

    public void OpenPage(ExtrasPage newPage)
    {
        if (newPage != _currentPage)
        {
            _currentPage.ClosePage();
            
            SetPageAsCurrentPage(newPage);
            _currentPageIndex = newPage.GetPageID();
            _currentPage.OpenPage();
        }
        ActivateSquishCurve();
    }

    public void ChangeTabBasedOnCurrentPage(int newTabIndex, ExtrasUITabButton tabButton)
    {
        _currentPage.SwitchTab(newTabIndex, tabButton);
    }

    public void OpenControlsDropdownButton(ExtrasControlsButton button)
    {
        _currentControlsDropdown.OpenControlsButtonDropdown(button);
    }

    public void ActivateSquishCurve()
    {
        _squishCurve.StartMovingUpOnCurve();
    }

    public void ShowExtrasUI()
    {
        _scaleCurve.StartMovingUpOnCurve();
    }

    public void HideExtrasUI()
    {
        _scaleCurve.StartMovingDownOnCurve();
    }
    
    private void CloseExtraUIPressed(InputAction.CallbackContext context)
    {
        HideExtrasUI();
    }

    private void SubscribeToPlayerInput()
    {
        _universalPlayerInputActions = new UniversalPlayerInputActions();
        _universalPlayerInputActions.GameplayActions.Enable();
        
        _universalPlayerInputActions.GameplayActions.DirectClick.started += CloseExtraUIPressed;
        _universalPlayerInputActions.GameplayActions.EscapePress.started += CloseExtraUIPressed;
    }

    private void UnsubscribeToPlayerInput()
    {
        _universalPlayerInputActions.GameplayActions.DirectClick.started -= CloseExtraUIPressed;
        _universalPlayerInputActions.GameplayActions.EscapePress.started -= CloseExtraUIPressed;
        
        _universalPlayerInputActions.GameplayActions.Disable();
    }
    
    #region Setters
    public void SetPageAsCurrentPage(ExtrasPage newPage) { _currentPage = newPage; }
    public void SetControlsDropdownAsCurrent(ExtrasControlsDropdown newDropdown) { _currentControlsDropdown = newDropdown; }
    #endregion
}
