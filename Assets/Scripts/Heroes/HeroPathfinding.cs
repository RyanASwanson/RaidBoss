using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HeroPathfinding : HeroChildrenFunctionality
{
    [SerializeField] private NavMeshAgent _meshAgent;

    private Coroutine _heroMovementCoroutine = null;

    public override void ChildFuncSetup(HeroBase heroBase)
    {
        base.ChildFuncSetup(heroBase);
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
        Debug.Log("Pathfinding Subscription");
        myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);
    }

    private void HeroSOAssigned(HeroSO heroSO)
    {
        
    }
    #endregion

    #region Getters
    public NavMeshAgent GetNavMeshAgent() => _meshAgent;
    #endregion
}
