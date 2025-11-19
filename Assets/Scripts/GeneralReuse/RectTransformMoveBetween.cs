using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTransformMoveBetween : MonoBehaviour
{
    [SerializeField] private bool _isUsingRelativePosition;
    [SerializeField] private Vector3 _relativeTargetPosition;
    [SerializeField] private Vector3 _targetPosition;
    private Vector3 _startPosition;
    
    [Space]
    [Header("Move with curve progression")]
    [SerializeField] private CurveProgression _curveProgression;
    
    private RectTransform _rectTransform;
    

    private void OnEnable()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.localPosition;
        
        if (_isUsingRelativePosition)
        {
            _targetPosition = _startPosition + _relativeTargetPosition;
        }
        
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    public void UpdateLocalPosition(float scaleProgress)
    {
        _rectTransform.localPosition = Vector3.Lerp(_startPosition, _targetPosition, scaleProgress);
    }

    public void SubscribeToEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.AddListener(UpdateLocalPosition);
        }
    }

    public void UnsubscribeFromEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.RemoveListener(UpdateLocalPosition);
        }
    }
}
