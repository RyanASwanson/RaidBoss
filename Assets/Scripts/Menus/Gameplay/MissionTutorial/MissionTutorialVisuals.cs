using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionTutorialVisuals : MonoBehaviour
{
    [SerializeField] private GeneralScrollPopUp _scrollPopUp;
    [SerializeField] private ScrollMissionTutorialContent _scrollMissionTutorialContent;
    
    [Space] 
    [Header("Page Navigation")]
    [SerializeField] private RectTransform _leftPageHolder;
    [SerializeField] private RectTransform _rightPageHolder;
    
    [SerializeField] private Button _leftPageButton;
    [SerializeField] private Button _rightPageButton;

    [Space] 
    [SerializeField] private float _spaceBetweenSpecificPageButtons;
    
    [Space]
    [SerializeField] private GameObject _specificPageButton;
    private SpecificTutorialPageButton[] _specificPageButtons;

    [Space] 
    [SerializeField] private GameObject _specificPageButtonHolder;

    [Space] 
    [SerializeField] private CurveProgression _playButtonScaleCurve;
    [SerializeField] private SelectionPlayButton _playButton;
    
    private int _currentPageID = 0;
    private int _previousPageID = 0;

    private int _totalPages = 0;

    private Vector2 _specificPageRectVector =Vector2.zero;
    private float _specificPageButtonStartX;

    private bool _hasLastPageBeenVisited = false;
    
    public void SetUpMissionTutorials()
    {
        SubscribeToEvents();
        CreateMissionTutorials();
        CreateSpecificPageButtons();
        SetArrowTransforms();
        SetPageButtonInteractability();
        SetUpPlayButton();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    private void CreateMissionTutorials()
    {
        if (!SelectionManager.Instance.IsPlayingMissionsMode())
        {
            gameObject.SetActive(false);
            return;
        }

        if (SelectionManager.Instance.GetSelectedMissionOut(out MissionSO mission))
        {
            _totalPages = mission.GetTutorialPages().Length;
        }

        _scrollMissionTutorialContent.CreatePages();
        
        
        _scrollPopUp.ShowScroll();
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
        
        _scrollPopUp.HideScroll();
        _scrollPopUp.ShowScroll();
    }

    private void SetPageButtonInteractability()
    {
        _leftPageButton.interactable = (_currentPageID != 0);
        _rightPageButton.interactable = (_currentPageID < _totalPages-1);
    }

    public void NewPageStartDisplay()
    {
        UpdatePreviousPageID();
        
        // If the last page has not been visited and we just opened the last page
        if (!_hasLastPageBeenVisited && _currentPageID == _totalPages - 1)
        {
            _hasLastPageBeenVisited = true;
            _playButton.MaxCharactersSelected(true);
        }
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
    
    #region SpecificPageButton

    private void CreateSpecificPageButtons()
    {
        _specificPageButtons = new SpecificTutorialPageButton[_totalPages];
        
        _specificPageButtonStartX = ((float)_totalPages)/2;
        _specificPageButtonStartX *= -_spaceBetweenSpecificPageButtons;

        _specificPageButtonStartX += _spaceBetweenSpecificPageButtons/2;
        
        for (int i = 0; i < _totalPages; i++)
        {
            CreateSpecificPageButton(i);
        }
    }

    private void CreateSpecificPageButton(int pageID)
    {
        SpecificTutorialPageButton pageButton = Instantiate(_specificPageButton, _specificPageButtonHolder.transform)
            .GetComponent<SpecificTutorialPageButton>();

        _specificPageButtons[pageID] = pageButton;
        
        pageButton.SetUpSpecificPageButton(this, pageID);
        
        _specificPageRectVector.Set((_specificPageButtonStartX)+(pageID * _spaceBetweenSpecificPageButtons), 0);
        
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
    #endregion
    
    #region PlayButton

    private void SetUpPlayButton()
    {
        _playButton.SetUpPlayButton();
        _playButton.ToggleInteractability(false);
    }
    #endregion
    
    #endregion

    // Function called by button
    public void CloseTutorial()
    {
        _scrollPopUp.HideScroll();
        _playButtonScaleCurve.StartMovingDownOnCurve();
        GameStateManager.Instance.StartProgressToStart();
    }
    
    

    private void SubscribeToEvents()
    {
    }

    private void UnsubscribeFromEvents()
    {
    }

    #region Getters

    public int GetCurrentPageID() => _currentPageID;
    public int GetPreviousPageID() => _previousPageID;

    #endregion
    
    #region Setters

    public void SetCurrentPageID(int pageID)
    {
        _currentPageID = pageID;
    }

    public void SetPreviousPageID(int pageID)
    {
        _previousPageID = pageID;
    }
    #endregion
    
}
