using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs functionality needed for the win lose screen
/// </summary>
public class WinLoseScreenFunctionality : MonoBehaviour
{
    [SerializeField] private GeneralScrollPopUp _unlockScroll;
    
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

    public void BattleWon()
    {
        if (SelectionManager.Instance.DoesCurrentCombatHaveUnlock())
        {
            _unlockScroll.ShowScroll();
        }
    }

    public void BattleLost()
    {
        
    }
}
