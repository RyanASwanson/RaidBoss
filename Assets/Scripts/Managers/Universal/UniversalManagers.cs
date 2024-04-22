using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalManagers : MonoBehaviour
{
    public static UniversalManagers Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    #region Get Managers


    #endregion
}
