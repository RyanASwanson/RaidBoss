using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ScrollUISelection : MonoBehaviour
{
    [SerializeField] private CurveProgression _appearingCurve;
    [SerializeField] private CurveProgression _scrollingCurve;
    [SerializeField] private CurveProgression _contentsCurve;

    [Space] 
    [SerializeField] private ScrollUIContents _scrollUIContents;
    
    private float _targetScrollSize = 0;
    private float _currentScrollSize = 0;

    private float _scrollTopStartingY;
    private float _scrollLowerStartingY;

    private bool _isBufferingNewScrollOpen = false;
    
    private Coroutine _scrollCloseDelayCoroutine;

    [Space] 
    [SerializeField] private float _scrollOpenSizeLineMultiplier;
    [SerializeField] private float _flatScrollOpenSize;
    
    [Space] 
    [SerializeField] private float _upperScrollDistanceFromTop;
    
    [Space]
    [SerializeField] private float _middleScrollScaleMultiplier;
    [SerializeField] private RectTransform _middleScrollUpperBounds;
    [SerializeField] private RectTransform _middleScrollLowerBounds;
    private Vector3 _middleScrollStartingScale;
    
    [Space]
    [SerializeField] private RectTransform _scrollHolder;
    [Space] 
    [SerializeField] private RectTransform _contents;
    [Space]
    [SerializeField] private RectTransform _scrollTop;
    [SerializeField] private RectTransform _scrollUpper;
    [SerializeField] private RectTransform _scrollMiddle;
    [SerializeField] private RectTransform _scrollLower;
    [SerializeField] private RectTransform _scrollBottom;

    [Space] 
    [SerializeField] private UnityEvent _onScrollOpening;
    
    // Start is called before the first frame update
    void Start()
    {
        _scrollTopStartingY = _scrollTop.localPosition.y;
        _scrollLowerStartingY = _scrollLower.localPosition.y;
        
        _middleScrollStartingScale = _scrollMiddle.localScale;

        SubscribeToEvents();
        
        _scrollUIContents.SetUpContents(this);
    }

    public void ShowNewScroll(float unscrollSize)
    {
        _targetScrollSize = unscrollSize;
        
        if (_appearingCurve.CurveStatus == ECurveStatus.AtMinValue ||
            _appearingCurve.CurveStatus == ECurveStatus.Decreasing)
        {
            StartScrollAppear();
        }
        else if (_scrollingCurve.CurveStatus == ECurveStatus.Decreasing)
        {
            _isBufferingNewScrollOpen = true;
        }
    }

    public void HideScroll()
    {
        if (_appearingCurve.CurveStatus == ECurveStatus.Increasing)
        {
            _scrollingCurve.StopMovingOnCurve();
            StartScrollDisappear();
        }
        else if (_appearingCurve.CurveStatus == ECurveStatus.AtMaxValue &&
                 _scrollingCurve.CurveStatus == ECurveStatus.AtMinValue)
        {
            _scrollingCurve.StopMovingOnCurve();
            StartScrollDisappear();
        }
        else if (_scrollingCurve.CurveStatus == ECurveStatus.Increasing || _scrollingCurve.CurveStatus == ECurveStatus.AtMaxValue)
        {
            StartScrollClose();
        }

        _isBufferingNewScrollOpen = false;

    }

    #region AppearDisappear

    private void StartScrollAppear()
    {
        _appearingCurve.StartMovingUpOnCurve();
    }

    private void StartScrollDisappear()
    {
        _appearingCurve.StartMovingDownOnCurve();
    }
    
    public void ScrollFullyAppeared()
    {
        StartScrollOpen();
    }
    
    public void ScrollFullyDisappeared()
    {
        
    }

    public void UpdateAppearingScale(float scale)
    {
        _scrollHolder.transform.localScale = new Vector3(scale, scale, scale);
    }
    #endregion
    
    #region ScrollUnscrolling

    public void StartScrollOpen()
    {
        InvokeOnScrollOpening();
        if (_scrollUIContents)
        {
            _scrollUIContents.UpdateContentsAndCountLines();
            
            _targetScrollSize = (_scrollOpenSizeLineMultiplier * _scrollUIContents.LineLength) + _flatScrollOpenSize;
            //Debug.Log("Target" + _targetScrollSize);
        }
        
        
        _scrollingCurve.StartMovingUpOnCurve();
        _contentsCurve.StartMovingUpOnCurve();
        _currentScrollSize = _targetScrollSize;

        PlayScrollOpenAudio();
    }

    public void StartScrollClose()
    {
        _scrollingCurve.StartMovingDownOnCurve();
        _contentsCurve.StartMovingDownOnCurve();
    }
    
    public void ScrollFullyOpened()
    {
        
    }
    

    public void ScrollFullyClosed()
    {
        if (_isBufferingNewScrollOpen)
        {
            _isBufferingNewScrollOpen = false;
            
            StartScrollOpen();
        }
        else
        {
            StartScrollDisappear();
        }
    }

    public void UpdateScrollProgress(float progress)
    {
        _scrollTop.localPosition = new Vector3(0, Mathf.Lerp(_scrollTopStartingY,_scrollTopStartingY+_currentScrollSize,progress), 0);
        _scrollLower.localPosition = new Vector3(0, Mathf.Lerp(_scrollLowerStartingY,_scrollLowerStartingY-_currentScrollSize,progress), 0);
        
        SetUpperTransform();
        SetMiddleTransform();
    }

    private void SetUpperTransform()
    {
        _scrollUpper.localPosition =
            new Vector3(0, _scrollTop.transform.localPosition.y - _upperScrollDistanceFromTop, 0);
    }

    private void SetMiddleTransform()
    {
        //_middleScrollUpperBounds.anchoredPosition.y;
        
        float middleY = ( _scrollLower.localPosition.y + _scrollUpper.localPosition.y) / 2;
        //float middleY = ( _middleScrollLowerBounds.localPosition.y + _middleScrollUpperBounds.localPosition.y) / 2;
        _scrollMiddle.localPosition  = new Vector3(_scrollMiddle.localPosition.x, middleY ,_scrollMiddle.localPosition.z);
        
        float middleScale = (_scrollUpper.localPosition.y - _scrollLower.localPosition.y) * _middleScrollScaleMultiplier;
        //float middleScale = (_middleScrollUpperBounds.localPosition.y - _middleScrollLowerBounds.localPosition.y) * _middleScrollScaleMultiplier;
        _scrollMiddle.localScale = new Vector3(_middleScrollStartingScale.x, middleScale, _middleScrollStartingScale.z);
    }
    
    #endregion
    
    #region Contents

    private void SetContentsScale(float scale)
    {
        _contents.position = _scrollMiddle.position;
        //Previously this line was used so the contents game object could be adjusted on the x
        //_contents.position = new Vector3(_contents.position.x,_scrollMiddle.position.y,_contents.position.z);
        //_contents.position = new Vector3(_scrollMiddle.position.x,_scrollMiddle.position.y,_contents.position.z);
        _contents.transform.localScale = new Vector3(scale, scale, scale);   
    }
    #endregion

    #region Audio
    private void PlayScrollOpenAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(AudioManager.Instance.UserInterfaceAudio
            .SelectionSceneUserInterfaceAudio.ScrollOpenStart);
    }
    
    #endregion
    
    private void SubscribeToEvents()
    {
        _appearingCurve.OnCurveValueChanged.AddListener(UpdateAppearingScale);
        _scrollingCurve.OnCurveValueChanged.AddListener(UpdateScrollProgress);
        _contentsCurve.OnCurveValueChanged.AddListener(SetContentsScale);
    }
    
    #region Events

    public void InvokeOnScrollOpening()
    {
        _onScrollOpening?.Invoke();
    }
    
    #endregion
}

