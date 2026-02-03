using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MoveBetween : MonoBehaviour
{
    [SerializeField] private float _moveTime;
    [SerializeField] private AnimationCurve _movementCurve;

    [Space] 
    [SerializeField] private float _floatDestroyDelay;

    [Space] 
    [SerializeField] private bool _hasDefaultStartPosition;
    [SerializeField] private Vector3 _defaultStartPosition;

    [Space] 
    [SerializeField] private UnityEvent _onEndOfMovement;
    
    
    [Space]
    [SerializeField] private Vector3 _defaultEndPosition;
    [SerializeField] private CurveProgression _curveProgression;

    private GameObject _moveTarget;
    private Vector3 _moveTargetPosition;
    private Vector3 _startPosition;
    
    private Coroutine _moveCoroutine;

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    public void StartMoveProcess(GameObject target)
    {
        _moveTarget = target;
        _moveCoroutine = StartCoroutine(MoveProcessWithGameObjectTarget());
    }

    public void StartMoveProcess(Vector3 targetPosition)
    {
        _moveTargetPosition = targetPosition;
        _moveCoroutine = StartCoroutine(MoveProcessWithPositionTarget());
    }

    public void StartMoveProcess(Vector3 targetPosition, Vector3 startPosition)
    {
        transform.position = startPosition;
        StartMoveProcess(targetPosition);
    }

    public void StartMoveProcessWithCurveProgression()
    {
        StartMoveProcessWithCurveProgression(_defaultEndPosition);

    }

    public void StartMoveProcessWithCurveProgression(Vector3 targetPosition)
    {
        if (_hasDefaultStartPosition)
        {
            _startPosition = _defaultStartPosition;
        }
        _moveTargetPosition = targetPosition;
        
        _curveProgression.StartMovingUpOnCurve();
    }

    public void StopMoveProcess()
    {
        if (!_moveCoroutine.IsUnityNull())
        {
            StopCoroutine(_moveCoroutine);
        }
    }
    
    /// <summary>
    /// The process of moving the passive projectile from where its created
    /// to the location of the boss
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProcessWithGameObjectTarget()
    {
        _startPosition = transform.position;

        float moveTimer = 0;

        while (moveTimer < 1)
        {
            if (_moveTarget)
            {
                _moveTargetPosition = _moveTarget.transform.position;
            }
            moveTimer += Time.deltaTime / _moveTime;
            //transform.position = Vector3.LerpUnclamped(_startPosition,_moveTargetPosition, _movementCurve.Evaluate(moveTimer));
            UpdateLocalPosition(moveTimer);
            yield return null;
        }

        EndOfMovement();
    }

    private IEnumerator MoveProcessWithPositionTarget()
    {
        _startPosition = transform.position;

        float moveTimer = 0;

        while (moveTimer < 1)
        {
            moveTimer += Time.deltaTime / _moveTime;
            //transform.localPosition = Vector3.LerpUnclamped(_startPosition,_moveTargetPosition, _movementCurve.Evaluate(moveTimer));
            UpdateLocalPosition(moveTimer);
            yield return null;
        }

        EndOfMovement();
    }
    
    public void UpdateLocalPosition(float scaleProgress)
    {
        transform.localPosition = Vector3.LerpUnclamped(_startPosition,_moveTargetPosition, _movementCurve.Evaluate(scaleProgress));
    }
    
    /// <summary>
    /// Is called when the projectile reaches the location of the boss
    /// </summary>
    private void EndOfMovement()
    {
        InvokeOnEndOfMovement();

        Destroy(gameObject,_floatDestroyDelay);
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
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
    
    #region Events

    public void InvokeOnEndOfMovement()
    {
        _onEndOfMovement?.Invoke();
    }
    #endregion
}
