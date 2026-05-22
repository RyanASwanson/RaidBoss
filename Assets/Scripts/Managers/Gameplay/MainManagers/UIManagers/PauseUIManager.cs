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
    [SerializeField] private Button _pauseButton;

    private void SetUpPauseButton()
    {
        _pauseButton.onClick.AddListener(PauseButtonPressed);

        TogglePauseButtonInteractability(false);
    }

    private void SceneFullyLoaded()
    {
        TogglePauseButtonInteractability(true);
    }
    
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

    public void TutorialToggle(bool isTutorialActive)
    {
        TogglePauseButtonInteractability(!isTutorialActive);
    }

    public void TogglePauseButtonInteractability(bool isInteractable)
    {
        _pauseButton.interactable = isInteractable;
    }

    private void GameWonOrLost()
    {
        _pauseButton.gameObject.SetActive(false);
    }

    #region BaseManager

    public override void ChildFuncSetUp()
    {
        base.ChildFuncSetUp();
        SetUpPauseButton();
    }

    /// <summary>
    /// Establishes the Instance for the Pause UI Manager
    /// </summary>
    public override void SetUpInstance()
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
        
        SceneLoadManager.Instance.GetOnEndOfSceneLoad().AddListener(SceneFullyLoaded);
        
        GameStateManager.Instance.GetBattleWonOrLostEvent().AddListener(GameWonOrLost);
    }

    protected override void UnsubscribeFromEvents()
    {
        TimeManager.Instance.GetGamePausedEvent().RemoveListener(GamePausedUI);
        TimeManager.Instance.GetGameUnpausedEvent().RemoveListener(GameUnpausedUI);
        
        SceneLoadManager.Instance.GetOnEndOfSceneLoad().RemoveListener(SceneFullyLoaded);
        
        GameStateManager.Instance.GetBattleWonOrLostEvent().RemoveListener(GameWonOrLost);
    }
    #endregion
}
