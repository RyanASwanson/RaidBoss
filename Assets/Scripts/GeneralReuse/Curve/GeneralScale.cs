using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GeneralScale : MonoBehaviour
{
    [SerializeField] private bool _hasOverrideStartingScale;
    [SerializeField] private Vector3 _overrideStartingScale;
    [SerializeField] private Vector3 _endingScale;
    
    [Space]
    [Header("Scaling with curve progression")]
    [SerializeField] private CurveProgression _curveProgression;
    
    private Vector3 _startingScale;
    

    void OnEnable()
    {
        if (_hasOverrideStartingScale)
        {
            transform.localScale = _overrideStartingScale;
        }
        _startingScale = transform.localScale;
        
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    public void UpdateLocalScale(float scaleProgress)
    {
        transform.localScale = Vector3.LerpUnclamped(_startingScale, _endingScale, scaleProgress);
    }

    public void SubscribeToEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.AddListener(UpdateLocalScale);
        }
    }

    public void UnsubscribeFromEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.RemoveListener(UpdateLocalScale);
        }
    }

    #region Setters

    public void MultiplyEndingScale(float multiplier) => _endingScale *= multiplier;

    #endregion
}
