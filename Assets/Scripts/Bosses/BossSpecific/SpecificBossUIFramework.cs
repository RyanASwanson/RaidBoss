using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the framework for UI elements that are specific to certain bosses
/// </summary>
public abstract class SpecificBossUIFramework : MonoBehaviour
{
    protected BossBase _myBossBase;
    protected SpecificBossFramework _associatedBossScript;

    public virtual void SetUpBossSpecificUIFunctionality(BossBase bossBase, SpecificBossFramework specificBoss)
    {
        _myBossBase = bossBase;
        _associatedBossScript = specificBoss;

        SubscribeToEvents();
    }

    protected abstract void SubscribeToEvents();
}
