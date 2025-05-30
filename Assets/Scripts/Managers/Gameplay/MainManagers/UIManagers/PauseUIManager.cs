using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Contains all ui functionality related to the pause menu
/// </summary>
public class PauseUIManager : GameUIChildrenFunctionality
{
    public static PauseUIManager Instance;
    
    [SerializeField] private GameObject _gamePausedUI;
    [SerializeField] private GameObject _pauseButton;

    /// <summary>
    /// Called when the pause button on the UI is pressed or the key for is pressed.
    /// Pauses the game
    /// </summary>
    public void PauseButtonPressed()
    {
        TimeManager.Instance.PressGamePauseButton();
    }

    private void GamePausedUI()
    {
        _gamePausedUI.SetActive(true);
    }

    private void GameUnpausedUI()
    {
        _gamePausedUI.SetActive(false);
    }

    private void GameWonOrLost()
    {
        _pauseButton.SetActive(false);
    }

    #region BaseManager
    /// <summary>
    /// Establishes the Instance for the Pause UI Manager
    /// </summary>
    protected override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Subscribes to all needed events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        TimeManager.Instance.GetGamePausedEvent().AddListener(GamePausedUI);
        TimeManager.Instance.GetGameUnpausedEvent().AddListener(GameUnpausedUI);

        GameStateManager.Instance.GetBattleWonOrLostEvent().AddListener(GameWonOrLost);
    }

    #endregion
}
