using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseScreenFunctionality : MonoBehaviour
{
    public void RetryLevel()
    {
        UniversalManagers.Instance.GetSceneLoadManager().ReloadCurrentScene();
    }

    public void BackToSelectionScreen()
    {
        UniversalManagers.Instance.GetSceneLoadManager().LoadSelectionScene();
    }

    public void BackToMainMenu()
    {
        UniversalManagers.Instance.GetSceneLoadManager().LoadMainMenuScene();
    }
}
