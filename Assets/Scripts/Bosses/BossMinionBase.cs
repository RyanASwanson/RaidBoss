using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossMinionBase : MonoBehaviour
{
    private BossBase _myBossBase;
    private SpecificBossFramework _mySpecificBoss;

    public virtual void SetupMinion()
    {

    }
}
