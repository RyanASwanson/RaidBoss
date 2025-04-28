using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the base functionality for Gameplay Managers
/// Inherits from BaseManager which provides the basic functionality for all managers
/// </summary>
public abstract class MainGameplayManagerFramework : MainManagerFramework
{
    /// <summary>
    /// Overrides the set up instance function
    /// </summary>
    public override void SetUpInstance()
    {

    }

    /// <summary>
    /// Subscribes the manager to needed events
    /// </summary>
    public override void SetUpMainManager()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Used to subscribe to all events required for functionality
    /// </summary>
    protected override void SubscribeToEvents()
    {

    }

    /// <summary>
    /// Unsubscribes from all events on destruction
    /// </summary>
    protected override void UnsubscribeToEvents()
    {

    }
}
