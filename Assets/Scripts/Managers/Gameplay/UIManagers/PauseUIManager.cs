using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseUIManager : GameUIChildrenFunctionality
{
    [SerializeField] private GameObject _gamePausedUI;
    [SerializeField] private GameObject _pauseButton;

    private void GamePausedUI()
    {
        _gamePausedUI.SetActive(true);
    }

    private void GameUnpausedUI()
    {
        _gamePausedUI.SetActive(false);
    }


    public override void ChildFuncSetup()
    {
        base.ChildFuncSetup();
    }


    public override void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetPauseManager().GetGamePausedEvent().AddListener(GamePausedUI);
        GameplayManagers.Instance.GetPauseManager().GetGameUnpausedEvent().AddListener(GameUnpausedUI);
    }
}
