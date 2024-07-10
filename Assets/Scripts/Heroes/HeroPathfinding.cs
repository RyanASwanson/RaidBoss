using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Controls the movement of the heroes and how they pathfind
/// Allows for heroes to be directed to a location 
/// </summary>
public class HeroPathfinding : HeroChildrenFunctionality
{
    [SerializeField] private NavMeshAgent _meshAgent;

    private Coroutine _heroMovementCoroutine = null;

    public override void ChildFuncSetup(HeroBase heroBase)
    {
        base.ChildFuncSetup(heroBase);
    }

    /// <summary>
    /// Makes a player walk to a destination
    /// </summary>
    /// <param name="newDestination"></param>
    public void DirectNavigationTo(Vector3 newDestination)
    {
        _meshAgent.SetDestination(newDestination);
        StartMovingCoroutine();
    }

    /// <summary>
    /// Starts the coroutine for moving on the nav mesh
    /// </summary>
    private void StartMovingCoroutine()
    {
        if (_heroMovementCoroutine != null)
            StopCoroutine(_heroMovementCoroutine);
        else
            myHeroBase.InvokeHeroStartedMovingEvent();

        _heroMovementCoroutine = StartCoroutine(MovingOnNavMesh());
    }

    private void StopAbilityToMove()
    {
        _meshAgent.speed = 0;
        _meshAgent.angularSpeed = 0;
        _meshAgent.isStopped = true;
    }

    /// <summary>
    /// This coroutine remains active as the player is moving on the nav mesh
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovingOnNavMesh()
    {
        yield return new WaitForEndOfFrame();
        while(_meshAgent.hasPath )
        {
            yield return null;
        }
        myHeroBase.InvokeHeroStoppedMovingEvent();
        _heroMovementCoroutine = null;
    }

    #region Events
    public override void SubscribeToEvents()
    {
        myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);

        myHeroBase.GetHeroDiedEvent().AddListener(StopAbilityToMove);
    }

    private void HeroSOAssigned(HeroSO heroSO)
    {
        
    }
    #endregion

    #region Getters
    public NavMeshAgent GetNavMeshAgent() => _meshAgent;

    public bool IsHeroMoving() => _heroMovementCoroutine != null;
    #endregion
}
