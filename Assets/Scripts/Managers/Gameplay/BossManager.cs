using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs general management of the boss
/// </summary>
public class BossManager : BaseGameplayManager
{
    [SerializeField] private BossBase _bossBase;

    private GameObject _bossGameObject;

    /// <summary>
    /// Sets up the manager then sets up the specific boss
    /// </summary>
    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
        _bossGameObject = _bossBase.gameObject;
        _bossBase.Setup(UniversalManagers.Instance.GetSelectionManager().GetSelectedBoss());
    }

    #region Events
    public override void SubscribeToEvents()
    {
        
    }
    #endregion

    #region Getters
    public BossBase GetBossBase() => _bossBase;
    public GameObject GetBossBaseGameObject() => _bossGameObject;
    #endregion

    #region Setters

    #endregion
}
