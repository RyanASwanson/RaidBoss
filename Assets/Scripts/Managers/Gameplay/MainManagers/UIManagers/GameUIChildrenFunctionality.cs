using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Provides a framework for all specific UI managers for gameplay
/// HeroUIManager, BossUIManager, PauseUIManager, GameStateManager inherit from this
/// </summary>
public abstract class GameUIChildrenFunctionality : MonoBehaviour
{
    public virtual void ChildFuncSetUp()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Establishes the Instance for any script inheriting from this script
    /// </summary>
    protected virtual void SetUpInstance()
    {
        
    }
    
    /// <summary>
    /// Subscribes to any events
    /// </summary>
    protected abstract void SubscribeToEvents();

}
