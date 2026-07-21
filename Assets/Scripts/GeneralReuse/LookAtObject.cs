using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    private Vector3 _lastLookEuler;
    private Coroutine _lookAtCoroutine;
    
    public void StartLookingAtObject(Transform lookTarget, bool doesOnlyUseY)
    {
        if (lookTarget.IsUnityNull())
        {
            return;
        }

        StopLookingAtObject();

        if (doesOnlyUseY)
        {
            _lookAtCoroutine = StartCoroutine(LookAtObjectWithY(lookTarget));
        }
        
    }

    public void StopLookingAtObject()
    {
        if (!_lookAtCoroutine.IsUnityNull())
        {
            StopCoroutine(_lookAtCoroutine);
        }
    }

    private IEnumerator LookAtObjectWithY(Transform lookTarget)
    {
        while (!lookTarget.IsUnityNull())
        {
            LookAtPosition(lookTarget.transform.position);
            yield return null;
        }
    }


    public void LookAtPosition(Vector3 targetPosition)
    {
        _lastLookEuler = transform.localEulerAngles;
        transform.LookAt(targetPosition);
        _lastLookEuler.Set(_lastLookEuler.x, transform.localEulerAngles.y, _lastLookEuler.z);
        transform.localEulerAngles = _lastLookEuler;
    }
}
