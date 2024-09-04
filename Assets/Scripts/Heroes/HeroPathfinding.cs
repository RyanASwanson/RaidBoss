using System.Collections;
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
        if (_heroMovementCoroutine != null)
            StopCoroutine(_heroMovementCoroutine);
        else
            myHeroBase.InvokeHeroStartedMovingEvent();

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

        if (!myHeroBase.GetHeroStats().IsHeroDead())
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
        HeroStats heroStats = myHeroBase.GetHeroStats();

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
        while(gameObject != null && _meshAgent.hasPath )
        {
            yield return null;

            if (_meshAgent.pathStatus == NavMeshPathStatus.PathInvalid || _meshAgent.pathStatus == NavMeshPathStatus.PathPartial)
                print("cant find path end");
        }
        myHeroBase.InvokeHeroStoppedMovingEvent();
        _heroMovementCoroutine = null;

        HeroLookAtBoss();
    }


    #region Hero Rotation

    private void HeroLookAtBoss()
    {
        HeroLookAt(GameplayManagers.Instance.GetBossManager().GetBossBaseGameObject().transform.position);
    }

    public void HeroLookAt(Vector3 lookLocation)
    {
        _heroRotationCoroutine = StartCoroutine(LookAtProcess(lookLocation));
    }

    private void StopHeroLookAt()
    {
        if (_heroRotationCoroutine != null)
            StopCoroutine(_heroRotationCoroutine);
    }


    private IEnumerator LookAtProcess(Vector3 lookLocation)
    {
        float progress = 0;
        Quaternion startingRotation = _rotationObject.transform.rotation;
        HeroStats heroStats = myHeroBase.GetHeroStats();

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
    public override void ChildFuncSetup(HeroBase heroBase)
    {
        base.ChildFuncSetup(heroBase);
    }

    public override void SubscribeToEvents()
    {
        myHeroBase.GetHeroDiedEvent().AddListener(StopAbilityToMove);
    }
    #endregion

    #region Getters
    public NavMeshAgent GetNavMeshAgent() => _meshAgent;

    public bool IsHeroMoving() => _heroMovementCoroutine != null;
    #endregion
}
