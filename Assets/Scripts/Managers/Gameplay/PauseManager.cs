using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : BaseGameplayManager
{
    private bool _gamePaused = false;

    private UnityEvent _gamePausedEvent = new UnityEvent();
    private UnityEvent _gameUnpausedEvent = new UnityEvent();

    public void PressGamePauseButton()
    {
        _gamePaused = !_gamePaused;
        if (_gamePaused)
            PauseGame();
        else
            UnpauseGame();
    }

    private void PauseGame()
    {
        InvokeGamePausedEvent();
        Time.timeScale = 0;
    }

    private void UnpauseGame()
    {
        InvokeGameUnpausedEvent();
        ResumeTime();
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }

    private void InvokeGamePausedEvent()
    {
        _gamePausedEvent?.Invoke();
    }

    private void InvokeGameUnpausedEvent()
    {
        _gameUnpausedEvent?.Invoke();
    }

    public override void SubscribeToEvents()
    {
        
    }

    #region Getters
    public bool GetGamePaused() => _gamePaused;

    public UnityEvent GetGamePausedEvent() => _gamePausedEvent;
    public UnityEvent GetGameUnpausedEvent() => _gameUnpausedEvent;
    #endregion

    #region Setters

    #endregion
}
