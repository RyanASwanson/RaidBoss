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
    [SerializeField] private UnityEvent _onEndOfMovement;

    private GameObject _moveTarget;
    private Vector3 _moveTargetPosition;
    private Vector3 _startPosition;
    
    private Coroutine _moveCoroutine;

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
            transform.position = Vector3.LerpUnclamped(_startPosition,_moveTargetPosition, _movementCurve.Evaluate(moveTimer));
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
            transform.localPosition = Vector3.LerpUnclamped(_startPosition,_moveTargetPosition, _movementCurve.Evaluate(moveTimer));
            yield return null;
        }

        EndOfMovement();
    }
    
    /// <summary>
    /// Is called when the projectile reaches the location of the boss
    /// </summary>
    private void EndOfMovement()
    {
        InvokeOnEndOfMovement();

        Destroy(gameObject,_floatDestroyDelay);
    }
    
    #region Events

    public void InvokeOnEndOfMovement()
    {
        _onEndOfMovement?.Invoke();
    }
    #endregion
}
