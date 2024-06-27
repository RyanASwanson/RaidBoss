using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenFunctionality : MonoBehaviour
{
    public void ResumeGame()
    {
        GameplayManagers.Instance.GetPauseManager().PressGamePauseButton();
    }

    public void RetryLevel()
    {
        GameplayManagers.Instance.GetPauseManager().ResumeTime();
        UniversalManagers.Instance.GetSceneLoadManager().ReloadCurrentScene();
    }

    public void BackToSelectionScreen()
    {
        GameplayManagers.Instance.GetPauseManager().ResumeTime();
        UniversalManagers.Instance.GetSceneLoadManager().LoadSelectionScene();
    }

    public void BackToMainMenu()
    {
        GameplayManagers.Instance.GetPauseManager().ResumeTime();
        UniversalManagers.Instance.GetSceneLoadManager().LoadMainMenuScene();
    }
}
