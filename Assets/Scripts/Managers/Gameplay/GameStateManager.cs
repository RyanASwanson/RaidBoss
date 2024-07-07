using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : BaseGameplayManager
{
    [SerializeField] private float _timeToStart;

    private GameplayStates _currentGameplayState = GameplayStates.PreBattle;

    private UnityEvent _startOfBattleEvent = new UnityEvent();

    private UnityEvent _battleLostEvent = new UnityEvent();
    private UnityEvent _battleWonEvent = new UnityEvent();

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
        StartCoroutine(ProgressToStart());
    }

    private IEnumerator ProgressToStart()
    {
        yield return new WaitForSeconds(_timeToStart);
        SetGameplayState(GameplayStates.Battle);
    }

    /// <summary>
    /// Changes which state the gameplay is to another one and invokes events for that state
    /// </summary>
    /// <param name="newGameplayState"></param>
    public void SetGameplayState(GameplayStates newGameplayState)
    {
        if (_currentGameplayState == newGameplayState || _currentGameplayState == GameplayStates.PostBattleLost ||
            _currentGameplayState == GameplayStates.PostBattleWon) return;

        _currentGameplayState = newGameplayState;

        switch(_currentGameplayState)
        {
            case (GameplayStates.Battle):
                InvokeStartOfBattleEvent();
                break;
            case (GameplayStates.PostBattleLost):
                InvokeBattleLostEvent();
                break;
            case (GameplayStates.PostBattleWon):
                UniversalManagers.Instance.GetSaveManager().SaveBossDifficultyHeroesDictionary();
                InvokeBattleWonEvent();
                break;
        }
    }

    #region BaseManager
    public override void SubscribeToEvents()
    {
        
    }
    
    public void InvokeStartOfBattleEvent()
    {
        _startOfBattleEvent?.Invoke();
    }
    public void InvokeBattleLostEvent()
    {
        _battleLostEvent?.Invoke();
    }
    public void InvokeBattleWonEvent()
    {
        _battleWonEvent?.Invoke();
    }

    #endregion

    #region Getters
    public bool GetIsFightOver() => (_currentGameplayState == GameplayStates.PostBattleLost
        || _currentGameplayState == GameplayStates.PostBattleWon);

    public UnityEvent GetStartOfBattleEvent() => _startOfBattleEvent;
    public UnityEvent GetBattleLostEvent() => _battleLostEvent;
    public UnityEvent GetBattleWonEvent() => _battleWonEvent;
    #endregion
}

public enum GameplayStates
{
    PreBattle,
    Battle,
    PostBattleLost,
    PostBattleWon
};