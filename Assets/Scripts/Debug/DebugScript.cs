using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Script purely used for testing misc functionality. Is not active in builds.
/// </summary>
public class DebugScript : MonoBehaviour
{
    public static DebugScript Instance;
    
    private void Start()
    {
        if(Instance.IsUnityNull())
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            SaveManager.Instance.BossDead();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            BossStats.Instance.DealDamageToBoss(99999);
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            BossStats.Instance.DealStaggerToBoss(99999);
        }
    }
#endif
}

