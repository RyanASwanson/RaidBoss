using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenFunctionality : MonoBehaviour
{
    public void ResumeGame()
    {
        GameplayManagers.Instance.GetPauseManager().PressGamePauseButton();
    }

    public void BackToSelectionScreen()
    {

    }

    public void BackToMainMenu()
    {

    }
}
