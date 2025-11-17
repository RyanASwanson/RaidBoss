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
    
    private Coroutine _curveProgressCoroutine;

    [Space] 
    [SerializeField] private bool _doesAutomaticallyMoveDownOnHittingMax;
    
    [Space] 
    [SerializeField] private bool _hasDecreaseDelay;
    [SerializeField] private float _decreaseDelay;
    private WaitForSeconds _decreaseWait;
    
    private Coroutine _decreaseWaitCoroutine;
    
    [Space]
    [SerializeField] private AnimationCurve _curve;

    [Space] 
    [SerializeField] private UnityEvent _onStartedIncreasing;
    [SerializeField] private UnityEvent _onStartedDecreasing;
    
    [SerializeField] private UnityEvent _onMaxValueReached;
    [SerializeField] private UnityEvent _onMinValueReached;
    //Curve Value Changed event is kept from being editor accessible as it defaults the value invoked to 0
    internal UnityEvent<float> _onCurveValueChanged = new UnityEvent<float>();


    private void Start()
    {
        if (_hasDecreaseDelay)
        {
            _decreaseWait = new WaitForSeconds(_decreaseDelay);
        }
    }

    public void StartMovingUpOnCurve()
    {
        if (_hasDecreaseDelay)
        {
            StopMoveDownDelay();
        }

        StopMovingOnCurve();
        InvokeOnStartedIncreasing();
        _curveProgressCoroutine = StartCoroutine(MovingUpOnCurveProgress());
    }

    public void StartMovingDownOnCurve()
    {
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
        _curveProgressCoroutine = StartCoroutine(MovingDownOnCurveProgress());
    }

    public void StopMovingOnCurve()
    {
        if (!_curveProgressCoroutine.IsUnityNull())
        {
            StopCoroutine(_curveProgressCoroutine);
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

    private void StartMoveDownDelay()
    {
        StopMoveDownDelay();

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

    public void InvokeOnCurveValueChanged()
    {
        //Debug.Log("Curve value " + CurveValue);
        _onCurveValueChanged?.Invoke(CurveValue);
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
}

public enum ECurveStatus
{
    AtMinValue,
    AtMaxValue,
    Increasing,
    Decreasing,
}
