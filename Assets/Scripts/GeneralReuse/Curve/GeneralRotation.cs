using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeneralRotation : MonoBehaviour
{
    [SerializeField] private bool _doesBeginRotationOnEnable;
    [SerializeField] private bool _doesResetRotationOnRotationEnable;
    
    private Coroutine _rotationCoroutine;
    
    [Space]
    [SerializeField] private Vector3 _rotationPerSecond;

    [Space] 
    [SerializeField] private GameObject _rotationIndepedentParent;

    [Space] 
    [Header("Rotate with curve and target")]
    [SerializeField] private Vector3 _targetLocalEulerAngles;
    [SerializeField] private CurveProgression _curveProgression;
    private Vector3 _startLocalEulerAngles;
    
    void OnEnable()
    {
        SubscribeToEvents();
        _startLocalEulerAngles = transform.localEulerAngles;
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
        
        _rotationCoroutine = StartCoroutine(_rotationIndepedentParent.IsUnityNull() ? RotationProcess() : RotationWithParent());
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

    private IEnumerator RotationProcess()
    {
        while (!gameObject.IsUnityNull())
        {
            transform.eulerAngles += _rotationPerSecond * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RotationWithParent()
    {
        Vector3 storedRotation = Vector3.zero;
        while (!gameObject.IsUnityNull())
        {
            storedRotation += _rotationPerSecond * Time.deltaTime;
            transform.localEulerAngles = -_rotationIndepedentParent.transform.eulerAngles + storedRotation;
            yield return null;
        }
    }
    
    public void UpdateRotationProgress(float rotationProgress)
    {
        transform.localEulerAngles = Vector3.Lerp(_startLocalEulerAngles, _targetLocalEulerAngles, rotationProgress);
        Debug.Log("Rotation progress " + rotationProgress);
    }

    public void SubscribeToEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.AddListener(UpdateRotationProgress);
        }
    }

    public void UnsubscribeFromEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.RemoveListener(UpdateRotationProgress);
        }
    }

    #region Setters

    public void SetRotationIndependentParent(GameObject parent)
    {
        _rotationIndepedentParent = parent;
    }

    #endregion
}
