using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseUIManager : GameUIChildrenFunctionality
{
    [SerializeField] private GameObject _gamePausedUI;
    [SerializeField] private GameObject _pauseButton;

    private TimeManager _timeManager;

    public void PauseButtonPressed()
    {
        _timeManager.PressGamePauseButton();
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

    public override void ChildFuncSetup()
    {
        _timeManager = UniversalManagers.Instance.GetTimeManager();
        base.ChildFuncSetup();
    }


    protected override void SubscribeToEvents()
    {
        _timeManager.GetGamePausedEvent().AddListener(GamePausedUI);
        _timeManager.GetGameUnpausedEvent().AddListener(GameUnpausedUI);

        GameplayManagers.Instance.GetGameStateManager().GetBattleWonOrLostEvent().AddListener(GameWonOrLost);
    }

    #endregion
}
