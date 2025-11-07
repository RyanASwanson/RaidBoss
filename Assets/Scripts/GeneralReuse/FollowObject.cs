using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private bool _doesDisableOnStart;
    [Space]
    [SerializeField] private bool _doesHideAfterStopFollowing;
    [SerializeField] private bool _doesDestroyAfterStopFollowing;

    [Space] 
    [SerializeField] private UnityEvent _onFollowStart;
    [SerializeField] private UnityEvent _onFollowStopDelayStarted;
    [SerializeField] private UnityEvent _onFollowStop;
    
    private GameObject _currentFollowTarget;
    
    private Coroutine _followCoroutine;
    private Coroutine _stopFollowCoroutine;

    private Animator _followingObjectAnimator;
    
    private const string START_FOLLOW_ANIM_TRIGGER = "StartFollowing";
    private const string STOPPING_FOLLOW_ANIM_TRIGGER = "StopFollowing";

    private void Start()
    {
        TryGetComponent<Animator>(out _followingObjectAnimator);

        if (_doesDisableOnStart)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartFollowingObject(GameObject target)
    {
        _currentFollowTarget = target;

        StopFollowing(false);

        TriggerStartFollowingAnimation();
        InvokeOnFollowStart();
        _followCoroutine = StartCoroutine(FollowingObjectProcess());
    }

    public void StopFollowingDelayed(float delay)
    {
        TriggerStopFollowingAnimation();
        InvokeOnFollowStopDelayStarted();
        
        _stopFollowCoroutine = StartCoroutine(StopFollowingDelayProcess(delay));
    }

    private IEnumerator StopFollowingDelayProcess(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopFollowing(true);
    }

    public void StopFollowing(bool doesPerformStopFollowingChecks)
    {
        if (!_stopFollowCoroutine.IsUnityNull())
        {
            StopCoroutine(_stopFollowCoroutine);
            _stopFollowCoroutine = null;
        }
        
        if (!_followCoroutine.IsUnityNull())
        {
            StopCoroutine(_followCoroutine);
            _followCoroutine = null;
            InvokeOnFollowStop();

            if (doesPerformStopFollowingChecks)
            {
                if (_doesHideAfterStopFollowing)
                {
                    gameObject.SetActive(false);
                }
                else if (_doesDestroyAfterStopFollowing)
                {
                    Destroy(gameObject);
                }
            }
        }
        
    }

    private IEnumerator FollowingObjectProcess()
    {
        while (!_currentFollowTarget.IsUnityNull())
        {
            transform.position = _currentFollowTarget.transform.position;
            yield return null;
        }
    }

    private void TriggerStartFollowingAnimation()
    {
        if (!_followingObjectAnimator.IsUnityNull())
        {
            _followingObjectAnimator.SetTrigger(START_FOLLOW_ANIM_TRIGGER);
        }
    }
    
    private void TriggerStopFollowingAnimation()
    {
        if (!_followingObjectAnimator.IsUnityNull())
        {
            _followingObjectAnimator.SetTrigger(STOPPING_FOLLOW_ANIM_TRIGGER);
        }
    }

    public void InvokeOnFollowStart()
    {
        _onFollowStart?.Invoke();
    }

    public void InvokeOnFollowStopDelayStarted()
    {
        _onFollowStopDelayStarted?.Invoke();
    }
    
    public void InvokeOnFollowStop()
    {
        _onFollowStop?.Invoke();
    }
}
