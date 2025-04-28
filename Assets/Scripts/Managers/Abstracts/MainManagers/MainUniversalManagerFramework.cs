using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Provides the base functionality for Universal Managers
/// </summary>
public abstract class MainUniversalManagerFramework : MainManagerFramework
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
    /// Subscribes to universal events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        //SceneLoadManager.Instance.GetOnGameplaySceneLoaded.AddListener(SubscribeToGameplayEvents);
        //SceneLoadManager.Instance.GetOnLeavingGameplayScene.AddListener(UnsubscribeToGameplayEvents);
    }

    /// <summary>
    /// Unsubscribes to universal events
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        //SceneLoadManager.Instance.GetOnGameplaySceneLoaded.RemoveListener(SubscribeToGameplayEvents);
        //SceneLoadManager.Instance.GetOnLeavingGameplayScene.RemoveListener(UnsubscribeToGameplayEvents);
    }

    /// <summary>
    /// Called after a gameplay scene is loaded
    /// Used to subscribe universal managers to gameplay manager events
    /// Since gameplay managers only exist in gameplay scenes you can't use the 
    ///     regular SubscribeToEvents as that is run when the universal managers are created
    /// </summary>
    protected virtual void SubscribeToGameplayEvents()
    {

    }

    /// <summary>
    /// Unsubscribed to all gameplay events
    /// </summary>
    protected virtual void UnsubscribeToGameplayEvents()
    {

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
