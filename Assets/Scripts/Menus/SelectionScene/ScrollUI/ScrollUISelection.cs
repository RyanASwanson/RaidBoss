using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrollUISelection : MonoBehaviour
{
    [SerializeField] private CurveProgression _appearingCurve;
    [SerializeField] private CurveProgression _scrollingCurve;
    
    [SerializeField] private float _unscrollTime;
    [SerializeField] private float _scrollTime;
    [SerializeField] private float _scrollDelayTime;
    [SerializeField] private AnimationCurve _scrollUnscrollCurve;
    private WaitForSeconds _scrollDelay;
    
    private float _unscrollProgress = 0;
    private float _unscrollCurveValue = 0;

    private float _targetScrollSize = 0;
    private float _currentScrollSize = 0;

    private float _scrollTopStartingY;
    private float _scrollLowerStartingY;
    
    private EScrollState _scrollState = EScrollState.NotVisible;

    private bool _isTargetScrollSizeBuffered = false;
    private bool _isScrollOpenBuffered = false;
    private bool _isScrollDisappearBuffered = false;
    
    private Coroutine _scrollOpenCloseCoroutine;
    private Coroutine _scrollCloseDelayCoroutine;
    
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
    [SerializeField] private RectTransform _scrollTop;
    [SerializeField] private RectTransform _scrollUpper;
    [SerializeField] private RectTransform _scrollMiddle;
    [SerializeField] private RectTransform _scrollLower;
    [SerializeField] private RectTransform _scrollBottom;

    [Space] 
    [SerializeField] private Animator _scrollAnimator;

    private static string SHOW_SCROLL_ANIM_BOOL = "ShowScroll";
    
    // Start is called before the first frame update
    void Start()
    {
        _scrollDelay = new WaitForSeconds(_scrollDelayTime);
        
        _scrollTopStartingY = _scrollTop.localPosition.y;
        _scrollLowerStartingY = _scrollLower.localPosition.y;
        
        _middleScrollStartingScale = _scrollMiddle.localScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            OpenScroll();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            CloseScroll();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetNewScrollSize(60);
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            SetNewScrollSize(90);
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            CloseScroll();
        }
        
        //SetUpperTransform();
        //SetMiddleTransform();
    }

    #region AppearDisappear
    public void ScrollAppear()
    {
        _isScrollOpenBuffered = true;
        ShowScrollAnimation(true);
        Debug.LogWarning("ScrollAppear");
    }

    public void ScrollFullyAppeared()
    {
        if (_isScrollOpenBuffered)
        {
            OpenScroll();
        }
    }

    public void ScrollDisappear()
    {
        Debug.Log("Scroll dissapear " + _scrollState);
        if (_scrollState == EScrollState.Closed)
        {
            ShowScrollAnimation(false);
        }
        else
        {
            _isScrollDisappearBuffered = true;
        }
        
        //CloseScroll();
        StopCloseScrollDelay();
        _scrollCloseDelayCoroutine = StartCoroutine(CloseScrollDelay());
    }

    public void ScrollFullyDisappeared()
    {
        _scrollState = EScrollState.NotVisible;
    }
    
    private IEnumerator CloseScrollDelay()
    {
        yield return _scrollDelay;
        Debug.Log("Scroll Delay Passed");
        CloseScroll();
    }
    
    private void StopCloseScrollDelay()
    {
        if (!_scrollCloseDelayCoroutine.IsUnityNull())
        {
            StopCoroutine(_scrollCloseDelayCoroutine);
        }
    }
    #endregion
    
    #region ScrollUnscrolling

    public void SetNewScrollSize(float unscrollSize)
    {
        _targetScrollSize = unscrollSize;

        StopCloseScrollDelay();
        
        Debug.LogWarning(_scrollState);

        switch (_scrollState)
        {
            case EScrollState.Closing:
            case EScrollState.NotVisible:
                ScrollAppear();
                break;
            case EScrollState.Closed:
                OpenScroll();
                break;
            case EScrollState.Opening:
            case EScrollState.FullyOpen:
                CloseScroll();
                _isTargetScrollSizeBuffered = true;
                _isScrollDisappearBuffered = false;
                break;
            default:
                break;
        }

        
    }

    public void OpenScroll()
    {
        StopCurrentScrollProgress();
        
        _currentScrollSize = _targetScrollSize;
        _scrollOpenCloseCoroutine = StartCoroutine(ScrollOpenProcess());
    }
    
    public void CloseScroll()
    {
        //Debug.Log("Close Scroll");
        _isScrollOpenBuffered = false;
        StopCurrentScrollProgress();
        
        _scrollOpenCloseCoroutine = StartCoroutine(ScrollCloseProcess());
    }
    
    public void StopCurrentScrollProgress()
    {
        if (!_scrollOpenCloseCoroutine.IsUnityNull())
        {
            StopCoroutine(_scrollOpenCloseCoroutine);
        }
    }
    
    private IEnumerator ScrollOpenProcess()
    {
        _scrollState = EScrollState.Opening;
        while (_unscrollProgress < 1)
        {
            _unscrollProgress += Time.deltaTime/_unscrollTime;
            //Debug.Log(_unscrollProgress);
            
            UpdateScrollProcess();
            
            yield return null;
        }

        ScrollFullyOpened();
    }

    private void ScrollFullyOpened()
    {
        _unscrollProgress = 1;
        _scrollState = EScrollState.FullyOpen;
        UpdateScrollProcess();
    }
    
    private IEnumerator ScrollCloseProcess()
    {
        _scrollState = EScrollState.Closing;
        while (_unscrollProgress > 0)
        {
            _unscrollProgress -= Time.deltaTime/_scrollTime;
            
            UpdateScrollProcess();
            
            yield return null;
        }

        ScrollFullyClosed();
    }

    private void ScrollFullyClosed()
    {
        _unscrollProgress = 0;
        _scrollState = EScrollState.Closed;
        UpdateScrollProcess();

        Debug.Log("Scroll fully closed with " + _isTargetScrollSizeBuffered + " " + _isScrollDisappearBuffered);

        if (_isTargetScrollSizeBuffered)
        {
            _isTargetScrollSizeBuffered = false;
            OpenScroll();
        }
        if (_isScrollDisappearBuffered)
        {
            _isScrollDisappearBuffered = false;
            ShowScrollAnimation(false);
        }
    }

    private void UpdateScrollProcess()
    {
        _unscrollCurveValue = _scrollUnscrollCurve.Evaluate(_unscrollProgress);
        //Debug.Log("Curve value " +_unscrollCurveValue);

        _scrollTop.localPosition = new Vector3(0, Mathf.Lerp(_scrollTopStartingY,_scrollTopStartingY+_currentScrollSize,_unscrollCurveValue), 0);
        _scrollLower.localPosition = new Vector3(0, Mathf.Lerp(_scrollLowerStartingY,_scrollLowerStartingY-_currentScrollSize,_unscrollCurveValue), 0);
        
        SetUpperTransform();
        SetMiddleTransform();
    }

    private void SetUpperTransform()
    {
        _scrollUpper.localPosition =
            new Vector3(0, _scrollTop.transform.localPosition.y - _upperScrollDistanceFromTop, 0);
        //Debug.Log(_scrollTop.transform.localPosition.y +"     " + _upperScrollDistanceFromTop);
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


    private void ShowScrollAnimation(bool show)
    {
        //_scrollAnimator.SetBool(SHOW_SCROLL_ANIM_BOOL, show);
    }
}

public enum EScrollState
{
    NotVisible,
    Appearing,
    Closed,
    Opening,
    FullyOpen,
    Closing,
    Disappearing
};
