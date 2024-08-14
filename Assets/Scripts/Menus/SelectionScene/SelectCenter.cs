using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menu in the middle of the selection screen
/// Handles starting the level
/// </summary>
public class SelectCenter : MonoBehaviour
{
    /// <summary>
    /// Causes the game to proceed to the currently selected level
    /// Called by play button press
    /// </summary>
    public void PlayLevel()
    {
        UniversalManagers.Instance.GetSceneLoadManager().LoadCurrentlySelectedLevelSO();
    }
 
}
