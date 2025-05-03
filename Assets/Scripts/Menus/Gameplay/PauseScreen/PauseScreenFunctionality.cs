using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functionality associated with the pause screen
/// </summary>
public class PauseScreenFunctionality : MonoBehaviour
{
    public void ResumeGame()
    {
        TimeManager.Instance.PressGamePauseButton();
    }

    public void RetryLevel()
    {
        //GameplayManagers.Instance.GetPauseManager().ResumeTime();
        SceneLoadManager.Instance.ReloadCurrentScene();
    }

    public void BackToSelectionScreen()
    {
        //GameplayManagers.Instance.GetPauseManager().ResumeTime();
        SceneLoadManager.Instance.LoadSelectionScene();
    }

    public void BackToMainMenu()
    {
        //GameplayManagers.Instance.GetPauseManager().ResumeTime();
        SceneLoadManager.Instance.LoadMainMenuScene();
    }
}
