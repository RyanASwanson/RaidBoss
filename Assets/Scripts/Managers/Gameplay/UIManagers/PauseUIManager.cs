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
        UniversalManagers.Instance.GetTimeManager().PressGamePauseButton();
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

    public override void ChildFuncSetup()
    {
        base.ChildFuncSetup();
    }


    public override void SubscribeToEvents()
    {
        UniversalManagers.Instance.GetTimeManager().GetGamePausedEvent().AddListener(GamePausedUI);
        UniversalManagers.Instance.GetTimeManager().GetGameUnpausedEvent().AddListener(GameUnpausedUI);

        GameplayManagers.Instance.GetGameStateManager().GetBattleWonOrLostEvent().AddListener(GameWonOrLost);
    }
}
