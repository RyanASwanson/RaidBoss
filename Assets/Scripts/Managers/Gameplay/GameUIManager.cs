using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the ui for the gameplay
/// </summary>
public class GameUIManager : BaseGameplayManager
{
    [SerializeField] private BossUIManager _bossUIManager;

    [SerializeField] private List<HeroUIManager> _heroUIManagers;

    [SerializeField] private PauseUIManager _pauseUIManager;

    [SerializeField] private GameStateUIManager _gameStateUIManager;

    private int _heroUIManagersAssigned = 0;

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
        SetupChildrenUIManagers();
    }

    /// <summary>
    /// Sets up all scripts that inherit from GameUIChildrenFunctionality
    /// </summary>
    private void SetupChildrenUIManagers()
    {
        foreach (GameUIChildrenFunctionality gameUIChildrenFunctionality
            in GetComponentsInChildren<GameUIChildrenFunctionality>())
            gameUIChildrenFunctionality.ChildFuncSetup() ;
    }

    public override void SubscribeToEvents()
    {
        
    }

    #region Getters
    public BossUIManager GetBossUIManager() => _bossUIManager;
    public HeroUIManager GetHeroUIManagerAt(int pos) => _heroUIManagers[pos];
    public PauseUIManager GetPauseManager() => _pauseUIManager;
    public GameStateUIManager GetGameStateUIManager() => _gameStateUIManager;
    #endregion

    #region Setters
    public void SetAssociatedHeroUIManager(HeroBase heroBase)
    {
        _heroUIManagers[_heroUIManagersAssigned++].AssignSpecificHero(heroBase);
    }    
    #endregion
}
