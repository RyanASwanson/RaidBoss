using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeneralRotation : MonoBehaviour
{
    [SerializeField] private bool _doesBeginRotationOnStart;
    [SerializeField] private bool _doesResetRotationOnRotationStart;
    
    private Coroutine _rotationCoroutine;
    
    [Space]
    [SerializeField] private Vector3 _rotationPerSecond;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_doesBeginRotationOnStart)
        {
            BeginRotation();
        }
    }

    public void BeginRotation()
    {
        StopRotation();

        if (_doesResetRotationOnRotationStart)
        {
            ResetRotation();
        }
        
        _rotationCoroutine = StartCoroutine(RotationProcess());
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
}
