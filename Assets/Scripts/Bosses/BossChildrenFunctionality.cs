using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossChildrenFunctionality : MonoBehaviour
{
    // TODO Remove this boss base
    protected BossBase _myBossBase;
    
    public virtual void ChildFuncSetUp(BossBase bossBase)
    {
        _myBossBase = bossBase;
        SetUpInstance();
        SubscribeToEvents();
    }
    
    /// <summary>
    /// Performs needed set up on the Instance
    /// </summary>
    protected virtual void SetUpInstance()
    {
        
    }

    public abstract void SubscribeToEvents();
}
