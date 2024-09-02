using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the Ui for the gameplay
/// Holds all other needed UI Managers for gameplay for more specific needs
/// </summary>
public class GameUIManager : BaseGameplayManager
{
    [SerializeField] private BossUIManager _bossUIManager;

    [SerializeField] private List<HeroUIManager> _heroUIManagers;

    [SerializeField] private PauseUIManager _pauseUIManager;

    [SerializeField] private GameStateUIManager _gameStateUIManager;

    private int _heroUIManagersAssigned = 0;

    
    /// <summary>
    /// Sets up all scripts that inherit from GameUIChildrenFunctionality
    /// </summary>
    private void SetupChildrenUIManagers()
    {
        foreach (GameUIChildrenFunctionality gameUIChildrenFunctionality
            in GetComponentsInChildren<GameUIChildrenFunctionality>())
            gameUIChildrenFunctionality.ChildFuncSetup() ;
    }

    #region BaseManager
    public override void SetupManager()
    {
        base.SetupManager();
        SetupChildrenUIManagers();
    }

    #endregion

    #region Getters
    public BossUIManager GetBossUIManager() => _bossUIManager;
    public HeroUIManager GetHeroUIManagerAt(int pos) => _heroUIManagers[pos];
    public PauseUIManager GetPauseManager() => _pauseUIManager;
    public GameStateUIManager GetGameStateUIManager() => _gameStateUIManager;
    #endregion

    #region Setters
    public HeroUIManager SetAssociatedHeroUIManager(HeroBase heroBase)
    {
        _heroUIManagers[_heroUIManagersAssigned++].AssignSpecificHero(heroBase);

        return _heroUIManagers[_heroUIManagersAssigned-1];
    }    
    #endregion
}
