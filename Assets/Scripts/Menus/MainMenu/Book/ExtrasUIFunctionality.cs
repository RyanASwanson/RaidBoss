using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrasUIFunctionality : MonoBehaviour
{
    public static ExtrasUIFunctionality Instance;
    
    [SerializeField] private CurveProgression _squishCurve;
    
    private int _currentPageIndex = 0;
    private ExtrasPage _currentPage;

    private int _currentSettingsTabIndex = 0;
    private int _currentControlsTabIndex = 0;
    private int _currentCreditsTabIndex = 0;

    private void Awake()
    {
        SetUpInstance();
    }

    private void SetUpInstance()
    {
        Instance = this;
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

    public void ActivateSquishCurve()
    {
        _squishCurve.StartMovingUpOnCurve();
    }
    
    #region Setters
    public void SetPageAsCurrentPage(ExtrasPage newPage) { _currentPage = newPage; }
    #endregion
}
