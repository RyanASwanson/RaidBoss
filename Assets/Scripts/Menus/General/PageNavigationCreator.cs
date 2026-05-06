using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PageNavigationCreator : MonoBehaviour
{
    [Space] 
    [SerializeField] private bool _doesPerformSetUpOnStart = false;
    
    [Header("Page Navigation")]
    [SerializeField] private RectTransform _leftPageHolder;
    [SerializeField] private RectTransform _rightPageHolder;
    
    [SerializeField] private Button _leftPageButton;
    [SerializeField] private Button _rightPageButton;

    [Space] 
    [SerializeField] private float _spaceBetweenSpecificPageButtons;

    [Space]
    [SerializeField] private bool _doesUseNumberInputs;
    
    [Space] 
    [SerializeField] private bool _doesTogglePageEnables = false;
    [SerializeField] private int _defaultAmountOfPages;
    [SerializeField] private GameObject[] _pages;
    
    [Space]
    [SerializeField] private GameObject _specificPageButton;
    private PageNavigationButton[] _specificPageButtons;
    [SerializeField] private Color _pressedColor;

    [Space] 
    [SerializeField] private GameObject _specificPageButtonHolder;

    [Space] 
    [SerializeField] private UnityEvent _onGeneralPageChange;
    private UnityEvent<int> _onNumberedPageChange;
    
    private int _currentPageID = 0;
    private int _previousPageID = 0;

    private int _totalPages = 0;

    private ColorBlock _buttonColorBlocks;
    
    private Vector2 _specificPageRectVector =Vector2.zero;
    private float _specificPageButtonStartX;
    
    private UniversalPlayerInputActions _universalPlayerInputActions;
    private bool _isSubscribedToInput = false;

    private void Start()
    {
        if (_doesPerformSetUpOnStart)
        {
            SetUpMissionTutorials();
        }
    }

    public void SetUpMissionTutorials()
    {
        CreateSpecificPageButtons();
        SetUpArrows();
        SetPageButtonInteractability();
        ShowStartingPages();
        SubscribeToPlayerInput();
    }

    private void OnDestroy()
    {
        UnsubscribeToPlayerInput();
    }
    
    #region ChangePage
    public void SetTargetPageNumber(int pageNumber)
    {
        if (pageNumber < 0 || pageNumber >= _totalPages)
        {
            return;
        }
        
        _specificPageButtons[_previousPageID].ButtonNoLongerPressed();
        _specificPageButtons[pageNumber].ToggleButton(false);
        
        _currentPageID = pageNumber;
        StartShowNewPage();
    }

    private void StartShowNewPage()
    {
        SetPageButtonInteractability();
        NewPageStartDisplay();
    }

    private void SetPageButtonInteractability()
    {
        _leftPageButton.interactable = (_currentPageID != 0);
        _rightPageButton.interactable = (_currentPageID < _totalPages-1);
    }

    public void NewPageStartDisplay()
    {
        if (_doesTogglePageEnables)
        {
            _pages[_currentPageID].SetActive(true);
            _pages[_previousPageID].SetActive(false);
        }

        InvokeOnGeneralPageChange();
        UpdatePreviousPageID();
    }

    public void DecreasePageNumber()
    {
        SetTargetPageNumber(_currentPageID - 1);
    }

    public void IncreasePageNumber()
    {
        SetTargetPageNumber(_currentPageID + 1);
    }

    private void UpdatePreviousPageID()
    {
        _previousPageID = _currentPageID;
    }

    private void ShowStartingPages()
    {
        if (_doesTogglePageEnables)
        {
            _pages[0].SetActive(true);
            if (_pages.Length > 1)
            {
                for (int i = 1; i < _pages.Length; i++)
                {
                    _pages[i].SetActive(false);
                }
            }
        }
    }

    #endregion
    
    #region SpecificPageButton

    private void CreateSpecificPageButtons()
    {
        _totalPages = _defaultAmountOfPages;
        _specificPageButtons = new PageNavigationButton[_totalPages];
        
        _specificPageButtonStartX = ((float)_totalPages)/2;
        _specificPageButtonStartX *= -_spaceBetweenSpecificPageButtons;

        _specificPageButtonStartX += _spaceBetweenSpecificPageButtons/2;
        
        CreateButtonColorBlock();
        
        for (int i = 0; i < _totalPages; i++)
        {
            CreateSpecificPageButton(i);
        }
    }

    private void CreateSpecificPageButton(int pageID)
    {
        PageNavigationButton pageButton = Instantiate(_specificPageButton, _specificPageButtonHolder.transform)
            .GetComponent<PageNavigationButton>();

        _specificPageButtons[pageID] = pageButton;
        
        pageButton.SetUpSpecificPageButton(this, pageID);
        pageButton.SetColorBlock(_buttonColorBlocks);
        
        _specificPageRectVector.Set((_specificPageButtonStartX)+(pageID * _spaceBetweenSpecificPageButtons), 0);
        
        pageButton.SetTutorialPageTransform(_specificPageRectVector);
    }

    private void SetUpArrows()
    {
        CreateArrowColorBlock();
        SetArrowTransforms();
    }

    private void SetArrowTransforms()
    {
        Vector2 arrowRect = Vector2.zero;
        
        arrowRect.Set(
            _specificPageButtons[0].GetButtonTransformAnchorX()-_spaceBetweenSpecificPageButtons,
            _leftPageHolder.anchoredPosition.y);
        
        _leftPageHolder.anchoredPosition = arrowRect;
        
        arrowRect.Set(
            _specificPageButtons[^1].GetButtonTransformAnchorX()+_spaceBetweenSpecificPageButtons,
            _rightPageHolder.anchoredPosition.y);
        
        _rightPageHolder.anchoredPosition = arrowRect;
    }
    
    public void PageButtonPressed(int pageNumber)
    {
        SetTargetPageNumber(pageNumber);
    }
    
    private void PageNumberPressed(InputAction.CallbackContext context)
    {
        int pressNumVal = (int)context.ReadValue<float>();
        
        SetTargetPageNumber(pressNumVal);
    }
    #endregion
    
    #region Button Colors

    private void CreateButtonColorBlock()
    {
        _buttonColorBlocks = _specificPageButton.GetComponent<PageNavigationButton>().GetAssociatedButton().colors;
        _buttonColorBlocks.disabledColor = _pressedColor;
    }

    private void CreateArrowColorBlock()
    {
        _buttonColorBlocks = _leftPageButton.colors;
        _buttonColorBlocks.disabledColor = _pressedColor;
        SetArrowColors();
    }

    private void SetArrowColors()
    {
        _leftPageButton.colors = _buttonColorBlocks;
        _rightPageButton.colors = _buttonColorBlocks;
    }
    #endregion
    
    #region Events

    private void InvokeOnGeneralPageChange()
    {
        _onGeneralPageChange?.Invoke();
    }
    #endregion
    
    private void SubscribeToPlayerInput()
    {
        if (_isSubscribedToInput)
        {
            return;
        }
        
        _universalPlayerInputActions = new UniversalPlayerInputActions();
        _universalPlayerInputActions.GameplayActions.Enable();

        if (_doesUseNumberInputs)
        {
            _universalPlayerInputActions.GameplayActions.NumberPress.started += PageNumberPressed;
        }


        _isSubscribedToInput = true;
    }

    private void UnsubscribeToPlayerInput()
    {
        if (!_isSubscribedToInput)
        {
            return;
        }

        if (_doesUseNumberInputs)
        {
            _universalPlayerInputActions.GameplayActions.NumberPress.started -= PageNumberPressed;
        }
        
        _universalPlayerInputActions.Disable();

        _isSubscribedToInput = false;
    }
}
