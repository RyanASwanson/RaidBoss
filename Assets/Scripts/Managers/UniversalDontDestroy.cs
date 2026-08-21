using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UniversalDontDestroy : MonoBehaviour
{
    public static UniversalDontDestroy Instance;

    private void Awake()
    {
        if (Instance.IsUnityNull())
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}