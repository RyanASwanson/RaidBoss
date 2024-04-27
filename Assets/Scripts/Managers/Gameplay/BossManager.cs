using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : BaseGameplayManager
{
    [SerializeField] private BossBase _bossBase;

    [SerializeField] BossSO TESTINGBOSSSO;

    /// <summary>
    /// Sets up the manager then sets up the specific boss
    /// </summary>
    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
        _bossBase.Setup(TESTINGBOSSSO);
    }

    #region Events
    public override void SubscribeToEvents()
    {
        
    }
    #endregion

    #region Getters
    public BossBase GetBossBase() => _bossBase;
    #endregion

    #region Setters

    #endregion
}
