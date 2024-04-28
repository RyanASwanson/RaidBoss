using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificBossFramework : MonoBehaviour
{

    internal BossBase myBossBase;


    public virtual HeroBase DetermineAggroTarget()
    {

        return null;
    }

    
    public virtual void SetupSpecificBoss(BossBase bossBase)
    {
        myBossBase = bossBase;
        SubscribeToEvents();
    }

    #region Events
    public virtual void SubscribeToEvents()
    {
        
    }

    #endregion
}
