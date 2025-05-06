using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseUIManager : GameUIChildrenFunctionality
{
    [SerializeField] private GameObject _gamePausedUI;
    [SerializeField] private GameObject _pauseButton;

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

    protected override void SubscribeToEvents()
    {
        TimeManager.Instance.GetGamePausedEvent().AddListener(GamePausedUI);
        TimeManager.Instance.GetGameUnpausedEvent().AddListener(GameUnpausedUI);

        GameStateManager.Instance.GetBattleWonOrLostEvent().AddListener(GameWonOrLost);
    }

    #endregion
}
