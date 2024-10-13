using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossChildrenFunctionality : MonoBehaviour
{
    protected BossBase _myBossBase;

    public virtual void ChildFuncSetup(BossBase bossBase)
    {
        _myBossBase = bossBase;
        SubscribeToEvents();
    }

    public abstract void SubscribeToEvents();
}
