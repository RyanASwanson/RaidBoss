using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeneralTranslate : MonoBehaviour
{
    [SerializeField] private float _speed;

    [Space]
    [SerializeField] private CurveProgression _curveProgression;
    private float _acceleration = 1;

    private Coroutine _moveCoroutine;
    
    void OnEnable()
    {
        SubscribeToEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    public void StartMoving(Vector3 direction)
    {
        StopMoving();
        
        _moveCoroutine = StartCoroutine(MoveObject(direction));
    }

    public void StopMoving()
    {
        if (!_moveCoroutine.IsUnityNull())
        {
            StopCoroutine(_moveCoroutine);
        }
    }

    /// <summary>
    /// Moves the projectile in a given direction
    /// </summary>
    /// <param name="direction"> The direction to move in</param>
    /// <returns></returns>
    private IEnumerator MoveObject(Vector3 direction)
    {
        while (!gameObject.IsUnityNull())
        {
            transform.position += direction * (_speed * _acceleration * Time.deltaTime);
            yield return null;
        }
    }
    
    public void UpdateAccelerationCurve(float scaleProgress)
    {
        _acceleration = Mathf.LerpUnclamped(0, _speed, scaleProgress);
    }

    public void SubscribeToEvents()
    {
        if (!_curveProgression.IsUnityNull())
        {
            _curveProgression.OnCurveValueChanged.AddListener(UpdateAccelerationCurve);
        }
    }

    public void UnsubscribeFromEvents()
    {
        if (!_curveProgression.IsUnityNull())
        {
            _curveProgression.OnCurveValueChanged.RemoveListener(UpdateAccelerationCurve);
        }
    }
}
