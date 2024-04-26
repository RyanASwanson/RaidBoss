using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossChildrenFunctionality : MonoBehaviour
{
    internal BossBase myBossBase;

    public virtual void ChildFuncSetup(BossBase bossBase)
    {
        myBossBase = bossBase;
        SubscribeToEvents();
    }

    public abstract void SubscribeToEvents();
}
