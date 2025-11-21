using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExtrasPage : MonoBehaviour
{
    [SerializeField] private int _pageID;

    private int _currentTabID = 0;
    private ExtrasUITabButton _currentTabButton;

    [Space]
    [SerializeField] private Color _selectedTextColor;
    [SerializeField] private Color _deselectedTextColor;
    
    [Space] 
    [SerializeField] private GameObject _pageHolder;

    [Space]
    [SerializeField] private ExtrasUITabButton[] _tabButtons;
    [SerializeField] private ExtrasUITabData[] _tabData;
    [SerializeField] private GameObject[] _tabs;
    [SerializeField] private ExtrasControlsDropdown[] _associatedDropdowns;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_pageID == 0)
        {
            ExtrasUIFunctionality.Instance.SetPageAsCurrentPage(this);
            TogglePageHolder(true);
        }
        else
        {
            TogglePageHolder(false);
        }

        SetUpTabs();
        SetStartingTab();
    }

    private void SetUpTabs()
    {
        for (int i = 0; i < _tabButtons.Length; i++)
        {
            _tabButtons[i].SetUpPage(i, _tabData[i]);
        }
    }

    private void SetStartingTab()
    {
        if (_tabButtons.Length > 0)
        {
            _currentTabButton = _tabButtons[0];
            _currentTabButton.SetButtonColor(_selectedTextColor);
            SetCurrentDropdownOnTabOpen(0);
        }
        
    }

    public void OpenPage()
    {
        TogglePageHolder(true);
    }

    public void ClosePage()
    {
        TogglePageHolder(false);
    }

    public void SwitchTab(int tabIndex, ExtrasUITabButton tabButton)
    {
        ExtrasUIFunctionality.Instance.ActivateSquishCurve();
        
        CloseTab(_currentTabID);
        _currentTabButton.SetButtonColor(_deselectedTextColor);
        
        _currentTabID = tabIndex;
        _currentTabButton = tabButton;
        
        OpenTab(_currentTabID);
        _currentTabButton.SetButtonColor(_selectedTextColor);
        SetCurrentDropdownOnTabOpen(tabIndex);
    }
    
    public void OpenTab(int tabIndex)
    {
        _tabs[tabIndex].SetActive(true);
    }

    public void CloseTab(int tabIndex)
    {
        _tabs[tabIndex].SetActive(false);
    }
    
    

    public void SetCurrentDropdownOnTabOpen(int tabIndex)
    {
        if (!_associatedDropdowns[tabIndex].IsUnityNull())
        {
            ExtrasUIFunctionality.Instance.SetControlsDropdownAsCurrent(_associatedDropdowns[tabIndex]);
        }
    }

    public void TogglePageHolder(bool isOn)
    {
        _pageHolder.SetActive(isOn);
    }

    #region Getters

    public int GetPageID() => _pageID;

    #endregion
    
    #region Setters
    
    public void SetTabButtonAsCurrentTabButton(ExtrasUITabButton newTabButton) { _currentTabButton = newTabButton; }
    
    #endregion
}
