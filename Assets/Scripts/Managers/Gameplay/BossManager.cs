using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : BaseGameplayManager
{
    [SerializeField] private BossBase _bossBase;

    [SerializeField] BossSO TESTINGBOSSSO;

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

    #endregion

    #region Setters

    #endregion
}
