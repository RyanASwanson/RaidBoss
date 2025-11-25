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
    
    void OnEnable()
    {
        if (_doesBeginRotationOnEnable)
        {
            BeginRotation();
        }
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

    #region Setters

    public void SetRotationIndependentParent(GameObject parent)
    {
        _rotationIndepedentParent = parent;
    }

    #endregion
}
