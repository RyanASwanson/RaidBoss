using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GeneralScale : MonoBehaviour
{
    [SerializeField] private bool _doesBeginScalingOnEnable;
    [SerializeField] private bool _doesResetScaleOnScalingEnable;
    
    private Coroutine _scalingCoroutine;
    
    [Space]
    [SerializeField] private float _scalingTime;

    [Space] 
    [SerializeField] private bool _hasOverrideStartingScale;
    [SerializeField] private Vector3 _overrideStartingScale;
    [SerializeField] private Vector3 _endingScale;
    
    [Space]
    [Header("Scaling with curve progression")]
    [SerializeField] private CurveProgression _curveProgression;
    
    private Vector3 _startingScale;
    

    void OnEnable()
    {
        SubscribeToEvents();

        if (_hasOverrideStartingScale)
        {
            transform.localScale = _overrideStartingScale;
        }
        _startingScale = transform.localScale;
        
        if (_doesBeginScalingOnEnable)
        {
            BeginScaling();
        }
    }

    public void SetUp()
    {
        // Delayed functionality that waits for curve set up
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    public void BeginScaling()
    {
        StopScaling();

        if (_doesResetScaleOnScalingEnable)
        {
            ResetScale();
        }
        
        _scalingCoroutine = StartCoroutine(ScalingProcess());
    }

    public void StopScaling()
    {
        if (!_scalingCoroutine.IsUnityNull())
        {
            StopCoroutine(_scalingCoroutine);
            _scalingCoroutine = null;
        }
    }

    public void ResetScale()
    {
        transform.localScale = _startingScale;
    }

    private IEnumerator ScalingProcess()
    {
        float scaleProgress = 0;
        while (!gameObject.IsUnityNull() && scaleProgress < 1)
        {
            scaleProgress += Time.deltaTime * _scalingTime;
            
            transform.localScale = Vector3.Lerp(_startingScale, _endingScale, scaleProgress);
            yield return null;
        }
    }

    public void UpdateLocalScale(float scaleProgress)
    {
        transform.localScale = Vector3.Lerp(_startingScale, _endingScale, scaleProgress);
    }

    public void SubscribeToEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnSetUpComplete.AddListener(SetUp);
            _curveProgression.OnCurveValueChanged.AddListener(UpdateLocalScale);
        }
    }

    public void UnsubscribeFromEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnSetUpComplete.RemoveListener(SetUp);
            _curveProgression.OnCurveValueChanged.RemoveListener(UpdateLocalScale);
        }
    }
}
