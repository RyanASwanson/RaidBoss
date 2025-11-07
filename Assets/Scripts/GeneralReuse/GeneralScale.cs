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
    [SerializeField] private Vector3 _endingScale;
    
    private Vector3 _startingScale;

    void OnEnable()
    {
        _startingScale = transform.localScale;
        if (_doesBeginScalingOnEnable)
        {
            BeginScaling();
        }
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
}
