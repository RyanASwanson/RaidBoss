using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalDontDestroy : MonoBehaviour
{
    public static UniversalDontDestroy Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
