using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeroPathfinding : HeroChildrenFunctionality
{
    [SerializeField] private NavMeshAgent _meshAgent;

    private void HeroSOAssigned(HeroSO heroSO)
    {
        Debug.Log("Hero Assigned SO Event");
        _meshAgent.speed = heroSO.GetMoveSpeed();
    }

    public void DirectNavigationTo(Vector3 newDestination)
    {
        _meshAgent.SetDestination(newDestination);
    }

    #region Events
    public override void SubscribeToEvents(HeroBase heroBase)
    {
        heroBase.GetSOSetEvent().AddListener(HeroSOAssigned);
    }
    #endregion
}
