using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CurveProgression : MonoBehaviour
{
    [SerializeField] private string _curveName;
    
    [Space]
    [SerializeField] private float _curveIncreaseTime;
    [SerializeField] private float _curveDecreaseTime;
    
    private float _movementProgress = 0;
    
    private float _curveEvaluated = 0;
    internal ECurveStatus CurveStatus = ECurveStatus.AtMinValue;
    
    [SerializeField] private float _minCurveValue;
    [SerializeField] private float _maxCurveValue;
    internal float CurveValue = 0;
    
    private Coroutine _curveIncreaseProgressCoroutine;
    private Coroutine _curveDecreaseProgressCoroutine;

    [Space] 
    [SerializeField] private bool _hasDefaultValue;
    [SerializeField] private float _defaultValue;

    [Space] 
    [SerializeField] private bool _doesResetToDefaultProgressOnEnable;
    
    [Space] 
    [SerializeField] private bool _doesAutomaticallyMoveDownOnHittingMax;

    [Space] 
    [SerializeField] private bool _hasIncreaseDelay;
    [SerializeField] private float _increaseDelay;
    private WaitForSeconds _increaseWait;
    
    private Coroutine _increaseWaitCoroutine;
    
    [SerializeField] private bool _hasDecreaseDelay;
    [SerializeField] private float _decreaseDelay;
    private WaitForSeconds _decreaseWait;
    
    private Coroutine _decreaseWaitCoroutine;
    
    [Space]
    [SerializeField] private AnimationCurve _curve;

    [Space] 
    public UnityEvent OnSetUpComplete;
    
    [SerializeField] private UnityEvent _onStartedIncreasing;
    [SerializeField] private UnityEvent _onStartedDecreasing;
    
    [SerializeField] private UnityEvent _onMaxValueReached;
    [SerializeField] private UnityEvent _onMinValueReached;
    //Curve Value Changed event is kept from being editor accessible as it defaults the value invoked to 0
    internal UnityEvent<float> OnCurveValueChanged = new UnityEvent<float>();


    private void Start()
    {
        if (_hasDefaultValue)
        {
            _movementProgress = _defaultValue;
            UpdateCurveProgress();
        }
        
        InvokeOnSetUpComplete();
    }

    private void OnEnable()
    {
        if (_hasIncreaseDelay)
        {
            _increaseWait = new WaitForSeconds(_increaseDelay);
        }
        
        if (_hasDecreaseDelay)
        {
            _decreaseWait = new WaitForSeconds(_decreaseDelay);
        }
        
        if (_doesResetToDefaultProgressOnEnable)
        {
            ResetCurve();
        }
    }

    public void ResetCurve()
    {
        StopMovingOnCurve();
        _movementProgress = _defaultValue;
        UpdateCurveProgress();
    }

    public void StartMovingOppositeDirectionOnCurve()
    {
        if (IsOppositeDirectionUpOnCurve())
        {
            StartMovingUpOnCurve();
        }
        else
        {
            StartMovingDownOnCurve();
        }
    }

    public void StartMovingUpOnCurve()
    {
        if (_hasDecreaseDelay)
        {
            StopMoveDownDelay();
        }
        if (_hasIncreaseDelay)
        {
            StartMoveUpDelay();
            return;
        }

        BeginMovingUpOnCurveOverride();
    }

    private void BeginMovingUpOnCurveOverride()
    {
        StopMovingOnCurve();
        InvokeOnStartedIncreasing();
        _curveIncreaseProgressCoroutine = StartCoroutine(MovingUpOnCurveProgress());
    }

    public void StartMovingDownOnCurve()
    {
        if (_hasIncreaseDelay)
        {
            StopMoveUpDelay();
        }
        if (_hasDecreaseDelay)
        {
            StartMoveDownDelay();
            return;
        }
        
        BeginMovingDownOnCurveOverride();
    }

    private void BeginMovingDownOnCurveOverride()
    {
        StopMovingOnCurve();
        InvokeOnStartedDecreasing();
        _curveDecreaseProgressCoroutine = StartCoroutine(MovingDownOnCurveProgress());
    }

    public void StopMovingOnCurve()
    {
        // Using 2 coroutine variables as it fixed an issue of it getting stuck moving both up and down the curve at the same time
        if (!_curveIncreaseProgressCoroutine.IsUnityNull())
        {
            StopCoroutine(_curveIncreaseProgressCoroutine);
            _curveIncreaseProgressCoroutine = null;
        }

        if (!_curveDecreaseProgressCoroutine.IsUnityNull())
        {
            StopCoroutine(_curveDecreaseProgressCoroutine);
            _curveDecreaseProgressCoroutine = null;
        }
    }
    
    private IEnumerator MovingUpOnCurveProgress()
    {
        CurveStatus = ECurveStatus.Increasing;
        while (_movementProgress < 1)
        {
            _movementProgress += Time.deltaTime / _curveIncreaseTime;
            UpdateCurveProgress();
            yield return null;
        }
        CurveReachedMaxValue();
    }
    
    private IEnumerator MovingDownOnCurveProgress()
    {
        CurveStatus = ECurveStatus.Decreasing;
        while (_movementProgress > 0)
        {
            _movementProgress -= Time.deltaTime / _curveDecreaseTime;
            UpdateCurveProgress();
            yield return null;
        }
        CurveReachedMinValue();
    }

    private void UpdateCurveProgress()
    {
        _curveEvaluated = _curve.Evaluate(_movementProgress);
        CurveValue = Mathf.Lerp(_minCurveValue,_maxCurveValue, _curveEvaluated);
        InvokeOnCurveValueChanged();
    }

    #region MoveDelay

    private void StartMoveUpDelay()
    {
        StopMoveUpAndDownDelay();
        
        _increaseWaitCoroutine = StartCoroutine(MoveUpDelayProcess());
    }

    private void StopMoveUpDelay()
    {
        if (!_increaseWaitCoroutine.IsUnityNull())
        {
            StopCoroutine(_increaseWaitCoroutine);
            _increaseWaitCoroutine = null;
        }
    }
    
    private IEnumerator MoveUpDelayProcess()
    {
        yield return _increaseWait;
        BeginMovingUpOnCurveOverride();
    }
    
    private void StartMoveDownDelay()
    {
        StopMoveUpAndDownDelay();

        _decreaseWaitCoroutine = StartCoroutine(MoveDownDelayProcess());
    }

    private void StopMoveDownDelay()
    {
        if (!_decreaseWaitCoroutine.IsUnityNull())
        {
            StopCoroutine(_decreaseWaitCoroutine);
            _decreaseWaitCoroutine = null;
        }
    }

    private IEnumerator MoveDownDelayProcess()
    {
        yield return _decreaseWait;
        BeginMovingDownOnCurveOverride();
    }

    private void StopMoveUpAndDownDelay()
    {
        StopMoveUpDelay();
        StopMoveDownDelay();
    }
    #endregion

    private void CurveReachedMaxValue()
    {
        CurveStatus = ECurveStatus.AtMaxValue;
        UpdateCurveProgress();
        InvokeOnMaxValueReached();

        if (_doesAutomaticallyMoveDownOnHittingMax)
        {
            StartMovingDownOnCurve();
        }
    }

    private void CurveReachedMinValue()
    {
        CurveStatus = ECurveStatus.AtMinValue;
        UpdateCurveProgress();
        InvokeOnMinValueReached();
    }

    public void InvokeOnSetUpComplete()
    {
        OnSetUpComplete.Invoke();
    }

    public void InvokeOnCurveValueChanged()
    {
        //Debug.Log("Curve value " + CurveValue);
        OnCurveValueChanged?.Invoke(CurveValue);
    }

    public void InvokeOnStartedIncreasing()
    {
        _onStartedIncreasing?.Invoke();
    }

    public void InvokeOnStartedDecreasing()
    {
        _onStartedDecreasing?.Invoke();
    }
    
    public void InvokeOnMaxValueReached()
    {
        _onMaxValueReached?.Invoke();
    }
    
    public void InvokeOnMinValueReached()
    {
        _onMinValueReached?.Invoke();
    }

    public bool IsOppositeDirectionUpOnCurve()
    {
        return CurveStatus == ECurveStatus.Decreasing || CurveStatus == ECurveStatus.AtMinValue;
    }
}

public enum ECurveStatus
{
    AtMinValue,
    AtMaxValue,
    Increasing,
    Decreasing,
}
