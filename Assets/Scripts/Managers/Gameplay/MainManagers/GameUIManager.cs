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
    
    private GameUIChildrenFunctionality[] _childrenUIFunctionality;

    [SerializeField] private List<HeroUIManager> _heroUIManagers;

    private int _heroUIManagersAssigned = 0;

    /// <summary>
    /// Sets up the instances for the scripts that inherit from GameUIChildrenFunctionality
    /// </summary>
    private void SetUpChildrenUIInstances()
    {
        foreach (GameUIChildrenFunctionality gameUIChildrenFunctionality
                 in _childrenUIFunctionality)
        {
            gameUIChildrenFunctionality.SetUpInstance();
        }
    }
    
    /// <summary>
    /// Sets up all scripts that inherit from GameUIChildrenFunctionality
    /// </summary>
    private void SetUpChildrenUIManagers()
    {
        foreach (GameUIChildrenFunctionality gameUIChildrenFunctionality
                 in _childrenUIFunctionality)
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
        _childrenUIFunctionality = GetComponentsInChildren<GameUIChildrenFunctionality>();
        
        SetUpChildrenUIInstances();
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
