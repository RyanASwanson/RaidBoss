using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the movement of the heroes and how they pathfind
/// Allows for heroes to be directed to a location 
/// </summary>
public class HeroPathfinding : HeroChildrenFunctionality
{
    [SerializeField] private GameObject _rotationObject;
    [SerializeField] private float _rotateSpeedMultiplier;

    private Coroutine _heroRotationCoroutine = null;

    [Space]
    [SerializeField] private NavMeshAgent _meshAgent;

    private Coroutine _heroMovementCoroutine = null;

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
        // Checks if the hero is already moving
        if (!_heroMovementCoroutine.IsUnityNull())
        {
            // Stop their current movement so the new movement can take its place
            StopCoroutine(_heroMovementCoroutine);
        }
        else
        {
            // Invoke the hero started moving event as the hero isn't moving yet
            _myHeroBase.InvokeHeroStartedMovingEvent();
        }

        _heroMovementCoroutine = StartCoroutine(MovingOnNavMesh());
    }

    public void BriefStopCurrentMovement()
    {
        _meshAgent.isStopped = true;
        _meshAgent.SetDestination(transform.position);
        StartCoroutine(BriefReenableMovement());
    }

    private IEnumerator BriefReenableMovement()
    {
        yield return null;

        if (!_myHeroBase.GetHeroStats().IsHeroDead())
        {
            _meshAgent.isStopped = false;
        }

        
    }

    public void StopAbilityToMove()
    {
        _meshAgent.speed = 0;
        _meshAgent.angularSpeed = 0;
        _meshAgent.isStopped = true;
    }

    public void EnableAbilityToMove()
    {
        HeroStats heroStats = _myHeroBase.GetHeroStats();

        if (heroStats.IsHeroDead()) return;

        _meshAgent.speed = heroStats.GetCurrentSpeed();
        _meshAgent.angularSpeed = heroStats.GetAngularSpeed();
        _meshAgent.isStopped = false;
    }

    /// <summary>
    /// This coroutine remains active as the player is moving on the nav mesh
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovingOnNavMesh()
    {
        StopHeroLookAt();
        _meshAgent.autoRepath = true;

        yield return new WaitForEndOfFrame();
        while(!gameObject.IsUnityNull() && _meshAgent.hasPath )
        {
            yield return null;

            if (_meshAgent.pathStatus == NavMeshPathStatus.PathInvalid || _meshAgent.pathStatus == NavMeshPathStatus.PathPartial)
                print("cant find path end");
        }
        _myHeroBase.InvokeHeroStoppedMovingEvent();
        _heroMovementCoroutine = null;

        HeroLookAtBoss();
    }

    #region Hero Rotation
    /// <summary>
    /// Rotates the hero to look in the direction of the boss
    /// </summary>
    private void HeroLookAtBoss()
    {
        HeroLookAt(BossBase.Instance.transform.position);
    }

    /// <summary>
    /// Rotates the hero to look at a specific location
    /// </summary>
    /// <param name="lookLocation"> The location to look at </param>
    public void HeroLookAt(Vector3 lookLocation)
    {
        _heroRotationCoroutine = StartCoroutine(LookAtProcess(lookLocation));
    }

    /// <summary>
    /// Stops the process of the hero looking at something
    /// </summary>
    private void StopHeroLookAt()
    {
        if (!_heroRotationCoroutine.IsUnityNull())
        {
            StopCoroutine(_heroRotationCoroutine);
        }
    }

    /// <summary>
    /// The process by which a hero turns to look at something
    /// </summary>
    /// <param name="lookLocation"> The target to look at</param>
    /// <returns></returns>
    private IEnumerator LookAtProcess(Vector3 lookLocation)
    {
        float progress = 0;
        Quaternion startingRotation = _rotationObject.transform.rotation;
        HeroStats heroStats = _myHeroBase.GetHeroStats();

        while (progress < 1)
        {
            progress += Time.deltaTime * _rotateSpeedMultiplier * heroStats.GetAngularSpeed() ;

            Vector3 lookDir = lookLocation - _rotationObject.transform.position;

            //Quaternion toRotation = Quaternion.FromToRotation(_visualObjectBase.transform.forward, lookDir);
            Quaternion toRotation = Quaternion.LookRotation(lookDir);

            _rotationObject.transform.rotation = Quaternion.Lerp
                (startingRotation, toRotation, progress);


            _rotationObject.transform.eulerAngles = new Vector3(0, _rotationObject.transform.eulerAngles.y, 0);

            yield return null;
        }

    }
    #endregion

    #region Base Hero
    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        _myHeroBase.GetHeroDiedEvent().AddListener(StopAbilityToMove);
    }
    #endregion

    #region Getters
    public NavMeshAgent GetNavMeshAgent() => _meshAgent;

    public bool IsHeroMoving() => !_heroMovementCoroutine.IsUnityNull();
    #endregion
}
