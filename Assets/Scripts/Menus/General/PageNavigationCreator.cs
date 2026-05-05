using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    [SerializeField] private int _defaultAmountOfPages;
    
    [Space]
    [SerializeField] private GameObject _specificPageButton;
    private PageNavigationButton[] _specificPageButtons;

    [Space] 
    [SerializeField] private GameObject _specificPageButtonHolder;
    
    private int _currentPageID = 0;
    private int _previousPageID = 0;

    private int _totalPages = 0;

    private bool _isTutorialOpen = false;

    private Vector2 _specificPageRectVector =Vector2.zero;
    private float _specificPageButtonStartX;

    private bool _hasLastPageBeenVisited = false;
    
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
        SetArrowTransforms();
        SetPageButtonInteractability();
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
    }

    private void SetPageButtonInteractability()
    {
        _leftPageButton.interactable = (_currentPageID != 0);
        _rightPageButton.interactable = (_currentPageID < _totalPages-1);
    }

    public void NewPageStartDisplay()
    {
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
    
    #endregion
    
    #region SpecificPageButton

    private void CreateSpecificPageButtons()
    {
        _totalPages = _defaultAmountOfPages;
        _specificPageButtons = new SpecificTutorialPageButton[_totalPages];
        
        _specificPageButtonStartX = ((float)_totalPages)/2;
        _specificPageButtonStartX *= -_spaceBetweenSpecificPageButtons;

        _specificPageButtonStartX += _spaceBetweenSpecificPageButtons/2;
        
        for (int i = 0; i < _totalPages; i++)
        {
            CreateSpecificPageButton(i);
            Debug.Log("CreatedSpecificPage");
        }
    }

    private void CreateSpecificPageButton(int pageID)
    {
        SpecificTutorialPageButton pageButton = Instantiate(_specificPageButton, _specificPageButtonHolder.transform)
            .GetComponent<SpecificTutorialPageButton>();

        _specificPageButtons[pageID] = pageButton;
        
        //pageButton.SetUpSpecificPageButton(this, pageID);
        
        _specificPageRectVector.Set((_specificPageButtonStartX)+(pageID * _spaceBetweenSpecificPageButtons), 0);

        if (pageButton.IsUnityNull())
        {
            Debug.Log("Null");
        }
        pageButton.SetTutorialPageTransform(_specificPageRectVector);
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
    
    
    private void SubscribeToPlayerInput()
    {
        if (_isSubscribedToInput)
        {
            return;
        }
        
        _universalPlayerInputActions = new UniversalPlayerInputActions();
        _universalPlayerInputActions.GameplayActions.Enable();
        
        _universalPlayerInputActions.GameplayActions.NumberPress.started += PageNumberPressed;

        _isSubscribedToInput = true;
    }

    private void UnsubscribeToPlayerInput()
    {
        if (!_isSubscribedToInput)
        {
            return;
        }
        
        _universalPlayerInputActions.GameplayActions.NumberPress.started -= PageNumberPressed;
        
        _universalPlayerInputActions.Disable();

        _isSubscribedToInput = false;
    }
}
