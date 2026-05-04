using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionModifierBase : MonoBehaviour
{
    
    public virtual void SetUpMissionModifier()
    {
        SubscribeToEvents();
    }

    #region StatModifiers

    public virtual void AdjustBossStatsModifier(BossStats bossStats)
    {
        
    }
    
    public virtual void AdjustHeroStatsModifier(HeroStats heroStats)
    {
        
    }
    
    #endregion

    protected virtual void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    protected virtual void SubscribeToEvents()
    {
        
    }
    
    protected virtual void UnsubscribeFromEvents()
    {
        
    }
    
    
}
