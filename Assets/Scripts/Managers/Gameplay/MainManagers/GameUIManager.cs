using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the Ui for the gameplay
/// Holds all other needed UI Managers for gameplay for more specific needs
/// </summary>
public class GameUIManager : MainGameplayManagerFramework
{
    public static GameUIManager Instance;
    
    [SerializeField] private BossUIManager _bossUIManager;

    [SerializeField] private List<HeroUIManager> _heroUIManagers;

    [SerializeField] private PauseUIManager _pauseUIManager;

    [SerializeField] private GameStateUIManager _gameStateUIManager;

    private int _heroUIManagersAssigned = 0;
    
    /// <summary>
    /// Sets up all scripts that inherit from GameUIChildrenFunctionality
    /// </summary>
    private void SetUpChildrenUIManagers()
    {
        foreach (GameUIChildrenFunctionality gameUIChildrenFunctionality
                 in GetComponentsInChildren<GameUIChildrenFunctionality>())
        {
            gameUIChildrenFunctionality.ChildFuncSetUp();
        }
    }

    #region BaseManager
    /// <summary>
    /// Establishes the Instance for the Game UI Manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Performs the set up for the Game UI Manager
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        SetUpChildrenUIManagers();
    }
    #endregion

    #region Setters
    public HeroUIManager SetAssociatedHeroUIManager(HeroBase heroBase)
    {
        HeroUIManager.Instances[_heroUIManagersAssigned] = _heroUIManagers[_heroUIManagersAssigned];
        
        _heroUIManagers[_heroUIManagersAssigned++].AssignSpecificHero(heroBase);

        return _heroUIManagers[_heroUIManagersAssigned-1];
    }    
    #endregion
}

/// <summary>
/// Provides an enum to be used for the numbers that appear after damage/stagger/healing/etc.
/// </summary>
public enum EDamageNumberType
{
    Damage,
    Stagger,
    Healing
};
