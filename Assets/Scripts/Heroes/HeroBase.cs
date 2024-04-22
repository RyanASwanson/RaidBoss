using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeroBase : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _meshAgent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    #region Navigation
    public void DirectNavigationTo(Vector3 newDestination)
    {
        _meshAgent.SetDestination(newDestination);
    }
    #endregion

    #region Getters

    #endregion

    #region Setters
    private void SetHeroSO(HeroSO heroSO)
    {

    }
    #endregion
}
