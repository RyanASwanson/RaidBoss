using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : BaseGameplayManager
{
    [SerializeField] private BossUIManager _bossUIManager;

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
    }

    public override void SubscribeToEvents()
    {
        
    }

    #region Getters
    public BossUIManager GetBossUIManager() => _bossUIManager;
    #endregion

    #region Setters

    #endregion
}
