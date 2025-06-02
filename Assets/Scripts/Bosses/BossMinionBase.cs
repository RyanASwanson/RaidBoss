using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a base class for boss minions for any shared functionality
/// </summary>
public abstract class BossMinionBase : MonoBehaviour
{
    private BossBase _myBossBase;
    private SpecificBossFramework _mySpecificBoss;

    public virtual void SetUpMinion(BossBase bossBase, SpecificBossFramework specificBoss)
    {
        _myBossBase = bossBase;
        _mySpecificBoss = specificBoss;
    }
}
