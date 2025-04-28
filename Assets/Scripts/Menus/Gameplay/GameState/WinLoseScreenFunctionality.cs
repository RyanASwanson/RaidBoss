using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs functionality needed for the win lose screen
/// </summary>
public class WinLoseScreenFunctionality : MonoBehaviour
{
    public void RetryLevel()
    {
        SceneLoadManager.Instance.ReloadCurrentScene();
    }

    public void BackToSelectionScreen()
    {
        SceneLoadManager.Instance.LoadSelectionScene();
    }

    public void BackToMainMenu()
    {
        SceneLoadManager.Instance.LoadMainMenuScene();
    }
}
