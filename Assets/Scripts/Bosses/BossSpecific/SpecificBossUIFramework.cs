using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificBossUIFramework : MonoBehaviour
{
    protected BossBase _myBossBase;
    protected SpecificBossFramework _associatedBossScript;

    public virtual void SetupBossSpecificUIFunctionality(BossBase bossBase, SpecificBossFramework specificBoss)
    {
        _myBossBase = bossBase;
        _associatedBossScript = specificBoss;

        SubscribeToEvents();
    }

    protected abstract void SubscribeToEvents();
}
