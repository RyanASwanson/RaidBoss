using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeroPathfinding : MonoBehaviour
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
    public void SubscribeToEvents()
    {
        GetComponentInParent<HeroBase>().GetSOSetEvent().AddListener(HeroSOAssigned);
    }
    #endregion
}
