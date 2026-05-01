using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeneralRotation : MonoBehaviour
{
    [SerializeField] private bool _doesBeginRotationOnEnable;
    [SerializeField] private bool _doesResetRotationOnRotationEnable;
    [SerializeField] private bool _doesResetLocalRotationOnRotationEnable = false;
    [SerializeField] private bool _doesResetRotationToDefaultOnEnable= false;

    [SerializeField] private bool _doesUseLocalEulerAngles = false;
    
    private Coroutine _rotationCoroutine;
    
    [Space]
    [SerializeField] private Vector3 _rotationPerSecond;
    [SerializeField] private CurveProgression _rotationAccelerationCurve;
    private float _rotationsPerSecondAcceleration = 1;
    
    [Space]
    [SerializeField] private bool _doesUseRotateFunction = false;
    [SerializeField] private float _rotateFunctionRotationsPerSecond;
    [SerializeField] private Vector3 _rotateFunctionAxis;

    private Vector3 _storedRotation;
    
    [Space] 
    [SerializeField] private GameObject _rotationIndepedentParent;

    [Space] 
    [Header("Rotate with curve and target")]
    [SerializeField] private bool _hasStartRotationOverride;
    [SerializeField] private Vector3 _startRotationOverride;
    [SerializeField] private Vector3 _targetLocalEulerAngles;
    [SerializeField] private CurveProgression _curveProgression;
    private Vector3 _startLocalEulerAngles;
    
    void OnEnable()
    {
        SubscribeToEvents();

        if (_hasStartRotationOverride)
        {
            _startLocalEulerAngles = _startRotationOverride;
        }
        else
        {
            _startLocalEulerAngles = transform.localEulerAngles;
        }
        
        if (_doesBeginRotationOnEnable)
        {
            BeginRotation();
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    public void BeginRotation()
    {
        StopRotation();

        if (_doesResetRotationOnRotationEnable)
        {
            ResetRotation();
        }

        if (_doesResetLocalRotationOnRotationEnable)
        {
            ResetLocalRotation();
        }

        if (_doesResetRotationToDefaultOnEnable)
        {
            ResetToDefaultRotation();
        }

        if (_doesUseRotateFunction)
        {
            _rotationCoroutine = StartCoroutine(RotationWithRotate());
        }
        else
        {
            _rotationCoroutine = StartCoroutine(_rotationIndepedentParent.IsUnityNull() ? RotationProcess() : RotationWithParentProcess());
        }
        
    }

    public void StopRotation()
    {
        if (!_rotationCoroutine.IsUnityNull())
        {
            StopCoroutine(_rotationCoroutine);
            _rotationCoroutine = null;
        }
    }

    public void ResetRotation()
    {
        transform.eulerAngles = Vector3.zero;
    }

    public void ResetLocalRotation()
    {
        transform.localEulerAngles = Vector3.zero;
    }

    public void ResetToDefaultRotation()
    {
        transform.localEulerAngles = _startLocalEulerAngles;
    }

    private IEnumerator RotationProcess()
    {
        while (!gameObject.IsUnityNull())
        {
            AddRotationWithoutParent(_rotationPerSecond * Time.deltaTime);
            yield return null;
        }
    }

    public void AddRotationWithoutParent(Vector3 rotation)
    {
        rotation *= _rotationsPerSecondAcceleration;
        if (_doesUseLocalEulerAngles)
        {
            transform.localEulerAngles += rotation;
        }
        else
        {
            transform.eulerAngles += rotation;
        }
    }

    private IEnumerator RotationWithParentProcess()
    {
        _storedRotation = Vector3.zero;
        while (!gameObject.IsUnityNull())
        {
            AddRotationWithParent(_rotationPerSecond * Time.deltaTime);
            yield return null;
        }
    }

    public void AddRotationWithParent(Vector3 rotation)
    {
        _storedRotation += rotation * _rotationsPerSecondAcceleration;
        transform.localEulerAngles = -_rotationIndepedentParent.transform.eulerAngles + _storedRotation;
    }

    public IEnumerator RotationWithRotate()
    {
        while (!gameObject.IsUnityNull())
        {
            AddRotationWithRotate(_rotateFunctionAxis, _rotateFunctionRotationsPerSecond*Time.deltaTime);
            yield return null;
        }
    }

    public void AddRotationWithRotate(Vector3 rotateAxis, float rotateAmount)
    {
        transform.Rotate(rotateAxis, rotateAmount*_rotationsPerSecondAcceleration);
    }
    
    public void UpdateRotationProgress(float rotationProgress)
    {
        transform.localEulerAngles = Vector3.Lerp(_startLocalEulerAngles, _targetLocalEulerAngles, rotationProgress);
    }

    public void UpdateAcceleration(float acceleration)
    {
        _rotationsPerSecondAcceleration = acceleration;
    }

    public void SubscribeToEvents()
    {
        if (!_rotationAccelerationCurve.IsUnityNull())
        {
            _rotationAccelerationCurve.OnCurveValueChanged.AddListener(UpdateAcceleration);
        }
        
        if (!_curveProgression.IsUnityNull())
        {
            _curveProgression.OnCurveValueChanged.AddListener(UpdateRotationProgress);
        }
    }

    public void UnsubscribeFromEvents()
    {
        if (!_rotationAccelerationCurve.IsUnityNull())
        {
            _rotationAccelerationCurve.OnCurveValueChanged.RemoveListener(UpdateAcceleration);
        }
        
        if (!_curveProgression.IsUnityNull())
        {
            _curveProgression.OnCurveValueChanged.RemoveListener(UpdateRotationProgress);
        }
    }

    #region Setters

    public void SetRotationsPerSecond(Vector3 rotationsPerSecond)
    {
        _rotationPerSecond = rotationsPerSecond;
    }
    
    public void SetRotationIndependentParent(GameObject parent)
    {
        _rotationIndepedentParent = parent;
    }

    #endregion
    
    #region Setters
    public void MultiplyTargetLocalEulerAngles(float targetAnglesMultiplier) => _targetLocalEulerAngles *= targetAnglesMultiplier;
    #endregion
}
