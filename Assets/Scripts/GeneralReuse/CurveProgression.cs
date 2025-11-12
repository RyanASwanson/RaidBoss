using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CurveProgression : MonoBehaviour
{
    [SerializeField] private float _curveIncreaseTime;
    [SerializeField] private float _curveDecreaseSpeed;
    
    private float _movementProgress = 0;
    
    internal float CurveValue = 0;
    internal ECurveStatus CurveStatus = ECurveStatus.AtMinValue;
    
    private Coroutine _curveProgressCoroutine;
    
    [Space]
    [SerializeField] private AnimationCurve _curve;

    [Space] 
    [SerializeField] private UnityEvent<float> _onCurveValueChanged;
    [SerializeField] private UnityEvent _onMaxValueReached;
    [SerializeField] private UnityEvent _onMinValueReached;
    
    

    public void StartMovingUpOnCurve()
    {
        StopMovingOnCurve();
        _curveProgressCoroutine = StartCoroutine(MovingUpOnCurveProgress());
    }

    public void StartMovingDownOnCurve()
    {
        StopMovingOnCurve();
        _curveProgressCoroutine = StartCoroutine(MovingDownOnCurveProgress());
    }

    private void StopMovingOnCurve()
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
            _movementProgress += Time.deltaTime * _curveIncreaseTime;
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
            _movementProgress -= Time.deltaTime * _curveIncreaseTime;
            UpdateCurveProgress();
            yield return null;
        }
        CurveReachedMinValue();
    }

    private void UpdateCurveProgress()
    {
        CurveValue = _curve.Evaluate(_movementProgress);
        InvokeOnCurveValueChanged();
    }

    private void CurveReachedMaxValue()
    {
        CurveStatus = ECurveStatus.AtMaxValue;
        UpdateCurveProgress();
        InvokeOnMaxValueReached();
    }

    private void CurveReachedMinValue()
    {
        CurveStatus = ECurveStatus.AtMinValue;
        UpdateCurveProgress();
        InvokeOnMinValueReached();
    }

    public void InvokeOnCurveValueChanged()
    {
        _onCurveValueChanged?.Invoke(CurveValue);
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
