using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HeroPathfinding : HeroChildrenFunctionality
{
    [SerializeField] private NavMeshAgent _meshAgent;

    private Coroutine _heroMovementCoroutine = null;
    private UnityEvent _heroStartedMovingOnMesh = new UnityEvent();
    private UnityEvent _heroStoppedMovingOnMesh = new UnityEvent();

    private void HeroSOAssigned(HeroSO heroSO)
    {
        //Debug.Log("Hero Assigned SO Event");
        _meshAgent.speed = heroSO.GetMoveSpeed();
    }

    public void DirectNavigationTo(Vector3 newDestination)
    {
        _meshAgent.SetDestination(newDestination);
        StartMovingCoroutine();
    }

    private void StartMovingCoroutine()
    {
        if (_heroMovementCoroutine != null)
            StopCoroutine(_heroMovementCoroutine);
        _heroMovementCoroutine = StartCoroutine(MovingOnNavMesh());
    }

    private IEnumerator MovingOnNavMesh()
    {
        yield return new WaitForEndOfFrame();
        while(_meshAgent.hasPath )
        {
            Debug.Log("Moving");
            yield return null;
        }
    }

    #region Events
    public override void SubscribeToEvents()
    {
        myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);
    }
    #endregion

    #region Getters
    public UnityEvent HeroStartedMovingEvent() => _heroStartedMovingOnMesh;
    public UnityEvent HeroStoppedMovingEvent() => _heroStoppedMovingOnMesh;
    #endregion
}
